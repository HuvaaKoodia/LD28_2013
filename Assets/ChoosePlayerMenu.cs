using UnityEngine;
using System.Collections;

public class ChoosePlayerMenu : MonoBehaviour {

	
	GameDatabase GDB;
	void Start(){
		GDB=GameObject.FindGameObjectWithTag("Databases").GetComponent<GameDatabase>();
	}
	
	void On1P(){
		GDB.SetPlayers(1);
		Application.LoadLevel("MapGameScene");
	}
	
	void On2P(){
		GDB.SetPlayers(2);
		Application.LoadLevel("MapGameScene");
	}
	
	void On3P(){
		GDB.SetPlayers(3);
		Application.LoadLevel("MapGameScene");
	}
	
	void On4P(){
		GDB.SetPlayers(4);
		Application.LoadLevel("MapGameScene");
	}
	
	void On5P(){
		GDB.SetPlayers(5);
		Application.LoadLevel("MapGameScene");
	}
	
	void On6P(){
		GDB.SetPlayers(6);
		Application.LoadLevel("MapGameScene");
	}
}