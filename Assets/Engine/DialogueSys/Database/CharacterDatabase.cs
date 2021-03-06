﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{

	public class CharacterDatabase{
		public Dictionary<string,CharacterData> Characters{get;private set;}

        public Dictionary<string, CharacterDataLoadObj> LoadObjects { get; private set; }

        CoreDatabase _core;

		public CharacterDatabase(CoreDatabase core){
            _core = core;
			Characters=new Dictionary<string,CharacterData>();
            LoadObjects = new Dictionary<string, CharacterDataLoadObj>();
		}

        public void AddLoadObj(string name, CharacterDataLoadObj load_obj)
        {
			if (LoadObjects.ContainsKey(name)){
				Debug.LogError("Character "+name+" already added once!");
			}
            LoadObjects.Add(name,load_obj);
        }

        /// <summary>
        /// Parse ObjectDatabase before this.
        /// </summary>
        public void ParseLoadObjects(CoreDatabase core) {
            foreach (var lo in LoadObjects) {
                var data = ParseLoadObject(lo.Key);

                AddCharacter(lo.Key, data);
            }
        }

        /// <summary>
        /// Parse ObjectDatabase before this.
        /// </summary>
        public CharacterData ParseLoadObject(string name)
        {
			if (!LoadObjects.ContainsKey(name))
			{
				return null;
			}
            var obj = LoadObjects[name];
			var data = new CharacterData(obj.Type,obj.Name);
			
			//incorporate base
			if (obj.Base!="")
			{
				if (!LoadObjects.ContainsKey(obj.Base))
				{
					Debug.LogError("Base object: "+obj.Base+" not found when loading object: "+obj.Name);
				}
				else{
					var bas=LoadObjects[obj.Base];

					foreach(var f in bas.Facts.Facts){
						data.Facts.AddFact(f.Key,f.Value,true);
					}

					foreach (var o in bas.Objects)
		            {
		                ObjectDatabase.ParseObjectCommand(o, data.Inventory, _core);
		            }
				}
			}

            foreach (var f in obj.Facts.Facts){
                
                data.Facts.AddOrSetFact(f.Key,f.Value,true);
            }

            foreach (var o in obj.Objects)
            {
                ObjectDatabase.ParseObjectCommand(o, data.Inventory, _core);
            }
			
            return data;
        }

		public void AddCharacter(string name,CharacterData obj){
            if (Characters.ContainsKey(name)) {
                Debug.LogError("Character with the name: " + name + " already exists!");
                return;
            }
			Characters.Add(name,obj);
		}
		
		public CharacterData GetCharacter(string Key)
		{
			CharacterData o;
			Characters.TryGetValue(Key,out o);
			if (o==null){
				Debug.LogError("Requested Character "+Key+" does not exist.");
				return null;
			}
			return o;
		}

        public CharacterData GetCharacterLazy(string Key)
        {
            CharacterData o = ParseLoadObject(Key);
            if (o == null)
            {
                Debug.LogError("Requested Character " + Key + " does not exist. (Lazy eval.)");
                return null;
            }
            return o;
        }
	}

    public class CharacterDataLoadObj{
		public string Type,Name,Base="";

        public FactContainer Facts=new FactContainer();
        public List<string> Objects=new List<string>();
    }
}