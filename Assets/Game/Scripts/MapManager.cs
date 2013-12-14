using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour 
{
	public Tile tilePrefab;
	public TileObjects tileObjects;
	public int gridX = 3;
	public int gridY = 3;
	public GameObject policeStationPrefab;
	private List<Tile> tiles = new List<Tile>();
	
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
		for (int i = 0; i < gridY; i++)
		{
			for (int e = 0; e < gridX; e++)
			{	
				Tile go = Instantiate(tilePrefab, new Vector3(e*tilePrefab.size.width, 0, i*tilePrefab.size.height), Quaternion.identity) as Tile;
				tiles.Add(go);				
			}
		}		
		
		tiles[0].tileObject = (GameObject)Instantiate(policeStationPrefab, Vector3.zero, Quaternion.identity);
		int tempId = 0;
		
		foreach (Tile t in tiles)
		{
			t.id = tempId++;
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
