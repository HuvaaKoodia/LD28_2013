using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DialogueSystem{

	public class LocationDatabase{
		public Dictionary<string,LocationData> Locations{get;private set;}
		
		public LocationDatabase(){
			Locations=new Dictionary<string,LocationData>();
		}

        public void AddLocation(string name, LocationData obj)
        {
            if (Locations.ContainsKey(name))
            {
                Debug.LogError("Location with the name: " + name + " already exists!");
                return;
            }
            Locations.Add(name, obj);
        }
		
		public LocationData GetLocation (string Key)
		{
            LocationData w;
			Locations.TryGetValue(Key,out w);
			if (w==null){
				Debug.LogError("Requested location "+Key+" does not exist.");
				return null;
			}
			return w;
		}
	}
}