using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
	[SerializeField] private Button useBotButton;
	[SerializeField] private Button actionButton;
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private TMP_Text actionText;
	[SerializeField] private TMP_Text actionButtonText;
	[SerializeField] private TMP_Text selectedBotText;
	[SerializeField] private Player player = null;

	private EventSystem eventSystem;
	private PlayerNetworkedMovement playerMovement;

	void Start()
	{
		ApplicationManager.Instance.PlayerModeChangedEvent.AddListener(PlayerModeChanged);
		useBotButton.onClick.AddListener(UseBotButtonClicked);
		actionButton.onClick.AddListener(ActionButtonClicked);

		eventSystem = EventSystem.current;

	}

	

	public IEnumerator WaitForConnection()
	{
		while (NetworkManager.Singleton == null)
		{
			yield return null;
		}

		while (NetworkManager.Singleton.IsConnectedClient == false && NetworkManager.Singleton.IsHost == false)
		{
			yield return new WaitForSeconds(1f);
		}

		if (player == null)
		{
			player = ApplicationManager.Instance.GetLocalPlayer();
			player.selectedBotChanged.AddListener(BotSelectionChanged);
		}
	}

	private void ActionButtonClicked()
	{
		if (player == null)
			return;

		switch (ApplicationManager.Instance.PlayerMode)
		{
			case PlayerModeOption.None:
				Debug.LogError("PlayerMode changed to None, this shouldn't happen.");
				break;

			case PlayerModeOption.Operator:
				if (player.SelectedBot != null)
				{
					if (inputField.text != "")
						player.SetSelectedBotMessage(inputField.text);
					else
						Debug.Log("input field is empty");
				}
				else
					Debug.LogWarning("No bot selected.");
				break;

			case PlayerModeOption.Bot:
				if (inputField.text != "")
				player.CreateBot(player.OwnerClientId, inputField.text);
				else
					Debug.Log("input field is empty");
				break;

			default:
				break;

		}
	}

	private void UseBotButtonClicked()
	{
		if (player == null)
			return;

		if (player.SelectedBot != null)
			player.UseBot();
	}

	private void PlayerModeChanged(PlayerModeOption playerMode)
	{
		StartCoroutine(WaitForConnection());

		switch (playerMode)
		{
			case PlayerModeOption.None:
				break;
			case PlayerModeOption.Operator:
				useBotButton.gameObject.SetActive(false);
				actionText.text = "Send a message:";
				actionButtonText.text = "Send";
				break;
			case PlayerModeOption.Bot:
				useBotButton.gameObject.SetActive(true);
				actionText.text = "Create new bot";
				actionButtonText.text = "Create";
				break;
			default:
				break;
		}
	}

	private void BotSelectionChanged(Bot bot)
	{
		selectedBotText.text = bot.GetName();
	}
}
