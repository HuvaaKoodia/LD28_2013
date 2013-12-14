using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{

	public class ObjectDatabase{
		public Dictionary<string,ObjectData> Objects{get;private set;}
		
		public ObjectDatabase(){
			Objects=new Dictionary<string,ObjectData>();
		}
		
		public void AddObject(string name,ObjectData obj){
            if (Objects.ContainsKey(name)) {
                Debug.LogError("Object with the name: " + name + " already exists!");
                return;
            }
			Objects.Add(name,obj);
		}
		
		public ObjectData GetObject (string Key)
		{
			ObjectData o;
			Objects.TryGetValue(Key,out o);
			if (o==null){
				Debug.LogError("Requested Object "+Key+" does not exist.");
				return null;
			}
			return o;
		}


        /// <summary>
        /// Parses an object command (e.g. "Add Object1 1 100")
        /// </summary>
        public static void ParseObjectCommand(string command,InventoryData Inventory, CoreDatabase core) {
            var spl = command.Split(' ');
            int amount = 1;

            int start_from=0,comm = 0;

            //get command
            if (spl[0].ToLower() == "add")
            {
                start_from = 1;
                comm = 0;
            }
            else if (spl[0].ToLower() == "remove")
            {
                start_from = 1;
                comm = 1;
            }

            string name = spl[start_from];

            if (spl.Length == start_from+1)
            {
                amount = 1;
            }
            else if (spl.Length == start_from + 2)
            {
                string number1 = spl[start_from + 1];
                amount = int.Parse(number1);
            }
            else if (spl.Length == start_from + 3)
            {
                string number1 = spl[start_from + 1], number2 = spl[start_from + 2];
                amount = Random.Range(int.Parse(number1), int.Parse(number2));
            }

            for (int i = 0; i < amount; i++)
            {
                if (comm==0)
                    Inventory.Add(core.object_database.GetObject(name));
                if (comm == 1)
                {
                    Inventory.Remove(name);
                }
            }
        }
	}
}