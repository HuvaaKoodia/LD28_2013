using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameMapController : MonoBehaviour {
	
	
	public CoreDatabase Core;
	public MapCharacter MapCharacterPrefab;
	public MapManager MapMan;
	
	int current_player_index=0;
	
	MapCharacterData CurrentCharacter;
	List<MapCharacterData> Characters=new List<MapCharacterData>();
	
	bool move_turn;
	
	// Use this for initialization
	void Start (){
		
	}
	
	//temp list
	int temp_i=0;
	string[] temp_chars=new string[]{"Policeman","Junkie"};
	
	// Update is called once per frame
	void Update(){
		if (Input.GetMouseButtonDown(2)){

			Component comp;
			if(Subs.GetObjectMousePos(out comp,100,"Tile"))
		   	{			
				Tile t = comp.transform.parent.GetComponent<Tile>();
				var c=Instantiate(MapCharacterPrefab,t.transform.position,Quaternion.identity) as MapCharacter;
				MapCharacterData data=new MapCharacterData();
				data.Main=c;
				data.Data=Core.character_database.GetCharacterLazy(temp_chars[temp_i++]);
				data.mapman=MapMan;
				data.CurPos=t.TilePosition;
				Characters.Add(data);
		   	}
		}
		
		if (Input.GetMouseButtonDown(0))
		{
			if (move_turn){
				
				Component comp;
				if(Subs.GetObjectMousePos(out comp,100,"Tile"))
			   	{			
					Tile t = comp.transform.parent.GetComponent<Tile>();
					
					//select move pos for map character
					CurrentCharacter.MovePos=t.TilePosition;
					move_turn=false;
				}
			}
		}
		
		//dev temp
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (!move_turn){
				if (current_player_index>Characters.Count){
					//move turn
					Debug.Log("Move turn");
					current_player_index=0;
				}
				else{
					move_turn=true;
					//start turn
					Debug.Log("Player "+(current_player_index+1) +"turn ");	
					
					CurrentCharacter=Characters[current_player_index];
				}
				
			}
		}
	}
}
