using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;


public enum BotState { Offline, Online }
public class BotOld : MonoBehaviour
{


	[Header("References")]
	[SerializeField] private SpriteRenderer botSprite;
	[SerializeField] private BotColorSettings botColorSettings;
	//[SerializeField] private GameObject rootObject;
	[SerializeField] private TMP_Text botNameTextObject;
	[SerializeField] private TMP_Text botMessageTextObject;
	[SerializeField] private GameObject botMessagePanelObject;
	[SerializeField] private BotMovement botMovement;
	[SerializeField] private AppManager appManager;
	//[SerializeField] private Camera botCamera;

	[Header("Debug")]
	[SerializeField] private string botName;
	public string BotName { get { return botName; } }
	public BotState BotState { get { return botState; } set { botState = value; } }
	[SerializeField] private BotState botState = BotState.Offline; //remember to remove serialize.
	[SerializeField] private bool messageReceived = false;
	public string Message { get { return currentMessage; } }
	[SerializeField] private string currentMessage = "";

	private void Awake()
	{
		//botCamera = GetComponentInChildren<Camera>();
		//botCamera.gameObject.SetActive(false);
	}

	private void Start()
	{
		//Initíalize("keks", new Vector2(14, 14), "tämä on testiviesti");
		//EditorApplication.playModeStateChanged += EditorQuitting;
	}
	//private void EditorQuitting(PlayModeStateChange obj)
	//{
	//	if (!EditorApplication.isPlayingOrWillChangePlaymode &&
	//		  EditorApplication.isPlaying)
	//	{
	//		appManager.UpdateBotPosition(this);
	//		appManager.UpdateBotMessage(this);
	//	}
	//}

	private void OnApplicationQuit()
	{
		appManager.UpdateBotPosition(this);
		appManager.UpdateBotMessage(this);
	}
	public void Initialize(string initName, Vector2 initPosition, string initMessage = "")
	{
		appManager = FindObjectOfType<AppManager>();
		botMovement = GetComponent<BotMovement>();
		SetBotName(initName);
		SetBotState(BotState.Offline);
		SetMessage(initMessage);
		Debug.Log(currentMessage);
		if (GetComponentInParent<BotContainer>())
			transform.position = initPosition;



		if (messageReceived == true)
			ShowMessagePanel();
		else HideMessagePanel();
	}

	private void HideMessagePanel() { botMessagePanelObject.SetActive(false); }
	private void ShowMessagePanel() { botMessagePanelObject.SetActive(true); }
	public void SetMessage(string message)
	{
		currentMessage = message;
		botMessageTextObject.text = currentMessage;

		if (message != "")
		{
			messageReceived = true;
			ShowMessagePanel();
		}
		else
		{
			messageReceived = false;
			HideMessagePanel();
		}

		SetBotColor();
	}
	private void SetBotState(BotState state)
	{
		botState = state;
		SetBotColor();
	}

	private void SetBotName(string value)
	{
		botName = value;
		botNameTextObject.text = botName;
	}
	public void SetBotColor()
	{
		switch (botState)
		{
			case BotState.Offline:
				if (messageReceived)
					botSprite.color = botColorSettings.messageReceivedOfflineColor;
				else
					botSprite.color = botColorSettings.offlineColor;
				break;

			case BotState.Online:
				if (messageReceived)
					botSprite.color = botColorSettings.messageReceivedOnlineColor;
				else
					botSprite.color = botColorSettings.onlineColor;
				break;

			default:
				break;
		}
	}

	//public void ActivateCamera() { botCamera.gameObject.SetActive(true); }

}
