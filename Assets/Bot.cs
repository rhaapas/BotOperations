using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NameChangedEvent : UnityEvent<string> { }
[System.Serializable]
public class MessageChangedEvent : UnityEvent<string> { }
[System.Serializable]
public class IsActiveChangedEvent : UnityEvent<Bot, bool> { }

public class Bot : NetworkBehaviour
{
	//[SerializeField] private Vector2 position;
	[SerializeField] private ForceNetworkSerializeByMemcpy<FixedString64Bytes> botName;
	[SerializeField] private ForceNetworkSerializeByMemcpy<FixedString64Bytes> message;

	[SerializeField] private TMP_Text nameText;
	[SerializeField] private TMP_Text messageText;

	public NameChangedEvent nameChanged;
	public MessageChangedEvent messageChanged;
	public IsActiveChangedEvent isActiveChanged;

	public BotDataButton LinkedButton { get { return linkedButton; } }
	private BotDataButton linkedButton;

	public bool IsActive
	{
		get { return isActive; }
		set
		{
			isActive = value;
			isActiveChanged.Invoke(this, value);
		}
	}
	private bool isActive;

	private void Awake()
	{
		if (nameChanged == null)
			nameChanged = new NameChangedEvent();

		if (messageChanged == null)
			messageChanged = new MessageChangedEvent();

		if (isActiveChanged == null)
			isActiveChanged = new IsActiveChangedEvent();

		nameChanged.AddListener(NameChanged);
		messageChanged.AddListener(MessageChanged);
	}
	#region Test Event Methods
	private void MessageChanged(string arg0) { messageText.text = arg0; }

	private void NameChanged(string arg0) { nameText.text = arg0; }
	#endregion
	public void SetLinkedButton(BotDataButton button) { linkedButton = button; }
	public void RemoveLinkedButton() { linkedButton = null; }
	public string GetName()
	{
		return botName.Value.ToString();
	}
	public void SetBotName(string name)
	{
		if (IsServer)
		{
			
			botName.Value = name;
			gameObject.name = $"Bot ({botName.Value.ToString()})";
			nameChanged.Invoke(name);
			SetBotNameClientRpc(name);
			Debug.Log($"set bot name (IsServer) called, name = {name}, botName.Value = {botName.Value.ToString()}");
		}
		if (IsClient && !IsServer)
		{
			SetBotNameServerRpc(name);
			Debug.Log($"set bot name (IsClient) called, name = {name}, botName.Value = {botName.Value.ToString()}");

		}
	}

	public void SetBotMessage(string newMessage)
	{
		if (IsServer)
		{
			message.Value = newMessage;
			messageChanged.Invoke(message.Value.ToString());
			SetBotMessageClientRpc(message.Value.ToString());
		}
		else if (IsClient && !IsServer)
		{
			SetBotMessageServerRpc(newMessage);
		}
	}

	#region Server RPC's
	[ServerRpc(RequireOwnership = false)]
	private void SetBotMessageServerRpc(string newMessage) { SetBotMessage(newMessage); }
	[ServerRpc(RequireOwnership = false)]
	private void SetBotNameServerRpc(string name) { 
		SetBotName(name);
		Debug.Log($"set bot name (Server RPC) called, name = {name}, botName.Value = {botName.Value.ToString()}");

	}
	#endregion
	#region Client RPC's
	[ClientRpc]
	private void SetBotMessageClientRpc(string newMessage)
	{
		if (IsServer) return;

		message.Value = newMessage;
		Debug.Log($"Client rpc, message = {message.Value}");
		messageChanged.Invoke(message.Value.ToString());
	}

	[ClientRpc]
	private void SetBotNameClientRpc(string name)
	{
		if (IsServer) return;

		botName.Value = name;
		gameObject.name = name;
		//Debug.Log($"Client rpc, botname = {botName.Value}");
		nameChanged.Invoke(name);
		//Debug.Log($"set bot name (Client RPC) called, name = {name}, botName.Value = {botName.Value.ToString()}");

	}

	#endregion

}
