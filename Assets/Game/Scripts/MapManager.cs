using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour 
{
	public Tile tilePrefab;
	public int gridX = 3;
	public int gridY = 3;
	public GameObject policeStationPrefab;
	private List<Tile> tiles = new List<Tile>();
	
	public Tile[,] tiles_map;
	public MapLoader ml;
	// Use this for initialization
	void Start () 
	{		
		GenerateGrid();
	}
	
	// Update is called once per frame
	void Update ()
	{	
		//GridSelection();
	}
	
	void GenerateGrid()
	{
	MapData md = ml.Maps[0];
		
	gridX=md.map_data.GetLength(1);
	gridY=md.map_data.GetLength(0);
		
	tiles_map=new Tile[gridY,gridX];

		for (int i = 0; i < gridY; i++)
		{
			for (int e = 0; e < gridX; e++)
			{	
				Tile go = Instantiate(tilePrefab, new Vector3(e*tilePrefab.size.width, 0, i*tilePrefab.size.height), Quaternion.identity) as Tile;
				tiles.Add(go);	
				tiles_map[i,e]=go;
				go.TilePosition=new Vector2(i,e);
			}
		}	
		
		int tempId = 0;

		for (int i = 0; i < gridY; i++)
		{
			for (int e = 0; e < gridX; e++)
			{
				//Debug.Log(tiles[i*e+1].tileObject);
				switch (md.map_data[i,e])
				{
					case "p":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(policeStationPrefab, tiles_map[i,e].transform.position, Quaternion.identity);						
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
				int ii=0;
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
