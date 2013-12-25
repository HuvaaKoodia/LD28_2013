using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileData
{
	public LocationData Location{get;private set;}
	public Vector2 TilePosition;
	
	public List<GameCharacterData> GameCharacters{get;private set;}
	public List<CharacterActionData> ActionsThisTurn{get;private set;}
	
	public TileData(){
		GameCharacters=new List<GameCharacterData>();
		ActionsThisTurn=new List<CharacterActionData>();
	}
	
	public void AddCharacter (GameCharacterData c)
	{
		GameCharacters.Add(c);
	}
	
	public void RemoveCharacter (GameCharacterData c)
	{
		GameCharacters.Remove(c);
	}
	
	
	public bool HasOtherCharacters (GameCharacterData character)
	{
		int amount=0;
		foreach (var c in GameCharacters){
			if (c==character) continue;
			//Dev.stealth check
			amount++;
		}
		return amount>0;
	}
	
	public bool HasOtherCharactersNotMoving (GameCharacterData character)
	{
		int amount=0;
		foreach (var c in GameCharacters){
			if (c==character) continue;
			//Dev.stealth check 
			if (!c.OnMovingAwayFromTile&&c.CurrentPosIsTurnStartPos())
				amount++;
		}
		return amount>0;
	}

	public void SetLocation (LocationData data)
	{
		Location=data;
	}
}
