using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGridFromTiles : MonoBehaviour
{
	public GameObject tilePrefab;
	public Vector2 gridSize;

	[ExecuteInEditMode]
	[ContextMenu("Spawn Grid")]
	public void SpawnGrid()
	{
		Vector2 spawnPostition = Vector2.zero;
		for (int i = 0; i < gridSize.x; i++)
		{
			for (int j = 0; j < gridSize.y; j++)
			{
				Instantiate(tilePrefab, spawnPostition, Quaternion.identity, transform);
				spawnPostition.y += 1;
			}
			spawnPostition.x += 1;
			spawnPostition.y = 0;
		}
	}

}
