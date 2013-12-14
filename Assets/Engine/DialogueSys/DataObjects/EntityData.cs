using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class EntityData{
	public string Class{get;protected set;}
	public string Type{get;protected set;}
	public string Name{ get; protected set;}

	public FactContainer Facts{get;private set;}

	public EntityMain Entity=null;
   
	public EntityData(){
		Facts=new FactContainer();
	}

	public EntityData(string Class,string Type,string Name):this(){
		this.Class=Class;
		this.Type=Type;
		this.Name=Name;

		Facts.AddFact("Name",Name);
		Facts.AddFact("Type",Type);
		Facts.AddFact("Class",Class);
	}

}
