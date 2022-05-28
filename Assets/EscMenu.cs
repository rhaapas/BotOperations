using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour
{
	[SerializeField] private Button quitButton;
	[SerializeField] private Button backButton;

	private void OnEnable()
	{
		quitButton.onClick.AddListener(QuitButtonClicked);
		backButton.onClick.AddListener(BackButtonClicked);

	}
	private void OnDisable()
	{
		quitButton.onClick.RemoveListener(QuitButtonClicked);
		backButton.onClick.RemoveListener(QuitButtonClicked);
	}

	private void BackButtonClicked()
	{
		gameObject.SetActive(false);
	}
	private void QuitButtonClicked()
	{
		Application.Quit();
	}
}
