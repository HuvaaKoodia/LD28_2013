using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
	public Rect size;
	public Vector3 position;
	public GameObject tileObject;
	
	public Vector2 TilePosition;
		
	public TileData Data{get;private set;}
	public void SetData(TileData data){
		Data=data;
	}
	
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
	
	void AddCharacter()
	{
		
	}

	public bool Blocked ()
	{
		return tileObject!=null;
	}

	
}
