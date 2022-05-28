using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SelectedBotChangedEvent : UnityEvent<Bot> { }

public class Player : NetworkBehaviour
{

	[SerializeField] private ulong playerId;
	[SerializeField] private NetworkObject playerObject;
	[SerializeField] private ApplicationManager appManager;
	[SerializeField] private Camera playerCamera;
	[SerializeField] private Transform operatorCameraPoint;

	public Bot CurrentlyUsingBot { get { return currentlyUsingBot; } }
	[SerializeField] private Bot currentlyUsingBot = null;

	public Bot SelectedBot
	{
		get { return selectedBot; }
		set
		{
			selectedBot = value;
			selectedBotChanged.Invoke(value);
		}
	}
	[SerializeField] private Bot selectedBot;

	public SelectedBotChangedEvent selectedBotChanged;

	private void Awake()
	{
		if (selectedBotChanged == null)
			selectedBotChanged = new SelectedBotChangedEvent();
	}

	void Start()
	{
		playerCamera = Camera.main;
		operatorCameraPoint = GameObject.FindGameObjectWithTag("OperatorCameraPoint").transform;
		appManager = ApplicationManager.Instance;
		playerObject = GetComponent<NetworkObject>();
		playerId = playerObject.OwnerClientId;

		appManager.PlayerModeChangedEvent.AddListener(OnPlayModeChanged);
	}

	private void OnPlayModeChanged(PlayerModeOption mode)
	{
		switch (mode)
		{
			case PlayerModeOption.None:
				break;
			case PlayerModeOption.Operator:
				playerCamera.transform.position = operatorCameraPoint.position;
				playerCamera.orthographicSize = 15;
				break;
			case PlayerModeOption.Bot:
				playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
				playerCamera.transform.parent = transform;
				playerCamera.orthographicSize = 5;
				break;
			default:
				break;
		}
	}

	public void CreateBot(ulong playerId, string name)
	{
		//Debug.Log($"Player.Createbot called. playerId = {playerId}, name = {name}");
		appManager.SpawnBot(playerId, name);
	}
	public void CreateBot() { appManager.SpawnBot(playerId, "Bot"); }

	public void UseBot() { StartCoroutine(UseBotRoutine(playerId, selectedBot)); }
	public IEnumerator UseBotRoutine(ulong playerId, Bot bot)
	{
		var playerMovement = GetComponent<PlayerNetworkedMovement>();

		playerMovement.AllowMovement = false;

		if (currentlyUsingBot != null)
			appManager.RequestRemoveBotServerRpc(currentlyUsingBot.NetworkObjectId);

		yield return new WaitForSeconds(0.5f);

		transform.position = bot.transform.position;


		yield return new WaitForSeconds(1f);

		//appManager.RequestBotOwnershipServerRpc(playerId, bot.NetworkObjectId);

		

		appManager.RequestBotReparentingServerRpc(playerId, bot.NetworkObjectId);

		yield return new WaitForSeconds(0.5f);

		currentlyUsingBot = bot;

		playerMovement.AllowMovement = true;



		//Bot oldBot = GetComponentInChildren<Bot>();
		//if (oldBot != null)
		//{
		//	NetworkObject oldBotNo = oldBot.GetComponent<NetworkObject>();
		//	appManager.RequestOldBotRemoval(playerId);
		//	while (oldBotNo.IsOwner) { yield return null; }
		//}

		//if (!bot.IsOwner)
		//{
		//	appManager.RequestBotOwnership(playerId, bot);
		//	while (!bot.IsOwner) { yield return null; }
		//}
		//bot.IsActive = true;
		//appManager.MovePlayerToPosition(playerId, bot.transform.position);
		//while (transform.position != bot.transform.position) { yield return null; }
		//appManager.RequestBotReparenting(playerId, bot.NetworkObjectId);
		//while (!bot.transform.IsChildOf(transform)) { yield return null; }
		//appManager.RequestResetBotLocalPosition(bot.NetworkObjectId);

		//appManager.RequestBotUse(playerId,bot, currentlyUsingBot);
		yield return null;

		//StartCoroutine(StartBotTrackCoroutine(bot));
	}
	public IEnumerator StartBotTrackCoroutine(Bot bot)
	{
		while (true)
		{
			Debug.Log($"BOT TRACK POS: x{bot.transform.localPosition.x} y{bot.transform.localPosition.y}");
			yield return null;
		};
	}
	public void SetCurrentlyUsingBot(Bot bot)
	{
		currentlyUsingBot = bot;
	}
	public void SetSelectedBotName(string name)
	{
		if (!selectedBot.IsOwner)
			appManager.RequestBotOwnership(playerId, selectedBot);

		if (name == null || name == "")
			name = $"Player {playerId}'s Bot";

		selectedBot.SetBotName(name);

	}

	public void SetSelectedBotMessage(string message)
	{
		if (!selectedBot.IsOwner)
			appManager.RequestBotOwnership(playerId, selectedBot);

		selectedBot.SetBotMessage(message);
	}







}
