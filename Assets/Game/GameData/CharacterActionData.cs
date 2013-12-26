using UnityEngine;
using System.Collections;
using DialogueSystem;

public class CharacterActionData{
	
	public GameCharacterData Character{get;private set;}
	public GameCharacterData Target{get;private set;}
	public string _Event {get;private set;}
	public QueryData Query {get;private set;}
	
	public bool Interrupted{get;private set;}
	public bool Stunned{get;set;}
	public bool IgnoreThis{get;set;}
	
	//public CharacterData Interrupter{get;private set;}
	
	public CharacterActionData(GameCharacterData character,GameCharacterData target,string _event,QueryData query){
		Character=character;
		Target=target;
		Query=query;
		_Event=_event;
		Interrupted=false;
	}
	
	public void Interrupt(GameCharacterData interrupter){
		Interrupted=true;
		Target=interrupter;
	}
	
}
