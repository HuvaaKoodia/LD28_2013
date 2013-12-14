using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class LocationMain :MonoBehaviour{
    
    public LocationData Location { get; private set; }
    public List<CharacterMain> Characters { get; private set; }
    public List<ObjectMain> Objects{ get; private set; }
	
	public LocationMain()
    {
		Characters = new List<CharacterMain>();
        Objects = new List<ObjectMain>();
    }

	public void SetLocation (LocationData locationData)
	{
		Location=locationData;
	}

	public void AddCharacter (CharacterMain c)
	{
		Characters.Add(c);
		Location.Characters.Add((CharacterData)c.Entity);
	}

	public void AddObject (ObjectMain o)
	{
		Objects.Add(o);
		Location.Objects.Add((ObjectData)o.Entity);
	}

	public void RemoveEntity (EntityData target)
	{
		foreach (var c in Characters){
			if (c.Entity==target){
				Location.Characters.Remove((CharacterData)c.Entity);
				Destroy(c);
				break;
			}
		}
		foreach (var c in Objects){
			if (c.Entity==target){
				Location.Objects.Remove((ObjectData)c.Entity);
				Destroy(c.gameObject);
				break;		
			}
		}
	}
}
