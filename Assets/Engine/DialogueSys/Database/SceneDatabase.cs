using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DialogueSystem{

	public class SceneDatabase{
		public Dictionary<string,SceneData> Scenes{get;private set;}

       // public List<SceneDataLoadObj> load_objects = new List<SceneDataLoadObj>();

		public SceneDatabase(){
			Scenes=new Dictionary<string,SceneData>();
		}

        /*
        public void AddLoadObj(SceneDataLoadObj load_obj) {
            load_objects.Add(load_obj);
        }

        /// <summary>
        /// Parse ObjectDatabase before this.
        /// </summary>
        public void ParseLoadObjects(CoreDatabase core) {
            foreach (var lo in load_objects) {
                var data = new SceneData();

                //scene.Location=
                foreach (var o in lo.Objects) {
                    //data.Objects.Add(core.object_database.GetObject(o));
                }
                foreach (var o in lo.Characters)
                {
                   // data.Characters.Add(core.character_database.GetCharacter(o));
                }

                AddScene(lo.Name, data);
            }

            load_objects = null;
        }*/

		public void AddScene(string name,SceneData obj){
            if (Scenes.ContainsKey(name)) {
                Debug.LogError("Scene with the name: " + name + " already exists!");
                return;
            }
			Scenes.Add(name,obj);
		}
		
		public SceneData GetScene (string Key)
		{
			SceneData o;
			Scenes.TryGetValue(Key,out o);
			if (o==null){
				Debug.LogError("Requested Scene "+Key+" does not exist.");
				return null;
			}
			return o;
		}
	}
    /*
    public class SceneDataLoadObj{
        public string Name,Location;
        public List<string> Characters = new List<string>();
        public List<string> Objects=new List<string>();
    }*/
}