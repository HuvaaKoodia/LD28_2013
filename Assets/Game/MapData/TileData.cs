using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileData
{
	public List<MapCharacterData> characters=new List<MapCharacterData>();
	
	public Vector2 TilePosition;
	
	void AddCharacter(){
		
	}

	public string LocationName="Street";

	public bool HasMultipleCharacters (MapCharacterData character)
	{
		int amount=0;
		foreach (var c in characters){
			if (c==character) continue;
			//Dev.stealth check 
			amount++;
		}
		return amount>0;
	}

	
	
}
