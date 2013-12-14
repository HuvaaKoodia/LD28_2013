using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class CharacterData:EntityData{

    public InventoryData Inventory {get;private set;}

	public CharacterData(string type,string name):base("Character",type,name){
        Inventory = new InventoryData();
	}

}
