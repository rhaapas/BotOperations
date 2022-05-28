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
	[SerializeField] private GameObject botPrefab;
	[SerializeField] private Transform botSpawnPoint;

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
	//public void SpawnBot()
	//{
	//	if (IsServer)
	//	{
	//		GameObject bot = Instantiate(botPrefab, botSpawnPoint.position, Quaternion.identity);
	//		bot.GetComponent<NetworkObject>().Spawn();
	//	}
	//	if (IsClient && !IsServer)
	//	{
	//		RequestSpawnBotServerRpc();
	//	}
	//}

	//public void RequestBotUse(ulong playerId, Bot bot, Bot previousBot = null)
	//{

	//	ulong botNetworkId = bot.NetworkObjectId;



	//	if (IsServer)
	//	{
	//		StartCoroutine(UseBotRoutine(playerId, botNetworkId, previousBot));
	//		UseBotClientRpc(playerId, botNetworkId);
	//	}
	//	if (IsClient && !IsServer)
	//	{
	//		if (previousBot != null)
	//		{
	//			ulong previousBotId = previousBot.NetworkObjectId;
	//			UseBotServerRpc(playerId, botNetworkId, previousBotId);
	//		}
	//		else
	//		{
	//			UseBotServerRpc(playerId, botNetworkId);
	//		}

	//	}
	//}

	//private IEnumerator UseBotRoutine(ulong playerId, ulong botNetworkId, Bot previousBot = null)
	//{
	//	NetworkObject bot = GetNetworkObject(botNetworkId);
	//	Player player = GetPlayerWithId(playerId);


	//	if (IsServer && previousBot != null)
	//		previousBot.transform.SetParent(null, true);


	//	if (player.transform.position != bot.transform.position)
	//		player.transform.position = bot.transform.position;



	//	if (bot.OwnerClientId != playerId)
	//		if (IsServer)
	//			bot.ChangeOwnership(playerId);

	//	while (bot.OwnerClientId != playerId)
	//	{
	//		Debug.Log($"UseBotRoutine: Waiting for player to recieve ownership of bot. bot owner id: {bot.OwnerClientId}, {playerId} ");

	//		yield return null;
	//	}

	//	if (bot.transform.parent != player.transform)
	//		if (IsServer)
	//			bot.transform.SetParent(player.transform);

	//	while (bot.transform.parent != player.transform)
	//	{
	//		yield return null;
	//		Debug.Log($"UseBotRoutine: Waiting for bot reparenting");

	//	}

	//	if (bot.transform.localPosition != Vector3.zero)
	//		bot.transform.localPosition = Vector3.zero;

	//	while (bot.transform.localPosition != Vector3.zero)
	//	{
	//		yield return null;
	//		bot.transform.localPosition = Vector3.zero;
	//		Debug.Log($"UseBotRoutine: Waiting for bot localposition to get zero {bot.transform.localPosition}");

	//	}

	//	Debug.Log("PAPAPAPAPAPPA " + bot.transform.localPosition);

	//	yield return null;//
	//}

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
	//public void RequestBotReparenting(ulong playerId, ulong botNetworkObjectId)
	//{
	//	var player = GetPlayerWithId(playerId);
	//	//var botNetworkObjectId = bot.GetComponent<NetworkObject>().NetworkObjectId;
	//	if (IsServer)
	//	{
	//		//ReparentOldBot(player.transform);
	//		Debug.Log($"Setting previous bot parent (IsServer):");
	//		Transform botTransform = GetNetworkObject(botNetworkObjectId).transform;
	//		botTransform.SetParent(player.transform, true);
	//		//bot.transform.SetParent(player.transform, true);
	//		//bot.transform.position = player.transform.position;
	//		//if (botTransform.localPosition != Vector3.zero)
	//		//{
	//		//	botTransform.localPosition = Vector3.zero;
	//		//}
	//		//ResetBotLocalPositionClientRpc(botNetworkObjectId);

	//	}
	//	else if (IsClient && !IsServer)
	//	{
	//		RequestBotReparentingServerRpc(playerId, botNetworkObjectId);
	//	}
	//}

	//public void RequestResetBotLocalPosition(ulong botNetworkObjectId)
	//{
	//	if (IsServer)
	//	{
	//		Transform botTransform = GetNetworkObject(botNetworkObjectId).transform;

	//		if (botTransform.localPosition != Vector3.zero)
	//		{
	//			botTransform.localPosition = Vector3.zero;
	//		}
	//		ResetBotLocalPositionClientRpc(botNetworkObjectId);
	//	}
	//	if (IsClient && !IsServer)
	//	{
	//		ResetBotLocalPositionServerRpc(botNetworkObjectId);
	//	}
	//}

	//public void RequestOldBotRemoval(ulong playerId)
	//{
	//	if (IsServer)
	//	{
	//		Player player = GetPlayerWithId(playerId);
	//		ReparentOldBot(player.transform);
	//		Debug.Log("old bot removed server.");
	//	}
	//	else if (IsClient && !IsServer)
	//	{
	//		RequestOldBotRemovalServerRpc(playerId);
	//	}
	//}

	//private void ReparentOldBot(Transform playerTransform)
	//{
	//	var childNos = playerTransform.GetComponentsInChildren<NetworkObject>();
	//	foreach (var childNo in childNos)
	//	{

	//		if (childNo.transform != playerTransform)
	//		{
	//			childNo.transform.SetParent(null, true);
	//			childNo.RemoveOwnership();
	//		}
	//	}
	//}

	//public void MovePlayerToPosition(ulong playerId, Vector2 position)
	//{
	//	if (IsServer)
	//	{
	//		Player playerObject = GetPlayerWithId(playerId);
	//		playerObject.transform.position = position;
	//		Debug.Log("player position changed server.");
	//		MovePlayerToPositionClientRpc(playerId, position);
	//	}
	//	else if (IsClient && !IsServer)
	//	{
	//		MovePlayerToPositionServerRpc(playerId, position);
	//	}
	//}

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
	public Player GetPlayerWithId(ulong ownerClientId) //testing.. was networkObjectId
	{
		Player[] players = FindObjectsOfType<Player>();
		foreach (Player player in players)
		{
			//Debug.Log("NetworkObjectId" + player.NetworkObjectId);
			//Debug.Log("OwnerClientId " + player.OwnerClientId);
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
		//player.SetCurrentlyUsingBot(bot);
	}
	[ServerRpc(RequireOwnership = false)]
	public void RequestRemoveBotServerRpc(ulong botNetworkId) //USED
	{
		var bot = GetNetworkObject(botNetworkId);
		bot.transform.parent = null;
	}

	//[ServerRpc(RequireOwnership = false)]
	//private void UseBotServerRpc(ulong playerId, ulong botNetworkId)
	//{
	//	Bot bot = GetNetworkObject(botNetworkId).GetComponent<Bot>();
	//	RequestBotUse(playerId, bot);
	//}
	//[ServerRpc(RequireOwnership = false)]
	//private void UseBotServerRpc(ulong playerId, ulong botNetworkId, ulong oldBotId)
	//{
	//	Bot bot = GetNetworkObject(botNetworkId).GetComponent<Bot>();
	//	Bot oldBot = GetNetworkObject(oldBotId).GetComponent<Bot>();
	//	RequestBotUse(playerId, bot, oldBot);
	//}
	//[ServerRpc(RequireOwnership = false)]
	//private void ResetBotLocalPositionServerRpc(ulong botNetworkObjectId)
	//{
	//	RequestResetBotLocalPosition(botNetworkObjectId);
	//	//Transform botTransform = GetNetworkObject(botNetworkObjectId).transform;

	//	//if (botTransform.localPosition != Vector3.zero)
	//	//{
	//	//	botTransform.localPosition = Vector3.zero;
	//	//}

	//}
	//[ServerRpc(RequireOwnership = false)]
	//private void RequestOldBotRemovalServerRpc(ulong playerId)
	//{
	//	Transform playerTransform = GetPlayerWithId(playerId).transform;
	//	ReparentOldBot(playerTransform);
	//	Debug.Log("old bot removed server rpc");
	//}
	//[ServerRpc(RequireOwnership = false)]
	//private void RequestBotReparentingServerRpc(ulong playerId, ulong botNetworkObjectId)
	//{
	//	//NetworkObject botNo = GetNetworkObject(botNetworkObjectId);
	//	//Player player = GetPlayerWithId(playerId);

	//	////player.GetComponentInChildren<NetworkObject>().transform.SetParent(null);
	//	////ReparentOldBot(player.transform);
	//	//Debug.Log("bot reparented server rpc.");


	//	//botNo.transform.SetParent(player.transform, true);
	//	////botNo.transform.position = player.transform.position;
	//	RequestBotReparenting(playerId, botNetworkObjectId);
	//}
	//[ServerRpc(RequireOwnership = false)]
	//private void RequestSpawnBotServerRpc() { SpawnBot(); }
	[ServerRpc(RequireOwnership = false)]
	private void RequestSpawnBotServerRpc(ulong ownerPlayerId, string name) { SpawnBot(ownerPlayerId, name); }
	[ServerRpc(RequireOwnership = false)]
	public void RequestBotOwnershipServerRpc(ulong playerId, ulong botNetworkObjectId)
	{
		GetNetworkObject(botNetworkObjectId).ChangeOwnership(playerId);
	}
	//[ServerRpc(RequireOwnership = false)]
	//private void MovePlayerToPositionServerRpc(ulong playerId, Vector2 position) { MovePlayerToPosition(playerId, position); }
	#endregion


	#region Client RPC's
	//[ClientRpc]
	//private void UseBotClientRpc(ulong playerId, ulong botNetworkId)
	//{
	//	StartCoroutine(UseBotRoutine(playerId, botNetworkId));
	//}
	//[ClientRpc]
	//private void ResetBotLocalPositionClientRpc(ulong botNetworkObjectId)
	//{
	//	Transform botTransform = GetNetworkObject(botNetworkObjectId).transform;

	//	if (botTransform.localPosition != Vector3.zero)
	//	{
	//		botTransform.localPosition = Vector3.zero;
	//	}
	//	Debug.Log($"Resetting bot localposition clientrpc. x{botTransform.localPosition.x} y{botTransform.localPosition.y}");
	//}
	//[ClientRpc]
	//private void MovePlayerToPositionClientRpc(ulong ownerClientId, Vector2 position)
	//{
	//	if (IsServer) return;

	//	Transform playerTransform = GetPlayerWithId(ownerClientId).transform;
	//	playerTransform.position = position;
	//	Debug.Log($"player position changed client rpc. x{playerTransform.position.x} y{playerTransform.position.y}");

	//}
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
