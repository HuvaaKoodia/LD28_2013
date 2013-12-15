using UnityEngine;
using System.Collections;

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
		
		DEBUG_print_character_facts();
	}
	
	void OnAnswerButtonClick(AnswerButtonMain button){
		
		GDB.CurrentCharacter.SelectedDialogueData=button.Data;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
		GDB.CurrentCharacter.SelectedQueryData=controller.dial_man.CurrentQuery;   
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void DEBUG_print_character_facts(){
		Debug.LogError(""+GDB.CurrentCharacter.Data.Name+" facts");
		foreach(var f in GDB.CurrentCharacter.Data.Facts.Facts){
			Debug.LogError(""+f.Key+": "+f.Value);
		}
	}
	
	void OnExit()
	{
		DEBUG_print_character_facts();
		
		controller.dial_man.StopDialogue();
		GDB.NextPlayersTurn();
		hud.OnBackToMapPressedEvent-=OnExit;
	}
}
