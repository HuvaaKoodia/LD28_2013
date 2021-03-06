﻿using UnityEngine;
using System.Collections;
using DialogueSystem;

public class CharacterActionData{
	
	public GameCharacterData Character{get;private set;}
	public GameCharacterData Target{get;private set;}
	public string _Event {get;private set;}
	public QueryData Query {get;private set;}
	
	public bool Interrupted{get;private set;}
	public bool ShowOnlyForCurrentCharacter{get;set;}
	public bool IgnoreThis{get;set;}

	
	//public CharacterData Interrupter{get;private set;}
	
	public CharacterActionData(GameCharacterData character,GameCharacterData target,string _event,QueryData query){
		Character=character;
		Target=target;
		Query=query;
		_Event=_event;
		Interrupted=false;
	}
	
	public CharacterActionData(GameCharacterData character,GameCharacterData target,string _event,LocationData location)
	:this(character,target,_event,new QueryData(location,character.Data,target.Data,_event))
	{}
	
	public void Interrupt(GameCharacterData interrupter){
		Interrupted=true;
		Target=interrupter;
	}
}
