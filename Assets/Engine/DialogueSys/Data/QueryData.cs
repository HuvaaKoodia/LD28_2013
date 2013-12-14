using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{

    public class QueryData
    {
        //query spesific facts
        public Dictionary<string, FactData> Facts;
        public List<FactContainer> containers;
        public EntityData Actor { get; private set; }
		public EntityData Target { get; private set; }
		
		public string _Event;
        
		public LocationData Location {get;private set;}

        public QueryData()
        {
            Facts = new Dictionary<string, FactData>();
            containers = new List<FactContainer>();
        }

        public QueryData(LocationData location,EntityData actor,EntityData target,string _event):this()
        {
			Location=location;
            Actor = actor;
			Target=target;

			//AddFact("Location", location.Facts.Facts["Location"]);
            if (actor!=null){
				//AddFact("ActorClass", actor.Class);
                //AddFact("ActorType", actor.Type);
			}
			_Event=_event;//AddFact("Event",new FactData(_event));

            AddContainer(actor.Facts);
			AddContainer(location.Facts);
			AddContainer(target.Facts);
        }

        public void AddFact(string Key, FactData data)
        {
            if (Facts.ContainsKey(Key))
            {
                Debug.Log("QUERY: " + this + " already contains a fact about " + Key);
            }
            Facts.Add(Key, data);
        }

        public void AddContainer(FactContainer con) {
            containers.Add(con);
        }

        FactData _temp;

		public FactData FindFact(string container,string fact){
			FactContainer c=GetEntityData(container).Facts;

			if (c!=null&&c.Facts.TryGetValue(fact, out _temp))
			{
				return _temp;
			}
			return null;
		}

        public FactData FindFact(string name)
        {
			var spl=Subs.Split(name,".");

			if (spl.Length>1){
				return FindFact(spl[0],spl[1]);
			}

//            if (Facts.TryGetValue(name, out _temp))
//            {
//                return _temp;
//            }

            foreach (var con in containers)
            {
                if (con.Facts.TryGetValue(name, out _temp))
				{
                    return _temp;
                }
            }
            return null;
        }

		public EntityData GetEntityData(string name){
			if (name=="Character"||name=="Actor"){
				return Actor;
			}if (name=="Target"){
				return Target;
			}if (name=="Location"){
				return Location;
			}
			return null;
		}
    }
}