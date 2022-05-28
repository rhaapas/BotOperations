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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Button startHostButton;
	[SerializeField] private Button startClientButton;
	[SerializeField] private TMP_Text ipText4;
	[SerializeField] private TMP_Text ipText6;
	[SerializeField] private TMP_InputField ipInputField;
	[SerializeField] private TMP_InputField portInputField;
	[SerializeField] private UNetTransport uNetTransport;

	private EventSystem eventSystem;

	#region Unity Methods
	private void Awake()
	{
		eventSystem = EventSystem.current;

		startHostButton.onClick.AddListener(StartHosting);
		startClientButton.onClick.AddListener(StartClient);

		GetLocalIPAddress();
		GetPublicIpAddressAsync();

	}

	private void Update()
	{
		//Tab navigation for input fields
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (eventSystem.currentSelectedGameObject == ipInputField.gameObject)
				eventSystem.SetSelectedGameObject(portInputField.gameObject);
			else if (eventSystem.currentSelectedGameObject == portInputField.gameObject)
				eventSystem.SetSelectedGameObject(ipInputField.gameObject);
			else eventSystem.SetSelectedGameObject(ipInputField.gameObject);
		}
	}
	#endregion

	#region Connect Methods
	private void StartHosting()
	{
		if (ipInputField.text != "")
			uNetTransport.ConnectAddress = ipInputField.text;
		if(portInputField.text != "")
			uNetTransport.ConnectPort = int.Parse(portInputField.text);

		if (NetworkManager.Singleton.StartHost())
		{
			Debug.Log("Host started..");
			UiManager.Instance.OpenNextPanel();
		}

	}
	private void StartClient()
	{
		if (ipInputField.text != "")
			uNetTransport.ConnectAddress = ipInputField.text;
		if(portInputField.text != "")
			uNetTransport.ConnectPort = int.Parse(portInputField.text);


		if (NetworkManager.Singleton.StartClient())
		{
			Debug.Log("Client started..");
			UiManager.Instance.OpenNextPanel();
		}
	}
	#endregion

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
}
