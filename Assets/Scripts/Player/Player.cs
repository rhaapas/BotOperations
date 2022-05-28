using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SelectedBotChangedEvent : UnityEvent<Bot> { }

public class Player : NetworkBehaviour
{
	[Header("References")]
	[SerializeField] private Transform operatorCameraPoint;
	[SerializeField] private Camera playerCamera;
	[Space]
	[Header("Debug")]
	[SerializeField] private ulong playerId;
	[SerializeField] private Bot currentlyUsingBot = null;
	[SerializeField] private Bot selectedBot;
	[Header("Events")]
	public SelectedBotChangedEvent selectedBotChanged;

	private NetworkObject playerObject;
	private ApplicationManager appManager;

	public Bot CurrentlyUsingBot { get { return currentlyUsingBot; } }
	public Bot SelectedBot
	{
		get { return selectedBot; }
		set
		{
			selectedBot = value;
			selectedBotChanged.Invoke(value);
		}
	}


	#region Unity methods
	private void Awake()
	{
		if (selectedBotChanged == null)
			selectedBotChanged = new SelectedBotChangedEvent();
	}

	void Start()
	{
		if (playerCamera == null)
			playerCamera = Camera.main;

		if (operatorCameraPoint == null)
			operatorCameraPoint = GameObject.FindGameObjectWithTag("OperatorCameraPoint").transform;

		appManager = ApplicationManager.Instance;
		playerObject = GetComponent<NetworkObject>();
		playerId = playerObject.OwnerClientId;

		appManager.PlayerModeChangedEvent.AddListener(OnPlayModeChanged);
	}
	#endregion

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

	public void CreateBot(ulong playerId, string name) { appManager.SpawnBot(playerId, name); }

	public void UseBot() { StartCoroutine(UseBotRoutine(playerId, selectedBot)); }

	//IEnumerator for quick way for adding delays to compensate for lag and keep excecution order.
	//TODO: rework server/event calls to make sure the rpc's excecute in correct manner with automatic position syncing.
	// (ie. movement has been properly synced with all clients before reparenting happens.)
	private IEnumerator UseBotRoutine(ulong playerId, Bot bot)
	{
		var playerMovement = GetComponent<PlayerNetworkedMovement>();

		playerMovement.AllowMovement = false;

		if (currentlyUsingBot != null)
			appManager.RequestRemoveBotServerRpc(currentlyUsingBot.NetworkObjectId);

		yield return new WaitForSeconds(0.5f);

		transform.position = bot.transform.position;

		yield return new WaitForSeconds(1f);

		appManager.RequestBotReparentingServerRpc(playerId, bot.NetworkObjectId);

		yield return new WaitForSeconds(0.5f);

		currentlyUsingBot = bot;

		playerMovement.AllowMovement = true;

		yield return null;
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
