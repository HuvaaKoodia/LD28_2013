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
		if (GameCharacters.Contains(c)){
			Debug.LogWarning("Tile already has the character "+c.Name);
			return;
		}
		
		GameCharacters.Add(c);
		Location.Characters.Add(c.Data);
	}
	
	public void RemoveCharacter (GameCharacterData c)
	{
		GameCharacters.Remove(c);
		Location.Characters.Remove(c.Data);
	}
	
	
	public bool HasOtherCharacters (GameCharacterData character)
	{
		int amount=0;
		foreach (var c in GameCharacters){
			if (c==character) continue;
			//Dev.stealth check
			if (!c.IsHiding)
				amount++;
		}
		return amount>0;
	}

	public bool HasCharacter (GameCharacterData data)
	{
		return GameCharacters.Contains(data);
	}
	
	public bool HasOtherCharactersNotMoving (GameCharacterData character)
	{
		int amount=0;
		foreach (var c in GameCharacters){
			if (c==character) continue;
			//Dev.stealth check 
			if (!c.OnMovingAwayFromTile&&c.CurrentPosIsTurnStartPos()&&!c.IsHiding)
				amount++;
		}
		return amount>0;
	}

	public void SetLocation (LocationData data)
	{
		Location=data;
	}
}
