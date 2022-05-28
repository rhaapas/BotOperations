using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ModeSelectionEnabler : NetworkBehaviour
{
	[SerializeField] private AppManager appManager;
	[SerializeField] private GameObject modeSelection;


	private void Awake()
	{
		appManager = FindObjectOfType<AppManager>();
		//Debug.Log($"mode {appManager.SelectedMode} , IsOwner {IsOwner}");
		//if (appManager.SelectedMode == ModeSelection.None && IsOwner)
		//	modeSelection.SetActive(true);
		//else
		//	modeSelection.SetActive(false);
	}
	private void Start()
	{
		StartCoroutine(DelayedInit(0.3f));
	}

	private IEnumerator DelayedInit(float delay)
	{
		yield return new WaitForSeconds(delay);
		Debug.Log($"mode {appManager.SelectedMode} , IsOwner {IsOwner}");
		if (appManager.SelectedMode == ModeSelection.None && IsOwner)
			modeSelection.SetActive(true);
		else
			modeSelection.SetActive(false);
	}
}
