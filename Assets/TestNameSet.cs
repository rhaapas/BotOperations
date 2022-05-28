using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestNameSet : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(Click);
	}

	private void Click()
	{
		//string name = "bot69";

		//FindObjectOfType<Bot>().SetBotName(name);

		var player = ApplicationManager.Instance.GetLocalPlayer();
		//player.SetSelectedBotName();

	}
}
