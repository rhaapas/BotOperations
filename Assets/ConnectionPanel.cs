using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour
{
	[SerializeField] private Button startHostButton;
	[SerializeField] private Button startClientButton;
	[SerializeField] private TMP_Text ipText4;
	[SerializeField] private TMP_Text ipText6;
	[SerializeField] private TMP_InputField ipInputField;
	[SerializeField] private TMP_InputField portInputField;
	[SerializeField] private UNetTransport uNetTransport;

	private void Awake()
	{
		startHostButton.onClick.AddListener(StartHosting);
		startClientButton.onClick.AddListener(StartClient);

		GetLocalIPAddress();
		GetPublicIpAddressAsync();

	}
	public void GetLocalIPAddress()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				ipText4.text = "Local IP: " + ip.ToString();
			}
		}
	}

	public async Task GetPublicIpAddressAsync()
	{
		var httpClient = new HttpClient();
		ipText6.text = "Public IP: " + await httpClient.GetStringAsync("https://api.ipify.org");
	}

	
	private void StartHosting()
	{

		uNetTransport.ConnectAddress = ipInputField.text;
		uNetTransport.ConnectPort = int.Parse(portInputField.text);

		if (NetworkManager.Singleton.StartHost())
		{
			Debug.Log("Host started..");
			UiManager.Instance.OpenNextPanel();
		}

	}
	private void StartClient()
	{
		NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipInputField.text;
		NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = int.Parse(portInputField.text);


		if (NetworkManager.Singleton.StartClient())
		{
			Debug.Log("Client started..");
			UiManager.Instance.OpenNextPanel();
		}
	}

}
