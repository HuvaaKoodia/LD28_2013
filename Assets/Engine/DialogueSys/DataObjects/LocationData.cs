using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class LocationData: EntityData{
	

    public List<CharacterData> Characters { get; private set; }
    public List<ObjectData> Objects { get; private set; }

    public LocationData()
    {
		
        Characters = new List<CharacterData>();
        Objects = new List<ObjectData>();
    }
}
