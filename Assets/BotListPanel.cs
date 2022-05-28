using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BotListPanel : MonoBehaviour
{
	[SerializeField] private GameObject botButtonPrefab;
	[SerializeField] private TMP_Text botListEmptyText;
	[SerializeField] private GameObject botListContentObject;
	[SerializeField] private List<BotDataButton> botButtonList;


	// Start is called before the first frame update
	void Start()
	{
		ApplicationManager.Instance.BotList.CollectionChanged += BotListChanged;
	}

	private void BotListChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		UpdateBotButtonList();
	}

	public void UpdateBotButtonList()
	{
		DestroyAllButtons();
		List<Bot> botList = ApplicationManager.Instance.BotList.ToList();

		foreach (Bot bot in botList)
		{
			if (!bot.IsActive)
			{
				GameObject buttonObject = Instantiate(botButtonPrefab, botListContentObject.transform);
				BotDataButton button = buttonObject.GetComponent<BotDataButton>();
				botButtonList.Add(button);
				button.Initialize(bot);
				bot.SetLinkedButton(button);
			}
			bot.isActiveChanged.AddListener(BotActiveChanged);
		}

		if (botButtonList.Count == 0)
			botListEmptyText.gameObject.SetActive(true);
		else
			botListEmptyText.gameObject.SetActive(false);
	}

	private void BotActiveChanged(Bot bot, bool active)
	{
		if (active)
		{
			botButtonList.Remove(bot.LinkedButton);
			bot.RemoveLinkedButton();
			Destroy(bot.LinkedButton?.gameObject);
		}

	}

	private void DestroyAllButtons()
	{
		foreach (BotDataButton button in botButtonList)
		{
			Destroy(button.gameObject);
		}
			botButtonList.Clear();
	}
}
