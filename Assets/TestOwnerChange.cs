using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestOwnerChange : MonoBehaviour
{
	public ApplicationManager manager;
	// Start is called before the first frame update
	void Start()
	{
		GetComponent<Button>().onClick.AddListener(Click);
		manager = ApplicationManager.Instance;
	}

	private void Click()
	{
		Player player = manager.GetLocalPlayer();
		Bot bot = FindObjectOfType<Bot>();
		//player.UseBot(player.OwnerClientId, bot);
	}
}
