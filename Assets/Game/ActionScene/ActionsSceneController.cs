using UnityEngine;
using System.Collections;
using DialogueSystem;

public class ActionsSceneController : MonoBehaviour {

	GameController controller;
	public Transform CurrentCharacterPos;
	
	GameDatabase GDB;
	HudMain hud;
	
	// Use this for initialization
	void Start (){
		controller=GameObject.FindGameObjectWithTag("GameControllers").GetComponent<GameController>();
		
		controller.SceneMan.CurrentCharacterPos=CurrentCharacterPos;
		//controller.SceneMan.LoadScene();
		
		GDB=GameObject.FindGameObjectWithTag("Databases").GetComponent<GameDatabase>();
		
		controller.SceneMan.LoadScene(GDB.CurrentTileData,GDB.CurrentCharacter);
		
		hud=GameObject.FindGameObjectWithTag("HudSystem").GetComponent<HudMain>();
		hud.ShowBackToMapButton(true);
		
		hud.OnBackToMapPressedEvent+=OnExit;
		
		hud.ShowBackToMapButton(false);
		
		controller.dial_man.OnAnswerButtonPressedEvent+=OnAnswerButtonClick;
		
		//DEBUG_print_character_facts();
		
		if (GDB.CurrentCharacter.SelectedDialogueData!=null){
			
		}
	}
	
	GameCharacterData SelectedData;
	
	void OnAnswerButtonClick(AnswerButtonMain button){
		var action=new CharacterActionData(
		GDB.CurrentCharacter,
		SelectedData,
		button.Data.ToEvent,
		controller.dial_man.CurrentQuery
		);
		
		GDB.CurrentCharacter.CurrentAction=action;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
		
		hud.ShowBackToMapButton(true);
	}
	
	// Update is called once per frame
	void Update () {
		
		//input
		if (GDB.planning_turn){
			if (Input.GetMouseButtonDown(0)){
				Component com;
				
				if (Subs.GetObjectMousePos(out com,100,"Character"))
				{
					CharacterMain target=com.GetComponent<CharacterMain>();
					
					SelectedData=target.CharacterData;
					
					if (target.Entity!=GDB.CurrentCharacter.Data){
						controller.dial_man.CheckQuery(
						new QueryData(controller.SceneMan.Location_Data,controller.SceneMan.CurrentPlayer.Entity,
						target.Entity,"OnClick"));
					}
				}
			}
		}
		else{
			if (Input.GetKeyDown(KeyCode.Space)){
				
				if (current_action>GDB.CurrentTileData.ActionsThisTurn.Count-1)
				{
					//all actions done.
					hud.ShowBackToMapButton(true);
					
				}
				else{
					//next action
					bool create_panel=true;
					
					var action=GDB.CurrentTileData.ActionsThisTurn[current_action++];
					CurrentAction=action;
					
					if (CurrentAction.Character.IsStunned()){
						create_panel=false;
					}
					else{
						if (action.Interrupted){
							if (action.Character==GDB.CurrentCharacter){
								CurrentAction=new CharacterActionData(
									action.Character,
									action.Target,
									"OnInterrupt",
									action.Query
								);
							}
							else{
								//hide other people's interruptions. Current player doesn't know what they tried to do.
								create_panel=false;	
							}
					}
					}
					
					if (create_panel){
						var loc=new LocationData("ActionTexts");
						var q=new QueryData(loc,CurrentAction.Character.Data,CurrentAction.Target.Data,CurrentAction._Event);
						var r=controller.dial_man.core_database.rule_database.CheckQuery(q);
						
						var ActionTextData=new DialogueData("ERROR!!!1!");
						var ActionTextQuery=CurrentAction.Query;
						if (r!=null){
							ActionTextData=r.Data;
						}
						
						hud.AddActionDataTextPanel(ActionTextData,ActionTextQuery);
					}
				}
			}
		}
	}
	
	int current_action=0;
	CharacterActionData CurrentAction=null;
	
	//DEV.TEMP
	void DEBUG_print_character_facts(){
		GDB.CurrentCharacter.Data.Print_character_facts();
	}
	
	void OnExit()
	{
		hud.ClearActionDataPanels();
		//DEBUG_print_character_facts();
		
		controller.dial_man.StopDialogue();
		GDB. NextPlayersTurn();
		hud.OnBackToMapPressedEvent-=OnExit;
	}
}
