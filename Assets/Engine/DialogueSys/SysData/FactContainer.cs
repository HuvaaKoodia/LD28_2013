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

        public FactData AddFact(string Key, FactData Value,bool copy_instance)
        {
            if (Facts.ContainsKey(Key))
            {
                Debug.Log("Adding Fact. Fact already exists! " + Key);
                return null;
            }
			
			if (copy_instance){
				System.Type type=Value.ValueObject.GetType();
				//Dev.lazy
				FactData f=null;
				if (type==typeof(float)){
					f=new FactData((float)Value.ValueObject);
				}
				else if (type==typeof(string)){
					f=new FactData((string)Value.ValueObject);
				}
				else if (type==typeof(bool)){
					f=new FactData((bool)Value.ValueObject);
				}
				if (Value.has_min_value)
					f.MinValue=Value.MinValue;
				if (Value.has_max_value)
					f.MaxValue=Value.MaxValue;
				Facts.Add(Key, f);
				return f;
			}
			
       	 	Facts.Add(Key, Value);
			return Value;
        }
		
		public FactData AddOrSetFact(string Key, FactData Value,bool copy_instance)
        {
            if (Facts.ContainsKey(Key))
            {
                var f=Facts[Key];
				
				System.Type type=Value.ValueObject.GetType();
				if (type==typeof(float)){
					f.SetValue((float)Value.ValueObject);
				}
				else if (type==typeof(string)){
					f.SetValue((string)Value.ValueObject);
				}
				else if (type==typeof(bool)){
					f.SetValue((bool)Value.ValueObject);
				}
				
                return f;
            }
			
			return AddFact(Key,Value,copy_instance);
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
		
		public void AddFactValue(string Key,float Add){
			if (!Facts.ContainsKey(Key)){
				Debug.Log("Adding value. Fact doesn't exists! "+Key);
				return;
			}
			Facts[Key].SetValue(Facts[Key].Symbol+Add);
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