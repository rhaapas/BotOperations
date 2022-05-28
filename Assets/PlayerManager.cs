using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;




public class PlayerManager : NetworkBehaviour
{
	public PlayerManager Instance { get { return instance; } }
	private PlayerManager instance;
	private void Awake()
	{
		if (instance != null && instance != this)
			Destroy(this);
		else instance = this;
	}
	// Start is called before the first frame update
	void Start()
	{
		NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
		NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
	}

	private void ClientDisconnected(ulong obj)
	{
		if (IsServer)
		{
			Debug.Log("Client disconnected.");
		}
	}

	private void ClientConnected(ulong obj)
	{
		if (IsServer)
		{
			Debug.Log("Client connected.");
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
