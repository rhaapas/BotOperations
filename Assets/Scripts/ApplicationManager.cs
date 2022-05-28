using System;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public enum PlayerModeOption { None, Operator, Bot }
[System.Serializable]
public class PlayerModeChangedEvent : UnityEvent<PlayerModeOption> { }

public class ApplicationManager : NetworkBehaviour
{
	[Header("References")]
	[SerializeField] private GameObject botPrefab;
	[SerializeField] private Transform botSpawnPoint;

	[Header("Events")]
	public PlayerModeChangedEvent PlayerModeChangedEvent;
	public UnityEvent BotListChangedEvent;
	public PlayerModeOption PlayerMode
	{
		get { return playerMode; }
		set
		{
			playerMode = value;
			PlayerModeChangedEvent.Invoke(value);
		}
	}
	private PlayerModeOption playerMode = PlayerModeOption.None;

	public ObservableCollection<Bot> BotList { get { return botList; } }
	[SerializeField] private ObservableCollection<Bot> botList = new ObservableCollection<Bot>();
	public static ApplicationManager Instance { get { return instance; } }
	private static ApplicationManager instance;

	#region Unity methods
	private void Awake()
	{
		if (instance != null && instance != this)
			Destroy(this);
		else instance = this;
	}
	private void Start()
	{
		if (PlayerModeChangedEvent == null)
			PlayerModeChangedEvent = new PlayerModeChangedEvent();

		if (botSpawnPoint == null)
			botSpawnPoint = GameObject.FindGameObjectWithTag("NewBotSpawnPoint").transform;
	}
	#endregion

	public void SpawnBot(ulong ownerPlayerId, string name)
	{
		if (IsServer)
		{

			GameObject botObject = Instantiate(botPrefab, botSpawnPoint.position, Quaternion.identity);
			NetworkObject botNo = botObject.GetComponent<NetworkObject>();

			botNo.SpawnWithOwnership(ownerPlayerId);

			Bot bot = botObject.GetComponent<Bot>();
			bot.SetBotName(name); //Bot handles its own RPC's, so need to only call by server.

			botList.Add(bot);
			AddBotToListClientRpc(botNo.NetworkObjectId);

			SetSelectedBotOnPlayerClientRpc(ownerPlayerId, botNo.NetworkObjectId);

			Debug.Log($"Appmanager Spawnbot: (IsServer) . playerId = {ownerPlayerId}, name = {name}");

		}
		if (IsClient && !IsServer)
		{
			RequestSpawnBotServerRpc(ownerPlayerId, name);
		}
	}
	public void RequestBotOwnership(ulong playerId, Bot bot)
	{
		var botNetworkObjectId = bot.GetComponent<NetworkObject>().NetworkObjectId;
		if (IsServer)
		{
			bot.GetComponent<NetworkObject>().ChangeOwnership(playerId);
		}
		else if (IsClient && !IsServer)
		{
			RequestBotOwnershipServerRpc(playerId, botNetworkObjectId);
		}
	}
	public Player GetLocalPlayer()
	{
		Player[] players = FindObjectsOfType<Player>();
		foreach (Player player in players)
		{
			if (player.IsOwner)
				return player;
		}

		Debug.LogError("No player found. Something is wrong.");
		return null;
	}
	public Player GetPlayerWithId(ulong ownerClientId) 
	{
		Player[] players = FindObjectsOfType<Player>();
		foreach (Player player in players)
		{
			if (player.OwnerClientId == ownerClientId)
				return player;
		}

		Debug.LogError($"No player found with id of {ownerClientId}. Something is wrong.");
		return null;
	}

	#region Server RPC's
	[ServerRpc(RequireOwnership = false)]
	public void RequestBotReparentingServerRpc(ulong playerId, ulong botNetworkId) // USED
	{

		Player player = GetPlayerWithId(playerId);
		Bot bot = GetNetworkObject(botNetworkId).GetComponent<Bot>();
		bot.transform.parent = player.transform;
	}
	[ServerRpc(RequireOwnership = false)]
	public void RequestRemoveBotServerRpc(ulong botNetworkId) //USED
	{
		var bot = GetNetworkObject(botNetworkId);
		bot.transform.parent = null;
	}
	[ServerRpc(RequireOwnership = false)]
	private void RequestSpawnBotServerRpc(ulong ownerPlayerId, string name) { SpawnBot(ownerPlayerId, name); }
	[ServerRpc(RequireOwnership = false)]
	public void RequestBotOwnershipServerRpc(ulong playerId, ulong botNetworkObjectId)
	{
		GetNetworkObject(botNetworkObjectId).ChangeOwnership(playerId);
	}
	#endregion

	#region Client RPC's
	[ClientRpc]
	private void AddBotToListClientRpc(ulong NetworkObjectId) //USED
	{
		if (IsServer) return;

		Bot bot = GetNetworkObject(NetworkObjectId).GetComponent<Bot>();
		if (!botList.Contains(bot))
			botList.Add(bot);
	}
	[ClientRpc]
	private void SetSelectedBotOnPlayerClientRpc(ulong ownerPlayerId, ulong networkObjectId) //USED
	{
		if (IsServer) return;

		Player player = GetLocalPlayer();
		if (player.OwnerClientId == ownerPlayerId)
		{
			NetworkObject botNo = GetNetworkObject(networkObjectId);
			player.SelectedBot = botNo.GetComponent<Bot>();
		}
	}
	#endregion

}
