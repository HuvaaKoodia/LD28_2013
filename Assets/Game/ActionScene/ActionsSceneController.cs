using UnityEngine;
using System.Collections;
using DialogueSystem;

public class ActionsSceneController : MonoBehaviour {

	GameController controller;
	public Transform CurrentCharacterPos;

	GameDatabase GDB;
	HudMain hud;

	GameCharacterData InterractTargetData;

	public GameObject StunEffect;

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
		hud.OnContinuePressedEvent+=NextMessage;

		hud.ShowBackToMapButton(false);
		hud.ShowContinueButton(false);

		controller.dial_man_1.OnAnswerButtonPressedEvent+=OnAnswerButtonClick;
		//controller.dial_man_2.OnAnswerButtonPressedEvent+=OnAnswerButtonClick;

		//stun effects for all
		foreach (var c in GDB.CurrentTileData.GameCharacters){
			if (c.OnMovingAwayFromTile||c.Inactive||c.IsHiding) continue;
			
			if (c.IsStunned()){	
				Instantiate(StunEffect,c.ActionMain.transform.position+Vector3.up*0.8f,Quaternion.identity);
			}
		}

		if (GDB.CurrentCharacter.IsStunned()){
			hud.AddActionDataTextPanel("You are stunned for this turn.");
			allow_input=false;

			hud.ShowBackToMapButton(true);
		}

		if (GDB.planning_turn){
			if (!GDB.CurrentCharacter.IsStunned()){
				controller.dial_man_2.CheckQuery(
					new QueryData(GDB.CurrentTileData.Location,GDB.CurrentCharacter.Data,
					GDB.CurrentCharacter.Data,"OnClickBasic")
				);
			}
		}
		else{
			hud.ShowContinueButton(true);
		}
	}

	void OnAnswerButtonClick(AnswerButtonMain button){
		GameCharacterData target=InterractTargetData;
		if (target==null)
			target=GDB.CurrentCharacter;
		var action=new CharacterActionData(
			GDB.CurrentCharacter,
			target,
			button.Data.ToEvent,
			GDB.CurrentTileData.Location
			);

		foreach (var c in GDB.CurrentTileData.Location.Characters){
			Debug.Log("name: "+c.Name);
		}

		GDB.CurrentCharacter.CurrentAction=action;
		hud.ShowBackToMapButton(true);
	}

	// Update is called once per frame
	void Update () {
		if (allow_input){
			//input
			if (GDB.planning_turn){
				if (Input.GetMouseButtonDown(0)){
					Component com;

					if (Subs.GetObjectMousePos(out com,100,"Character"))
					{
						CharacterMain target=com.GetComponent<CharacterMain>();

						if (target.Entity!=GDB.CurrentCharacter.Data){
							InterractTargetData=target.CharacterData;

							controller.dial_man_1.CheckQuery(
							new QueryData(GDB.CurrentTileData.Location,GDB.CurrentCharacter.Data,target.Entity,"OnClick"));
						}
					}
				}
			}
			else if (GDB.action_turn){
				if (Input.GetKeyDown(KeyCode.Space)){
					
				}
			}
		}
	}
	
	void NextMessage(){
		while (true){
			if (current_action>GDB.CurrentTileData.ActionsThisTurn.Count-1)
			{
				//all actions done.
				hud.ShowBackToMapButton(true);
				hud.ShowContinueButton(false);
				break;
			}
			else{
				//next action
				var action=GDB.CurrentTileData.ActionsThisTurn[current_action++];
				QueryData action_q=new QueryData(new LocationData("ActionTexts"),action.Character.Data,action.Target.Data,action._Event);

				if (action.Character.IsHiding||action.IgnoreThis){
					continue;
				}
				else if (action.Interrupted){
					if (action.Character!=GDB.CurrentCharacter) continue;//don't show interrupted message for other players.

					action_q=new QueryData(new LocationData("ActionTexts"),action.Character.Data,action.Target.Data,"OnInterrupt");
				}
				else if (action.ShowOnlyForCurrentCharacter){
					if (action.Character!=GDB.CurrentCharacter) continue;//don't show message for other players.
				}

				//add panel

				var r=controller.dial_man_1.core_database.rule_database.CheckQuery(action_q);

				var ActionTextData=new DialogueData("ERROR!!!1!");
				if (r!=null){
					ActionTextData=r.Link;
				}

				hud.AddActionDataTextPanel(ActionTextData,action_q);
				break;
			}
		}
	}
		
	int current_action=0;

	void OnExit()
	{
		hud.OnBackToMapPressedEvent-=OnExit;
		hud.OnContinuePressedEvent-=NextMessage;
		controller.dial_man_1.OnAnswerButtonPressedEvent-=OnAnswerButtonClick;
		controller.dial_man_2.OnAnswerButtonPressedEvent-=OnAnswerButtonClick;
		
		hud.ShowBackToMapButton(false);
		hud.ShowContinueButton(false);
		
		hud.ClearActionDataPanels();
		
		controller.dial_man_1.StopDialogue();
		controller.dial_man_2.StopDialogue();
		GDB.NextPlayersTurn();
				
		Application.LoadLevel("MapGameScene");
	}
}
