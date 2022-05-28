using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModePanel : MonoBehaviour
{
	[SerializeField] private Button operatorModeButton;
	[SerializeField] private Button botModeButton;


	private void Awake()
	{
		operatorModeButton.onClick.AddListener(OperatorModeButtonClicked);
		botModeButton.onClick.AddListener(BotModeButtonClicked);
	}


	private void OperatorModeButtonClicked()
	{
		ApplicationManager.Instance.PlayerMode = PlayerModeOption.Operator;

	}
	private void BotModeButtonClicked()
	{
		ApplicationManager.Instance.PlayerMode = PlayerModeOption.Bot;
	}
}
