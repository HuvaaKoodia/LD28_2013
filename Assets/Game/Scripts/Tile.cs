using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
	public int id;
	public Rect size;
	public Vector3 position;
	public int maxObjects = 10;
	public GameObject tileObject;
	public List<CharacterMain> characters;
	
	public Vector2 TilePosition;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{	
	}
	
	public void UpdateObjects () 
	{
		
	}
	
	void AddCharacter(){
		
	}

	public bool Blocked ()
	{
		return tileObject!=null;
	}
}
