using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectingPanel : MonoBehaviour
{
	private NetworkManager networkManager;

	#region Unity Methods
	private void OnEnable()
	{
		StartWaitForConnection();
	}
	#endregion

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
		}

		while (networkManager.IsConnectedClient == false && networkManager.IsHost == false)
		{
			yield return new WaitForSeconds(1f);
		}

		UiManager.Instance.OpenNextPanel();
	}
}
