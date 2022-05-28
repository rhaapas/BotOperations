using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
	public GameObject RootObject { get { return rootObject; } }	
	[SerializeField] private GameObject rootObject;
	[SerializeField] private float moveSpeed = 1f;

	public bool IsMoving { get; private set; }

	private void Awake()
	{

	}
	void Update()
	{
		if (Input.GetAxis("Horizontal") != 0)
		{
			MoveHorizontal(Input.GetAxis("Horizontal"));
			IsMoving = true;
		}
		if (Input.GetAxis("Vertical") != 0)
		{
			MoveVertical(Input.GetAxis("Vertical"));
			IsMoving = true;
		}
		if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
		{
			IsMoving = false;
		}

	}

	private void MoveVertical(float value)
	{
		if (rootObject != null) 
		{
			if (value < 0 && rootObject.transform.position.y <= 0.5f)
				return;
			if (value > 0 && rootObject.transform.position.y >= 28.5f)
				return;

			rootObject.transform.Translate(0, value * Time.deltaTime * moveSpeed, 0);
		}
		else if (GetComponentInParent<BotContainer>() != null)
			return;
		else
			rootObject = GetComponentInParent<NetworkObject>().gameObject;

	}

	private void MoveHorizontal(float value)
	{
		if (rootObject != null)
		{
			if (value < 0 && rootObject.transform.position.x <= 0.5f)
				return;
			if (value > 0 && rootObject.transform.position.x >= 28.5f)
				return;

			rootObject.transform.Translate(value * Time.deltaTime * moveSpeed, 0, 0);
		}
		else if (GetComponentInParent<BotContainer>() != null)
			return;
		else
			rootObject = GetComponentInParent<NetworkObject>().gameObject;
	}
}
