using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorModeBotSelectionPanel : MonoBehaviour
{
	[SerializeField] private AppManager appManager;
	[SerializeField] private GameObject botDataButtonPrefab;
	[SerializeField] private GameObject scrollViewContainer;
	[SerializeField] private List<BotDataButton> botDataButtons = new List<BotDataButton>();

	private void Awake()
	{
		appManager = FindObjectOfType<AppManager>();
	}
	public void SpawnButtons()
	{
		Debug.Log($"Instatiating buttons... appManager.botsInPlay size = {appManager.botsInPlay.Count}");
		foreach (BotOld bot in appManager.botsInPlay)
		{
			var dataButtonObject = Instantiate(botDataButtonPrefab, scrollViewContainer.transform);
			var dataButton = dataButtonObject.GetComponent<BotDataButton>();
			//dataButton.Initialize(bot.BotName);
			botDataButtons.Add(dataButton);
		}
	}
}
