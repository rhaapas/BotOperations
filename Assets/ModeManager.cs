using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ModeSelection { None, Operator, Bot }
public class ModeManager : MonoBehaviour
{
	[SerializeField] private Button operatorModeButton;
	[SerializeField] private GameObject operatorModeObject;
	[SerializeField] private Button botModeButton;
	[SerializeField] private GameObject botModeObject;
	[SerializeField] private AppManager appManager;
	[SerializeField] private Camera playerCamera;
 	
	
	private void Awake()
	{
		appManager = FindObjectOfType<AppManager>();
		playerCamera = FindObjectOfType<Camera>();

		if (appManager.SelectedMode != ModeSelection.None)
		{
			gameObject.SetActive(false);
		}

		operatorModeButton.onClick.AddListener(OperatorModeButtonClicked);
		botModeButton.onClick.AddListener(BotModeButtonClicked);
		appManager.SetBotModeParent(botModeObject.transform);
	}

	private void Start()
	{
		if (operatorModeObject.activeSelf)
			operatorModeObject.SetActive(false);

		if (botModeObject.activeSelf)
			botModeObject.SetActive(false);
	}
	private void OperatorModeButtonClicked()
	{
		appManager.SetSelectedMode(ModeSelection.Operator);
		operatorModeObject.SetActive(true);
		operatorModeObject.GetComponentInChildren<OperatorModeBotSelectionPanel>().SpawnButtons();
		playerCamera.orthographicSize = 16;
		gameObject.SetActive(false);
	}
	private void BotModeButtonClicked()
	{
		appManager.SetSelectedMode(ModeSelection.Bot);
		botModeObject.SetActive(true);
		botModeObject.GetComponentInChildren<BotModeBotSelectionPanel>().SpawnButtons();
		gameObject.SetActive(false);
		playerCamera.transform.parent = transform.parent;
		playerCamera.transform.localPosition = new Vector3(0,0,-1f);
	}
}
