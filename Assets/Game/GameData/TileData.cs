using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileData
{
	public List<GameCharacterData> characters=new List<GameCharacterData>();
	
	public Vector2 TilePosition;
	
	public List<CharacterActionData> ActionsThisTurn=new List<CharacterActionData>();

	public string LocationName="Street";//DEV.todo Imp. LocationData in tileData

	public bool HasOtherCharacters (GameCharacterData character)
	{
		int amount=0;
		foreach (var c in characters){
			if (c==character) continue;
			//Dev.stealth check 
			amount++;
		}
		return amount>0;
	}
	
	public bool HasOtherCharactersNotMoving (GameCharacterData character)
	{
		int amount=0;
		foreach (var c in characters){
			if (c==character) continue;
			//Dev.stealth check 
			if (!c.OnTheMove)
				amount++;
		}
		return amount>0;
	}

	
	
}
