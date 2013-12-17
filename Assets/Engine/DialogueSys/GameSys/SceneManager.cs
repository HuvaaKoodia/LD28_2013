using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class SceneManager : MonoBehaviour {
	
	public GameObject CharacterPrefab; 
	
	public CoreDatabase Core;
	public LocationMain Location;
	public LocationData Location_Data{get{return Location.Location;}}

	public CharacterMain CurrentPlayer;
	
	public Transform CurrentCharacterPos;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	public void LoadScene(TileData data,GameCharacterData currentCharacter){
		//dev.TODO have a real location for each tiledata.
		Location.SetLocation(Core.location_database.GetLocation(data.LocationName));
		
		var go=Instantiate(CharacterPrefab,CurrentCharacterPos.position,Quaternion.AngleAxis(90,Vector3.up)) as GameObject;
		CurrentPlayer=go.GetComponent<CharacterMain>();
		
		CurrentPlayer.SetCharacterData(currentCharacter.Data);
		
		var add_to_pos=Vector3.right*1;
		
		foreach (var c in data.characters){
			if (c==currentCharacter||c.OnTheMove) continue;
			
			go=Instantiate(CharacterPrefab,CurrentCharacterPos.position+add_to_pos,Quaternion.AngleAxis(270,Vector3.up)) as GameObject;
			var cm=go.GetComponent<CharacterMain>();
			
			cm.SetCharacterData(c);
			Location.AddCharacter(cm);
			
			add_to_pos+=Vector3.right*1;
		}
//	
//		foreach(var f in Object1.Character.Facts.Facts){
//			Debug.Log(f.Key+" "+f.Value.Value);
//		}

		Location.AddCharacter(CurrentPlayer);
		
	}
	
	public void LoadScene(){
		
		Location.SetLocation(Core.location_database.GetLocation("Street"));
		
		string current_player=Location.Location.Facts.GetString("CurrentPlayer");
		
		var go=Instantiate(CharacterPrefab,CurrentCharacterPos.position,Quaternion.identity) as GameObject;
		CurrentPlayer=go.GetComponent<CharacterMain>();
		
		CurrentPlayer.SetCharacterData(Core.character_database.GetCharacterLazy(current_player));
		
		
		var add_to_pos=Vector3.right*0.1f;
		
		foreach (var c in Core.character_database.LoadObjects){
			if (c.Key==current_player) continue;
			
			go=Instantiate(CharacterPrefab,CurrentCharacterPos.position+add_to_pos,Quaternion.identity) as GameObject;
			var cm=go.GetComponent<CharacterMain>();
			
			cm.SetCharacterData(Core.character_database.GetCharacterLazy(c.Key));
			Location.AddCharacter(cm);
			
			add_to_pos+=Vector3.right*0.8f;
		}
//	
//		foreach(var f in Object1.Character.Facts.Facts){
//			Debug.Log(f.Key+" "+f.Value.Value);
//		}

		Location.AddCharacter(CurrentPlayer);
		
	}


}
