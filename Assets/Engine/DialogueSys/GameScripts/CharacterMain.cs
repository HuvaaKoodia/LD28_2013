using UnityEngine;
using System.Collections;
using DialogueSystem;

public class CharacterMain : EntityMain {
	
	public GameObject graphics;
	public GameObject graphics_offset;
	
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update (){}
	
	public void SetCharacterData(CharacterData data){
		//DEV.TEMP
		Color c=Color.gray;
		switch(data.Name){
			case "Policeman": c=Color.blue;break;
			case "Junkie": c=Color.black;break;
			case "CallGirl": c=Color.red;break;
			case "Reporter": c=Color.green;break;
			case "Dealer": c=Color.magenta;break;
			case "Politician": c=Color.white;break;
		}
		graphics.renderer.material.color=c;
		
		Entity=data;
		
	}
}
