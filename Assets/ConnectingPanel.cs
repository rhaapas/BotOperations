using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectingPanel : MonoBehaviour
{
	private NetworkManager networkManager;

	private void OnEnable()
	{
		StartWaitForConnection();
	}

	public void StartWaitForConnection()
	{
		networkManager = NetworkManager.Singleton;
		StartCoroutine(WaitForConnection());
	}
	public IEnumerator WaitForConnection()
	{
		while( networkManager == null)
		{
			yield return null;
			networkManager = NetworkManager.Singleton;
			Debug.Log("waiting to get networkmanager..");
		}

		while (networkManager.IsConnectedClient == false && networkManager.IsHost == false)
		{
			yield return new WaitForSeconds(1f);
			Debug.Log("waiting to get IsConnectedClient..");
		}
		UiManager.Instance.OpenNextPanel();
	}
}
