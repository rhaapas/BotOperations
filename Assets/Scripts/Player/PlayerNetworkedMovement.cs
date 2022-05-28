using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkedMovement : NetworkBehaviour
{
	[Header("Settings")]
	[SerializeField] private float moveSpeed = 2f;

	public bool IsMoving { get { return isMoving; } }
	private bool isMoving = false;
	public bool AllowMovement { get { return allowMovement; } set { allowMovement = value; } }
	[SerializeField] private bool allowMovement;

	#region Unity Methods
	void Update()
	{
		if (IsClient && IsOwner)
		{
			UpdateClient();
		}
	}
	#endregion

	#region Input
	private void UpdateClient()
	{

		if (allowMovement)
		{
			float x = transform.position.x;
			float y = transform.position.y;


			if (Input.GetAxis("Horizontal") != 0)
			{
				MoveHorizontal(Input.GetAxis("Horizontal"), out x);
				isMoving = true;
			}
			if (Input.GetAxis("Vertical") != 0)
			{
				MoveVertical(Input.GetAxis("Vertical"), out y);
				isMoving = true;
			}
			if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
			{
				isMoving = false;
			}
		}
	}
	#endregion

	#region Movement
	private void MoveVertical(float value, out float newValue)
	{
		newValue = transform.position.y;

		if (value < 0 && transform.position.y <= 0.5f)
			return;
		if (value > 0 && transform.position.y >= 28.5f)
			return;

		transform.Translate(0, value * Time.deltaTime * moveSpeed, 0);

		newValue = transform.position.y;

	}
	private void MoveHorizontal(float value, out float newValue)
	{
		newValue = transform.position.x;

		if (value < 0 && transform.position.x <= 0.5f)
			return;
		if (value > 0 && transform.position.x >= 28.5f)
			return;

		transform.Translate(value * Time.deltaTime * moveSpeed, 0, 0);

		newValue = transform.position.x;

	}
	#endregion
}
