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
		
		
		
		controller.dial_man.OnAnswerButtonPressedEvent+=OnAnswerButtonClick;
		
		//DEBUG_print_character_facts();
		
		
		if (GDB.CurrentCharacter.SelectedDialogueData!=null){
			
		}
	}
	
	void OnAnswerButtonClick(AnswerButtonMain button){
		var action=new CharacterActionData();
		action.Character=GDB.CurrentCharacter;
		action.Dialogue=button.Data;
		action._Event=button.Data.ToEvent;
		action.Query=controller.dial_man.CurrentQuery;
		
		GDB.CurrentCharacter.CurrentAction=action;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
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
					controller.dial_man.CheckQuery(new QueryData(controller.SceneMan.Location_Data,controller.SceneMan.CurrentPlayer.Entity,target.Entity,"OnInteract"));
				}
			}
		}
		else{
			if (Input.GetKeyDown(KeyCode.Space)){
				
				if (current_action>GDB.CurrentTileData.ActionsThisTurn.Count-1)
				{
					//all actions done.
					
				}
				else{
					//forward actions
					CurrentAction=GDB.CurrentTileData.ActionsThisTurn[current_action++];
					hud.AddActionDataTextPanel(CurrentAction);
				}
			}
		}
	}
	
	int current_action=0;
	CharacterActionData CurrentAction=null;
	
	void DEBUG_print_character_facts(){
		Debug.LogError(""+GDB.CurrentCharacter.Data.Name+" facts");
		foreach(var f in GDB.CurrentCharacter.Data.Facts.Facts){
			Debug.LogError(""+f.Key+": "+f.Value.Symbol);
		}
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
