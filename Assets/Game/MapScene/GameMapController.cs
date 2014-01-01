using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameMapController : MonoBehaviour {
	
	GameController controller;
	GameDatabase GDB;
	HudMain hudman;
	
	public MapCharacter MapCharacterPrefab;
	public MapGenerator MapMan;
	
	List<GameObject> PathDots=new List<GameObject>();
	public GameObject PathObjPrefab;
	
	bool create_path_mode,move_characters_phase=false,wait_for_moving_to_end=false,goto_action_scene=false;
	int player_text=0;
	
	List<string> temp_names=new List<string>();
	
	bool allow_input=true,start_screen_on=true,temp_create_characters=true;
	
	void OnNextTurn(){
		hudman.OnBackToMapPressedEvent-=OnNextTurn;
		GDB.NextPlayersTurn();
		hudman.go_hud.SetText(GDB.CurrentCharacter.Name,GDB.planning_turn);
		hudman.PlayerHud_.SetPlayer(GDB.CurrentCharacter);
		
		controller.dial_man_1.OnAnswerButtonPressedEvent-=PressMapActionButton;
		Application.LoadLevel(Application.loadedLevel);
	}
	
	// Use this for initialization
	void Start (){
		controller=GameObject.FindGameObjectWithTag("GameControllers").GetComponent<GameController>();
		hudman=GameObject.FindGameObjectWithTag("HudSystem").GetComponent<HudMain>();
		GDB=GameObject.FindGameObjectWithTag("Databases").GetComponent<GameDatabase>();
		
		hudman.PlayerHud_.gameObject.SetActive(false);
		
		hudman.OnBackToMapPressedEvent+=OnNextTurn;
		hudman.ShowBackToMapButton(true);
		hudman.ClearActionDataPanels();
		
		controller.dial_man_1.StopDialogue();
		controller.dial_man_2.StopDialogue();
		
		//game state
		if (GDB.CurrentCharacter!=null){
			
			if (GDB.planning_turn){
				hudman.PlayerHud_.gameObject.SetActive(true);
			}
			if (!GDB.CurrentCharacter.OnMovingAwayFromTile&&GDB.CurrentCharacter.TurnStartTile().Data.HasOtherCharactersNotMoving(GDB.CurrentCharacter)){
				goto_action_scene=true;
				hudman.OnBackToMapPressedEvent-=OnNextTurn;
			}
			else{
				controller.dial_man_1.OnAnswerButtonPressedEvent+=PressMapActionButton;
				
 				if (GDB.action_turn){
					//if (GDB.CurrentCharacter.OnTheMove){
					move_characters_phase=true;
				}
				
				if (GDB.CurrentCharacter.IsArrested()){
					hudman.AddActionDataTextPanel("You are in jail.\n"+GDB.CurrentCharacter.ArrestedTurns+" turns left.");
					allow_input=false;
				}
				else if (GDB.CurrentCharacter.IsStunned()){
					hudman.AddActionDataTextPanel("You are stunned for this turn.");
					allow_input=false;
				}
			}
			
			if (player_text==0){
				player_text=1;
				hudman.go_hud.SetText(GDB.CurrentCharacter.Name,GDB.planning_turn);
				hudman.PlayerHud_.SetPlayer(GDB.CurrentCharacter);
			}
		}
		
		if (!goto_action_scene){
			//create objects
			MapMan.GenerateGrid(GDB,GameObject.FindGameObjectWithTag("Databases").GetComponent<MapLoader>());
			
			foreach(var data in GDB.Characters){
				data.mapman=MapMan;//DEV.todo. Get rid of map man references. -> use TileData objects instead
				if (data.Inactive) continue;
				
				var t=data.TurnStartTile();
				var c=Instantiate(MapCharacterPrefab,t.transform.position,Quaternion.AngleAxis(90,Vector3.up)) as MapCharacter;
				data.SetMain(c);
				
				data.Main.graphics_offset.gameObject.SetActive(false);
			}
			
			//temp
			foreach (var s in GDB.CharacterGraphics.Keys){
				temp_names.Add(s);
			}
			
			//map actions
			if (GDB.planning_turn){
				if (allow_input&&GDB.CurrentCharacter!=null){
					controller.dial_man_1.CheckQuery(
						new QueryData(GDB.CurrentTileData.Location,GDB.CurrentCharacter.Data,
						GDB.CurrentCharacter.Data,"OnMapActions")
					);
				}
			}
		}
	}
	
	//temp list
	int temp_i=0;
	
	// Update is called once per frame
	void Update(){
		//DEB.TEMP creating characters by hand
		if (temp_create_characters&&Input.GetMouseButtonDown(2)){

			Component comp;
			if(Subs.GetObjectMousePos(out comp,100,"Tile"))
		   	{			
				if (temp_names.Count>GDB.Characters.Count){
					Tile t = comp.transform.parent.GetComponent<Tile>();
					var c=Instantiate(MapCharacterPrefab,t.transform.position,Quaternion.AngleAxis(90,Vector3.up)) as MapCharacter;
					GameCharacterData data=new GameCharacterData("Player "+(temp_i+1));
					
					data.mapman=MapMan;
					
					data.Data=GDB.Core.character_database.GetCharacterLazy(temp_names[temp_i++]);
					data.SetMain(c);
	  			
					data.SetStartPosition(t.TilePosition);
					
					GDB.Characters.Add(data);
					t.Data.AddCharacter(data);
				}
			}
		}
		//gameplay input
		
		if (allow_input&&!start_screen_on&&Input.GetMouseButtonDown(0))
		{
			if (create_path_mode){
				Component comp;
				if(Subs.GetObjectMousePos(out comp,100,"Tile"))
			   	{
					//remove path dots
					while(PathDots.Count>0){
	              		var p=PathDots[0];
		              	Destroy(p);
		              	PathDots.RemoveAt(0);
		            }
					
					Tile t =comp.transform.parent.GetComponent<Tile>();
					
					//select move pos for map character
					GDB.CurrentCharacter.CalculatePath(t.TilePosition);
					
					foreach(var p in GDB.CurrentCharacter.Path_positions){
						PathDots.Add(Instantiate(PathObjPrefab,MapMan.tiles_map[(int)p.x,(int)p.y].transform.position,Quaternion.identity) as GameObject);
					}
				}
			}
		}
		
		if (wait_for_moving_to_end&&move_characters_phase)
		{
			if (GDB.AllMoved()){
				move_characters_phase=false;
				wait_for_moving_to_end=false;
			}
		}

		//dev temp
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (player_text==1){
				temp_create_characters=false;
				start_screen_on=false;
				hudman.go_hud.RemoveText();
				player_text=2;
				
				if (goto_action_scene){
					Application.LoadLevel("ActionGameScene");	
				}
				else{
					if (allow_input)
						hudman.ShowBackToMapButton(true);
					
					if (move_characters_phase){
						GDB.MoveAll();
						wait_for_moving_to_end=true;
					}
					else{
						create_path_mode=true;
					}
				}
			}
		}
		
				
		if (!start_screen_on&&!goto_action_scene&&GDB.CurrentCharacter!=null){
			//current character view range
			foreach(var data in GDB.Characters){
				if (data.Inactive||GDB.CurrentCharacter.Inactive) continue;
				bool show_c=false;
				
				//in view range
				if (Vector3.Distance(GDB.CurrentCharacter.Main.transform.position,data.Main.transform.position)<
					data.Data.Facts.GetFloat("ViewRange")
					)show_c=true;
				
				//target hiding or current char running
				if (data.CurrentMovementType==CharacterMovementType.Hiding
					||GDB.CurrentCharacter.CurrentMovementType==CharacterMovementType.Running
					) show_c=false;
				
				//current char -> always show
				if (data==GDB.CurrentCharacter) show_c=true;
				
				data.Main.graphics_offset.gameObject.SetActive(show_c);
			}
		}
	}
	void ClearPathDots()
	{
		while(PathDots.Count>0){
			var p=PathDots[0];
			Destroy(p);
			PathDots.RemoveAt(0);
		}
	}
	
	void PressMapActionButton(AnswerButtonMain button){
		if (button.Data.ToEvent=="OnMapWait"){
			ClearPathDots();
			GDB.CurrentCharacter.RemovePath();
		}
		if (button.Data.ToEvent=="OnMapMove"){
			
		}
		
		if (button.Data.ToEvent=="OnMapUseVehicle"){
			GDB.CurrentCharacter.ToggleRunning();
		}
		
		if (button.Data.ToEvent=="OnMapHide"){
			GDB.CurrentCharacter.ToggleHiding();
		}
		hudman.PlayerHud_.SetPlayer(GDB.CurrentCharacter);
	}
	
}
