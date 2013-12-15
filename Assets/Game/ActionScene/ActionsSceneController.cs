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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnExit()
	{
		GDB.NextPlayersTurn();
		hud.OnBackToMapPressedEvent-=OnExit;
	}
}
