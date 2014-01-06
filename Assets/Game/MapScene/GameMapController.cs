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
	
	List<string> temp_names=new List<string>();
	
	bool allow_input=true,start_screen_on=true;
	
	void OnNextTurn(){
		LoadLevel(true);
	}
	
	void LoadLevel(bool next_turn){
		hudman.Turn_Report_Panel.ClosePanel();
		hudman.OnBackToMapPressedEvent-=OnNextTurn;
		controller.dial_man_1.OnAnswerButtonPressedEvent-=PressMapActionButton;
		
		if (next_turn)
			GDB.NextPlayersTurn();
		
		if (GDB.CurrentCharacter.AI)
			hudman.go_hud.SetText("AI",GDB.planning_turn);
		else{
			hudman.go_hud.SetText(GDB.CurrentCharacter.Name,GDB.planning_turn);
			hudman.PlayerHud_.SetPlayer(GDB.CurrentCharacter);
		}
		
		Application.LoadLevel(Application.loadedLevel);
	}
	
	// Use this for initialization
	void Start (){
		controller=GameObject.FindGameObjectWithTag("GameControllers").GetComponent<GameController>();
		hudman=GameObject.FindGameObjectWithTag("HudSystem").GetComponent<HudMain>();
		GDB=GameObject.FindGameObjectWithTag("Databases").GetComponent<GameDatabase>();
		
		hudman.PlayerHud_.ShowPanel(false);
		
		hudman.OnBackToMapPressedEvent+=OnNextTurn;
		hudman.ShowBackToMapButton(true);
		hudman.ClearActionDataPanels();
		
		controller.dial_man_1.StopDialogue();
		controller.dial_man_2.StopDialogue();
		
		hudman.Turn_Report_Panel.ClosePanel();
		
		//game state
		if (GDB.CurrentCharacter!=null){
			
			if (GDB.CurrentCharacter.MustInterract()){
				goto_action_scene=true;
			}
			
			if (GDB.CurrentCharacter.AI){
				return;
			}
			else{
				
				if (GDB.planning_turn){
					hudman.PlayerHud_.ShowPanel(true);
				}
				if (goto_action_scene){
					hudman.OnBackToMapPressedEvent-=OnNextTurn;
				}
				else{
					controller.dial_man_1.OnAnswerButtonPressedEvent+=PressMapActionButton;
					
	 				if (GDB.action_turn){
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
				
				hudman.go_hud.SetText(GDB.CurrentCharacter.Name,GDB.planning_turn);
				hudman.PlayerHud_.SetPlayer(GDB.CurrentCharacter);
			}
		}
		
		if (!goto_action_scene){
			//create objects
			MapMan.GenerateGrid(GDB,GameObject.FindGameObjectWithTag("Databases").GetComponent<MapLoader>());
			
			foreach(var data in GDB.Characters){
				if (data.Inactive) continue;
				
				var t=MapMan.GetTile(data.TurnStartTileData);
				var c=Instantiate(MapCharacterPrefab,t.transform.position,Quaternion.AngleAxis(90,Vector3.up)) as MapCharacter;
				c.mapman=MapMan;
				
				data.SetMain(c);
				data.Main.graphics_offset.gameObject.SetActive(false);
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
	//int temp_i=0;
	
	// Update is called once per frame
	void Update(){
		//gameplay input
		if (allow_input&&!start_screen_on&&Input.GetMouseButtonDown(0))
		{
			if (hudman.Turn_Report_Panel.TurnReportPanelOn){
				hudman.Turn_Report_Panel.ClosePanel();
			}
			else
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
					GDB.CurrentCharacter.CalculatePath(t.Data.TilePosition,GDB);
					
					foreach(var p in GDB.CurrentCharacter.Path_positions){
						PathDots.Add(Instantiate(PathObjPrefab,MapMan.tiles_map[(int)p.x,(int)p.y].transform.position,Quaternion.identity) as GameObject);
					}
					//update movement points
					//hudman.PlayerHud_.SetPlayer(GDB.CurrentCharacter);
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (start_screen_on){
				start_screen_on=false;
				hudman.go_hud.RemoveText();
				
				if (GDB.CurrentCharacter.AI){
					if (GDB.planning_turn){
						GDB.SimulateAIs();
						LoadLevel(false);
						return;
					}
					if (GDB.action_turn){
						GDB.JumpToNextPlayer();
						LoadLevel(false);
						return;
					}
				}
				
 				if (GDB.planning_turn){
					hudman.Turn_Report_Panel.SetStats(GDB.CurrentCharacter);
				}
				
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
				}
			}
		}
		
		//update		
		if (wait_for_moving_to_end&&move_characters_phase)
		{
			if (GDB.AllMoved()){
				move_characters_phase=false;
				wait_for_moving_to_end=false;
			}
		}
		
		//hide/show characters
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
		if (hudman.Turn_Report_Panel.TurnReportPanelOn){
			hudman.Turn_Report_Panel.ClosePanel();
		}
		
		if (button.Data.ToEvent=="OnMapWait"){
			ClearPathDots();
			GDB.CurrentCharacter.RemovePath();
		}
		if (button.Data.ToEvent=="OnMapMove"){
			create_path_mode=!create_path_mode;
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
