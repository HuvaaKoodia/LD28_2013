using UnityEngine;
using System.Collections;
using DialogueSystem;

public class CharacterMain : EntityMain {
	
	public GameObject graphics;
	public GameObject graphics_offset;
	
	public GameCharacterData CharacterData{get;private set;}
	
	// Use this for initialization
	void Start (){
	   Functions=new CharacterFunctions();
	}
	
	// Update is called once per frame
	void Update (){}
	
	public void SetCharacterData(GameCharacterData Data){
		CharacterData=Data;
		SetCharacterData(Data.Data);
	}
	
	public void SetCharacterData(CharacterData data){
		//DEV.TEMP
		
		var gdb=GameObject.FindGameObjectWithTag("Databases").GetComponent<GameDatabase>();
		GameObject go=null;
		if (gdb.CharacterGraphics.ContainsKey(data.Name)){
			go=gdb.CharacterGraphics[data.Name];
		}
		
		if (go==null){
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
		}
		else{
	 		graphics_offset.SetActive(false);
			
			graphics_offset=Instantiate(go,transform.position,Quaternion.identity) as GameObject;
			graphics_offset.transform.parent=transform;
			graphics_offset.transform.localPosition=Vector3.zero;
			graphics_offset.transform.localRotation=transform.rotation;
		}
		
		
		

		Entity=data;
		data.Entity=this;
		
	}
}
