using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class CharacterData:EntityData{

    public InventoryData Inventory {get;private set;}

	public CharacterData(string type,string name):base("Character",type,name){
        Inventory = new InventoryData();
	}
	
	public void Print_character_facts(){
		Debug.LogWarning(""+Name+" facts:");
		foreach(var f in Facts.Facts){
			Debug.LogWarning(""+f.Key+": "+f.Value.Symbol);
		}
	}
	
}
