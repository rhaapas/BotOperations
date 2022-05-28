using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Button quitButton;
	[SerializeField] private Button backButton;

	#region Unity Methods
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
	#endregion

	#region Event Methods
	private void BackButtonClicked()
	{
		gameObject.SetActive(false);
	}
	private void QuitButtonClicked()
	{
		Application.Quit();
	}
	#endregion
}
