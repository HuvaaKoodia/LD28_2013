﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour 
{
	public Tile tilePrefab;
	public int gridX = 3;
	public int gridY = 3;
	public GameObject policeStationPrefab;
	private List<Tile> tiles = new List<Tile>();
	
	public MapLoader ml;
	
	// Use this for initialization
	void Start () 
	{		
		GenerateGrid();
	}
	
	// Update is called once per frame
	void Update ()
	{	
		GridSelection();
	}
	
	void GenerateGrid()
	{
		MapData md = ml.Maps[0];
		
		for (int i = 0; i < gridY; i++)
		{
			for (int e = 0; e < gridX; e++)
			{	
				Tile go = Instantiate(tilePrefab, new Vector3(e*tilePrefab.size.width, 0, i*tilePrefab.size.height), Quaternion.identity) as Tile;
				tiles.Add(go);				
			}
		}		
		
		int tempId = 0;
		Debug.Log(md.map_data[0,1]);
		Debug.Log(md.map_data[0,2]);
		Debug.Log(md.map_data[0,3]);
		Debug.Log(md.map_data[0,4]);
		for (int i = 0; i < gridY; i++)
		{
			for (int e = 0; i < gridX; i++)
			{
				Debug.Log(tiles[i*e+1].tileObject);
				switch (md.map_data[e,i])
				{
					case "p":
						tiles[e*i+e].tileObject = (GameObject)Instantiate(policeStationPrefab, Vector3.zero, Quaternion.identity);						
					break;
					case "a":
					break;
					case "s":
					break;
					case "k":
					break;
					case "e":
					break;
					case "x":
					break;
					case ".":
						tiles[i].tileObject = null;
					break;
				}
			}
		}
		
		for (int i = 0; i < tiles.Count; i++)
		{
			tiles[i].id = tempId++;
		}
	}
	
	void UpdateTiles()
	{
		foreach (Tile t in tiles)
		{
			t.UpdateObjects();
		}
	}
	
	void GridSelection()
	{
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out hit, 10) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Tile") )
       	{			
			Tile t = hit.transform.parent.GetComponent<Tile>();
			
			Debug.Log(tiles.Contains(t));
			Debug.Log(t.id);
       	}		
	}	
}
