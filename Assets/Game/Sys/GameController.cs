using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameController : MonoBehaviour {
	
	public CameraMain Main_camera;
	
	public DialogueManager dial_man_1,dial_man_2;
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
		if (dial_man_1.DIALOGUE_ON) return;
		if (Input.GetKeyDown(KeyCode.Escape)){
			back_to_menu.ToggleMenu();
		}
		if (back_to_menu.IsOn()) return;

	}
}
