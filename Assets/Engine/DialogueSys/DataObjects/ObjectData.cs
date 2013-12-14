using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class ObjectData:EntityData{
    
    public string NameOrType {
        get {
            if (Name != "")
            {
                return Name;
            }
            return Type;
        }
    }
	
	public ObjectData(string type,string name):base("Object",type,name)
    {
    }
}
