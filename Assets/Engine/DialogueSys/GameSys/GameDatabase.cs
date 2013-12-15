using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameDatabase : MonoBehaviour {
	
	public CoreDatabase Core;
	public MapLoader mapload;
	
	public MapCharacterData CurrentCharacter;
	public List<MapCharacterData> Characters=new List<MapCharacterData>();
	
	public int current_player_index=0;
	
	public Dictionary<string,GameObject> CharacterGraphics=new Dictionary<string, GameObject>();
	
	public bool map_turn=true,action_turn;
	
	public TileData[,] tiledata_map;
	public TileData CurrentTileData;
	
	void Awake(){
		CharacterGraphics.Add("Policeman",Resources.Load("PoliceGraphics") as GameObject);
		CharacterGraphics.Add("Junkie",Resources.Load("BumGraphics")as GameObject);
		CharacterGraphics.Add("CallGirl",Resources.Load("CallGirlGraphics")as GameObject);

		tiledata_map=new TileData[mapload.Maps[0].map_data.GetLength(0),mapload.Maps[0].map_data.GetLength(1)];
		
		for (int i=0;i<tiledata_map.GetLength(0);i++){
				
			for (int j=0;j<tiledata_map.GetLength(1);j++){
				tiledata_map[i,j]=new TileData();
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		map_turn=true;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void CalculateMovementsAll(){
		foreach(var c in Characters)
		{
			c.StarTempMovement();
		}
		while(true){
			bool chars_still_moving=false;
			foreach(var c in Characters)
			{
				if (c.TempMovement){
					var t=c.CurrentTempTile().Data;
					
					if (c.TempPos!=c.CurPos&&t.HasMultipleCharacters(c)){
						//stop_here
						c.EndPathToTempPos();
						c.EndTempMovement();
					}
					else{
						t.characters.Remove(c);
						
						c.MoveToNextTempPos();
						
						t=c.CurrentTempTile().Data;
						t.characters.Add(c);
						chars_still_moving= true;
					}
				}
			}
			
			if (!chars_still_moving)
				break;
			
			foreach(var c in Characters)
			{
				if (c.TempMovement){
					
					var t=c.CurrentTempTile();

					
					//whole path traversed
					if (c.TempPosIsLastPathPos()){
						c.EndTempMovement();
					}
				}
			}
		}
	}
	
	public void SetToPathEndAll(){
		foreach(var c in Characters)
		{
			c.SetToPathEnd();
		}
	}
	
	public void MoveAll(){
		foreach(var c in Characters)
		{
			c.Move();
		}
	}
	
	public bool AllMoved(){
		foreach(var c in Characters)
		{
			if (c.Main.Moving)
				return false;
		}
		return true;
	}

	public bool NextPlayersTurn ()
	{				
		if (current_player_index+1>Characters.Count){
			
			if (map_turn){
				CalculateMovementsAll();
				map_turn=false;
				action_turn=true;
			}
			else {
				SetToPathEndAll();
				
				map_turn=true;
				action_turn=false;
			}
			
			current_player_index=0;
			CurrentCharacter=Characters[current_player_index++];
			
			return false;
		}
		
		CurrentCharacter=Characters[current_player_index++];
		return true;
	}
}
