using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour 
{
	public Tile tilePrefab;
	public int gridX = 3;
	public int gridY = 3;
	
	public GameObject policeStationPrefab;
	public GameObject cityHallPrefab;
	public GameObject alleyPrefab;
	public GameObject foxxyPrefab;
	public GameObject urbanDrugstashPrefab;
	public GameObject newsStationPrefab;
	
	public GameObject tilePolice;
	public GameObject tileAlley;
	public GameObject tileStraight;
	public GameObject tileCrossroad;
	public GameObject tileDeadend;
	
	private List<Tile> tiles = new List<Tile>();
	
	public Tile[,] tiles_map;
	public MapLoader ml;
	
	public bool GenerateOnStartUp=true;
	// Use this for initialization
	void Start ()
	{
		if (GenerateOnStartUp){
			//ml=GameObject.FindGameObjectWithTag("Databases").GetComponent<MapLoader>();
			GenerateGrid();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{	
		//GridSelection();
	}
	
	public void GenerateGrid()
	{
	MapData md = ml.Maps[0];
		
	gridX=md.map_data.GetLength(0);
	gridY=md.map_data.GetLength(1);
		
	tiles_map=new Tile[gridX,gridY];

		for (int i = 0; i < gridX; i++)
		{
			for (int e = 0; e < gridY; e++)
			{	
				Tile go = Instantiate(tilePrefab, new Vector3(i*tilePrefab.size.width, 0, e*tilePrefab.size.height), Quaternion.identity) as Tile;
				tiles.Add(go);	
				tiles_map[i,e]=go;
				go.TilePosition=new Vector2(i,e);
			}
		}	
		
		int tempId = 0;
		for (int i = 0; i < gridX; i++)
		{
			for (int e = 0; e < gridY; e++)
			{
				//Debug.Log(tiles[i*e+1].tileObject);
				switch (md.map_data[i,e])
				{
					case "p":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(policeStationPrefab, tiles_map[i,e].transform.position, policeStationPrefab.transform.rotation);
						//tiles_map[i,e].tileGround = (GameObject)Instantiate(tilePolice, tiles_map[i,e].transform.position, tilePolice.transform.rotation);
					break;
					case "a":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(alleyPrefab, tiles_map[i,e].transform.position, alleyPrefab.transform.rotation);
					break;
					case "n":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(newsStationPrefab, tiles_map[i,e].transform.position, newsStationPrefab.transform.rotation);
					break;
					case "u":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(urbanDrugstashPrefab, tiles_map[i,e].transform.position, urbanDrugstashPrefab.transform.rotation);
					break;
					case "c":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(cityHallPrefab, tiles_map[i,e].transform.position, cityHallPrefab.transform.rotation);
					break;
					case "|":
						tiles_map[i,e].tileGround = (GameObject)Instantiate(tileStraight, tiles_map[i,e].transform.position, tileStraight.transform.rotation);
					break;
					case "x":
						tiles_map[i,e].tileGround = (GameObject)Instantiate(tileCrossroad, tiles_map[i,e].transform.position, tileCrossroad.transform.rotation);
					break;
					case "-":
						tiles_map[i,e].tileGround = (GameObject)Instantiate(tileStraight, tiles_map[i,e].transform.position, Quaternion.Euler(new Vector3(-90,0,0)));
					break;
					case ".":
						tiles_map[i,e].tileGround = (GameObject)Instantiate(tileCrossroad, tiles_map[i,e].transform.position, tileCrossroad.transform.rotation);
					break;
					case "t":
						tiles_map[i,e].tileGround = (GameObject)Instantiate(tileDeadend, tiles_map[i,e].transform.position, tileDeadend.transform.rotation);
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
