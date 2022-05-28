using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionPanel : MonoBehaviour
{
	[SerializeField] private Button operatorModeButton;
	[SerializeField] private Button botModeButton;

	private void Awake()
	{
		operatorModeButton.onClick.AddListener(OperatorModeButtonClicked);
		botModeButton.onClick.AddListener(BotModeButtonClicked);
	}

	private void BotModeButtonClicked()
	{
		ApplicationManager.Instance.PlayerMode = PlayerModeOption.Bot;
		UiManager.Instance.OpenNextPanel();
		Debug.Log("click");
	}

	private void OperatorModeButtonClicked()
	{
		ApplicationManager.Instance.PlayerMode = PlayerModeOption.Operator;
		UiManager.Instance.OpenNextPanel();
		Debug.Log("click");

	}
}
