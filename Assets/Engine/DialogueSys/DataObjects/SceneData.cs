using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;


public class SceneData{
    /*
    public LocationData Location { get; private set; }
    public List<CharacterData> Characters { get; private set; }
    public List<ObjectData> Objects{ get; private set; }
    */

    public string Location { get; set; }
    public List<string> Characters { get; private set; }
    public List<string> Objects { get; private set; }
    
    public SceneData()
    {
        Characters = new List<string>();
        Objects = new List<string>();
    }
}
