using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotDataButton : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private TMP_Text botNameText;

	public Bot LinkedBot { get { return linkedBot; } }
	private Bot linkedBot;

	private Player player;

	#region Event Methods
	public void SelectBot()
	{
		if (linkedBot == null)
		{
			Debug.LogError("linked bot is null");
			return;
		}

		player.SelectedBot = linkedBot;
	}
	#endregion

	public void Initialize(Bot bot)
	{
		player = ApplicationManager.Instance.GetLocalPlayer();
		linkedBot = bot;
		botNameText.text = bot.GetName();
		GetComponent<Button>().onClick.AddListener(SelectBot);
	}
}
