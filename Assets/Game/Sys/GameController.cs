using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameController : MonoBehaviour {
	
	public CameraMain Main_camera;
	
	public DialogueManager dial_man;
	public BackToMenuMain back_to_menu;

	public SceneManager SceneMan;

	bool gameover=false;
	
	public bool GAMEOVER{
		get{
			return gameover;
		}
		set{
			gameover=value;
			if (gameover){
				Main_camera.LOCK_INPUT=true;
			}
		}
	}

	FactContainer facts;
	
	// Use this for initialization
	void Start (){
		HudMain hud=GameObject.FindGameObjectWithTag("HudSystem").GetComponent<HudMain>();
		back_to_menu=hud.back_to_menu;
		back_to_menu.OnToggle+=OnToggle;

		//SceneMan.LoadScene();
	}
	
	void OnToggle(bool on){
		if (Main_camera!=null)
			Main_camera.LOCK_INPUT=on;
	}
	
	// Update is called once per frame
	void Update (){
		if (GAMEOVER){
			if (Input.GetKey(KeyCode.Return)){
				//go to main menu
				Application.LoadLevel("MainMenuScene");
			}
			return;
		}
		if (dial_man.DIALOGUE_ON) return;
		if (Input.GetKeyDown(KeyCode.Escape)){
			back_to_menu.ToggleMenu();
		}
		if (back_to_menu.IsOn()) return;

		//game

		if (Input.GetMouseButtonDown(0)){
			Component com;
			if (Subs.GetObjectMousePos(out com,100,"Character")){

				CharacterMain target=com.GetComponent<CharacterMain>();

				dial_man.CheckQuery(new QueryData(SceneMan.Location_Data,SceneMan.CurrentPlayer.Entity,target.Entity,"OnInteract"));
			}
		}
	}
	
	/*
	private bool GetMouseSoldier(out SoldierMain soldier){
		var ray=Camera.main.ScreenPointToRay(Input.mousePosition);
		int mask=1<<LayerMask.NameToLayer("Soldier");
		RaycastHit info;
		if (Physics.Raycast(ray,out info,500,mask)){
			soldier=info.collider.gameObject.GetComponent<SoldierMain>();
			return true;
		}
		soldier=null;
		return false;
	}*/
	
}
