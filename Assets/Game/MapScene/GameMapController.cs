using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameMapController : MonoBehaviour {
	
	GameDatabase GDB;
	HudMain hudman;
	
	public MapCharacter MapCharacterPrefab;
	public MapManager MapMan;
	
	List<GameObject> PathLines=new List<GameObject>();
	public GameObject PathObjPrefab;
	
	bool create_path_mode,move_characters_phase=false,wait_for_moving_to_end=false,goto_action_scene=false;
	int player_text=0;
	
	
	List<string> temp_names=new List<string>();
	
	// Use this for initialization
	void Start (){
		hudman=GameObject.FindGameObjectWithTag("HudSystem").GetComponent<HudMain>();
		GDB=GameObject.FindGameObjectWithTag("Databases").GetComponent<GameDatabase>();
		
		MapMan.ml=GameObject.FindGameObjectWithTag("Databases").GetComponent<MapLoader>();
		MapMan.GenerateGrid();
		
		//game state
		if (GDB.CurrentCharacter!=null){
			
			if (GDB.CurrentCharacter.CurrentTile().Data.HasOtherCharactersNotMoving(GDB.CurrentCharacter)){
				goto_action_scene=true;
			}
			else{
				if (GDB.action_turn){
					//if (GDB.CurrentCharacter.OnTheMove){
					move_characters_phase=true;
				}
			}
			
			if (player_text==0){
				player_text=1;
				hudman.go_hud.SetText(GDB.CurrentCharacter.Name,GDB.planning_turn);	
			}
		}
		
		if (!goto_action_scene){
			//create objects
			
			for (int i=0;i<MapMan.gridX;i++){
					
				for (int j=0;j<MapMan.gridY;j++){
					MapMan.tiles_map[i,j].SetData(GDB.tiledata_map[i,j]);
				}
			}
			
			foreach(var data in GDB.Characters){
				data.mapman=MapMan;
				
				var t=data.CurrentTile();
				
				var c=Instantiate(MapCharacterPrefab,t.transform.position,Quaternion.AngleAxis(90,Vector3.up)) as MapCharacter;
				data.SetMain(c);
				//c.transform.position=t.TilePosition;
			}
			
			//temp
			foreach (var s in GDB.CharacterGraphics.Keys){
				temp_names.Add(s);
			}
		}
	}
	
	//temp list
	int temp_i=0;
	//string[] temp_chars=new string[]{"Policeman","Junkie"};
	
	bool start_input=true;
	
	// Update is called once per frame
	void Update(){
		//DEB.TEMP creating characters by hand
		if (start_input&&Input.GetMouseButtonDown(2)){

			Component comp;
			if(Subs.GetObjectMousePos(out comp,100,"Tile"))
		   	{			
				if (temp_names.Count>GDB.Characters.Count){
					Tile t = comp.transform.parent.GetComponent<Tile>();
					var c=Instantiate(MapCharacterPrefab,t.transform.position,Quaternion.AngleAxis(90,Vector3.up)) as MapCharacter;
					GameCharacterData data=new GameCharacterData("Player "+(temp_i+1));
					
					t.Data.characters.Add(data);
					
					data.mapman=MapMan;
					
					data.Data=GDB.Core.character_database.GetCharacterLazy(temp_names[temp_i++]);
					data.SetMain(c);
	  			
					data.CurPos=data.TempPos=t.TilePosition;
					GDB.Characters.Add(data);
				}
			}
		}
		//gameplay input
		
		if (Input.GetMouseButtonDown(0))
		{
			if (create_path_mode){
				Component comp;
				if(Subs.GetObjectMousePos(out comp,100,"Tile"))
			   	{
					Tile t = comp.transform.parent.GetComponent<Tile>();
					
					//select move pos for map character
					GDB.CurrentCharacter.SetMovePos(t.TilePosition);
					
					foreach(var p in GDB.CurrentCharacter.Path_positions){
						PathLines.Add(Instantiate(PathObjPrefab,MapMan.tiles_map[(int)p.x,(int)p.y].transform.position,Quaternion.identity) as GameObject);
					}
					
					create_path_mode=false;

					player_text=0;
				}
			}
		}
		
		if (wait_for_moving_to_end&&move_characters_phase)
		{
			if (GDB.AllMoved()){
				move_characters_phase=false;
				wait_for_moving_to_end=false;
				
				//check for interactions
//				if (GDB.CurrentCharacter.CurrentTempTile().Data.HasOtherCharacters(GDB.CurrentCharacter)){
//					GDB.CurrentTileData=GDB.CurrentCharacter.CurrentTempTile().Data;
//					must_interact=true;
//				}
			}
		}
		
		//dev temp
		if (Input.GetKeyDown(KeyCode.Space))
		{
			start_input=false;
			if (player_text==1){
				hudman.go_hud.RemoveText();
				player_text=2;
				
				if (goto_action_scene){
					Application.LoadLevel("ActionGameScene");	
				}
				else{
				
					if (move_characters_phase){
						GDB.MoveAll();
						wait_for_moving_to_end=true;
					}
					else
						create_path_mode=true;
				}
			}	
			else
			if (!move_characters_phase){
				//next character
				GDB.NextPlayersTurn();
				Application.LoadLevel(Application.loadedLevel);
			}
			
		}
	}
	void ClearPathDots()
	{
		while(PathLines.Count>0){
			var p=PathLines[0];
			Destroy(p);
			PathLines.RemoveAt(0);
		}
	}
	
}
