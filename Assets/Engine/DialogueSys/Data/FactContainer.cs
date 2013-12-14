﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{
	public class FactContainer{
		public Dictionary<string,FactData> Facts{get;private set;}
		
		public FactContainer(){
			Facts=new Dictionary<string,FactData>();
		}

		public FactData this[string name]
		{
			get { return this.Facts[name]; }
			set { this.Facts[name] = value; }
		}

        public void AddFact(string Key, FactData Value)
        {
            if (Facts.ContainsKey(Key))
            {
                Debug.Log("Adding. Fact already exists! " + Key);
                return;
            }
            Facts.Add(Key, Value);
        }

		public void AddFact(string Key,string Value){
			if (Facts.ContainsKey(Key)){
				Debug.Log("Adding. Fact already exists! "+Key);
				return;
			}
			Facts.Add(Key,new FactData(Value));
		}
		
		public void AddFact(string Key,float Value){
			if (Facts.ContainsKey(Key)){
				Debug.Log("Adding. Fact already exists! "+Key);
				return;
			}
			Facts.Add(Key,new FactData(Value));
		}
		
		public void AddFact(string Key,bool Value){
			if (Facts.ContainsKey(Key)){
				Debug.Log("Adding. Fact already exists! "+Key);
				return;
			}
			Facts.Add(Key,new FactData(Value));
		}
		
		public void SetFact(string Key,string Value){
			if (Facts.ContainsKey(Key)){
				Debug.Log("Setting. Fact doesn't exists! "+Key);
				return;
			}
			Facts[Key].SetValue(Value);
		}
		
		public void SetFact(string Key,float Value){
			if (!Facts.ContainsKey(Key)){
				Debug.Log("Setting. Fact doesn't exists! "+Key);
				return;
			}
			Facts[Key].SetValue(Value);
		}
		
		public void SetFact(string Key,bool Value){
			if (!Facts.ContainsKey(Key)){
				Debug.Log("Setting. Fact doesn't exists! "+Key);
				return;
			}
			Facts[Key].SetValue(Value);
		}
		
		public int GetInt(string name)
		{
			return (int)GetFloat (name);
		}
		public float GetFloat(string name)
		{
			if (!Facts.ContainsKey(name)) {Debug.LogError("Fact "+name+" doesn't exist in container.");return -1;}
			return (float)Facts[name].ValueObject;
		}
		public bool GetBool(string name)
		{
			if (!Facts.ContainsKey(name)) {Debug.LogError("Fact "+name+" doesn't exist in container.");return false;}
			return (bool)Facts[name].ValueObject;
		}
		public string GetString(string name)
		{
			if (!Facts.ContainsKey(name)) {Debug.LogError("Fact "+name+" doesn't exist in container.");return "";}
			return Facts[name].Value;
		}
		
		public void PrintFacts ()
		{
			string print="Facts: \n";
			foreach (var f in Facts){
				print+=""+f.Key+":["+f.Value.Value+","+f.Value.Symbol+"]\n";
			}
			Debug.Log(print);
		}
	}
}