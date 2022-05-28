using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestMessageSet : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(Click);
	}

	private void Click()
	{
		//string message = "Heisulivei!";

		//FindObjectOfType<Bot>().SetBotMessage(message);
		var player = ApplicationManager.Instance.GetLocalPlayer();
		//player.SetSelectedBotMessage();
	}
}
