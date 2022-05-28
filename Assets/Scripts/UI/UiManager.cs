using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiManager : MonoBehaviour
{

	[SerializeField] private GameObject connectionPanelObject;
	[SerializeField] private GameObject modePanelObject;
	[SerializeField] private GameObject connectingPanelObject;
	[SerializeField] private GameObject escMenuPanelObject;

	private List<GameObject> mainPanelObjects;
	private int currentPanelIndex = 0;

	public static UiManager Instance { get { return instance; } }
	private static UiManager instance;

	private EventSystem eventSystem;

	private void Awake()
	{
		if (instance != null && instance != this)
			Destroy(this);
		else instance = this;
	}

	void Start()
	{
		eventSystem = EventSystem.current;

		mainPanelObjects = new List<GameObject>() { connectionPanelObject, connectingPanelObject, modePanelObject };

		if (escMenuPanelObject.activeSelf)
			escMenuPanelObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			ToggleEscMenu();
	}

	private void ToggleEscMenu()
	{
		if (escMenuPanelObject.activeSelf)
			escMenuPanelObject.SetActive(false);
		else
			escMenuPanelObject.SetActive(true);
	}

	public void OpenNextPanel()
	{
		if (currentPanelIndex == mainPanelObjects.Count - 1)
		{
			DisableMainPanels();
			return;
		}
		currentPanelIndex++;
		EnableMainPanel(currentPanelIndex);
	}
	public void OpenPreviousPanel()
	{
		if (currentPanelIndex == 0)
			return;
		currentPanelIndex--;
		EnableMainPanel(currentPanelIndex);
	}

	private void EnableMainPanel(GameObject panel)
	{
		if (mainPanelObjects.Contains(panel))
		{
			DisableMainPanels();
			panel.SetActive(true);
		}
	}
	private void EnableMainPanel(int index)
	{
		DisableMainPanels();
		mainPanelObjects[index].SetActive(true);
	}
	private void DisableMainPanels()
	{
		foreach (GameObject panel in mainPanelObjects)
			panel.SetActive(false);
	}

	public void EventSystemDeselectCurrent()
	{
		eventSystem.SetSelectedGameObject(null);
	}

}
