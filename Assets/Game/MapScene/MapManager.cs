using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour 
{
	public int gridX = 3;
	public int gridY = 3;
	
	public GameObject policeStationPrefab;
	public GameObject cityHallPrefab;
	public GameObject alleyPrefab;
	public GameObject foxxyPrefab;
	public GameObject urbanDrugstashPrefab;
	public GameObject newsStationPrefab;
	
	public Tile tileStraight;
	public Tile tileCrossroad;
	public Tile tileDeadend;
	
	private List<Tile> tiles = new List<Tile>();
	
	public Tile[,] tiles_map;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{	
		//GridSelection();
	}
	
	public void GenerateGrid(GameDatabase GDB,MapLoader ml)
	{
	MapData md = ml.Maps[0];
		
	gridX=md.map_data.GetLength(0);
	gridY=md.map_data.GetLength(1);
		
	tiles_map=new Tile[gridX,gridY];

		for (int i = 0; i < gridX; i++)
		{
			for (int j = 0; j < gridY; j++)
			{	
				Tile go = Instantiate(tileCrossroad, new Vector3(i*tileCrossroad.size.width, 0, j*tileCrossroad.size.height), Quaternion.identity) as Tile;
				tiles.Add(go);	
				tiles_map[i,j]=go;
				go.TilePosition=new Vector2(i,j);
				go.SetData(GDB.tiledata_map[i,j]);
			}
		}	
		
		for (int i = 0; i < gridX; i++)
		{
			for (int e = 0; e < gridY; e++)
			{
				var pos=tiles_map[i,e].transform.position;

				switch (md.map_data[i,e])
				{
					case "p":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(policeStationPrefab, pos, policeStationPrefab.transform.rotation);
					break;
					case "a":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(alleyPrefab,pos, alleyPrefab.transform.rotation);
					break;
					case "n":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(newsStationPrefab, pos, newsStationPrefab.transform.rotation);
					break;
					case "u":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(urbanDrugstashPrefab, pos, urbanDrugstashPrefab.transform.rotation);
					break;
					case "c":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(cityHallPrefab, pos, cityHallPrefab.transform.rotation);
					break;
					case "f":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(foxxyPrefab, pos, foxxyPrefab.transform.rotation);
					break;
				}
			}
		}
	}
	
	void UpdateTiles()
	{
		foreach (Tile t in tiles)
		{
			t.UpdateObjects();
		}
	}	
}