using UnityEngine;
using System.Collections;
using DialogueSystem;

public class ActionsSceneController : MonoBehaviour {

	GameController controller;
	public Transform CurrentCharacterPos;
	
	GameDatabase GDB;
	HudMain hud;
	
	GameCharacterData InterractTargetData;
	
	bool allow_input;
	
	// Use this for initialization
	void Start (){
		allow_input=true;
		
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
		if (GDB.planning_turn){
			if (GDB.CurrentCharacter.IsStunned()){
				
				hud.AddActionDataTextPanel("You are stunned for this turn.");
				allow_input=false;
				
				hud.ShowBackToMapButton(true);
			}
		}
	}
	
	
	
	void OnAnswerButtonClick(AnswerButtonMain button){
		var action=new CharacterActionData(
			GDB.CurrentCharacter,
			InterractTargetData,
			button.Data.ToEvent,
			controller.dial_man.CurrentQuery
		);
		
		GDB.CurrentCharacter.CurrentAction=action;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
		
		hud.ShowBackToMapButton(true);
	}
	
	// Update is called once per frame
	void Update () {
		
		//input
		if (GDB.planning_turn&&allow_input){
			if (Input.GetMouseButtonDown(0)){
				Component com;
				
				if (Subs.GetObjectMousePos(out com,100,"Character"))
				{
					CharacterMain target=com.GetComponent<CharacterMain>();
					
					if (target.Entity!=GDB.CurrentCharacter.Data){
						InterractTargetData=target.CharacterData;
						controller.dial_man.CheckQuery(
						new QueryData(controller.SceneMan.Location_Data,controller.SceneMan.CurrentPlayer.Entity,
						target.Entity,"OnClick"));
					}
				}
			}
		}
		else{
			if (Input.GetKeyDown(KeyCode.Space)){
				
				while (true){
					if (current_action>GDB.CurrentTileData.ActionsThisTurn.Count-1)
					{
						//all actions done.
						hud.ShowBackToMapButton(true);
						break;
						
					}
					else{
						//next action
						var action=GDB.CurrentTileData.ActionsThisTurn[current_action++];
						CurrentAction=action;
					
						if (action.Interrupted){
							CurrentAction=new CharacterActionData(
								action.Character,
								action.Target,
								"OnInterrupt",
								action.Query
							);
						
							if (action.Character!=GDB.CurrentCharacter) continue;
						}
						else if (action.Stunned){
							CurrentAction=new CharacterActionData(
								action.Character,
								action.Target,
								"OnStun",
								action.Query
							);
							if (action.Character!=GDB.CurrentCharacter) continue;
						}
						
						//add panel
						var loc=new LocationData("ActionTexts");
						var q=new QueryData(loc,CurrentAction.Character.Data,CurrentAction.Target.Data,CurrentAction._Event);
						var r=controller.dial_man.core_database.rule_database.CheckQuery(q);
						
						var ActionTextData=new DialogueData("ERROR!!!1!");
						var ActionTextQuery=CurrentAction.Query;
						if (r!=null){
							ActionTextData=r.Data;
						}
						
						hud.AddActionDataTextPanel(ActionTextData,ActionTextQuery);
						break;
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
		controller.dial_man.OnAnswerButtonPressedEvent-=OnAnswerButtonClick;
	}
}
