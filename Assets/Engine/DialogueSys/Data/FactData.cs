using UnityEngine;
using System.Collections;
using System;

namespace DialogueSystem{
	public class FactData{
		public object ValueObject{get;private set;}
		public string Value{get;private set;}
		public float Symbol{get;private set;}

        //public string Key{get;private set;}
		public bool IsString{get {return ValueObject.GetType()==typeof(string);}}
		
		public FactData(string v){
			SetValue(v);
		}
		
		public FactData(float v){
			SetValue(v);
		}
		
		public FactData(bool v){
			SetValue(v);
		}
		
		public void SetValue(string v){
			Value=v;
			Symbol=SymbolDatabase.GetSymbol(v);
			ValueObject = v;
		}
		
		public void SetValue(float v){
			Value=v.ToString();
			Symbol=v;
			ValueObject = v;
		}
		
		public void SetValue(bool v){
			Value=v.ToString();
			if (v)
				Symbol=1;
			else
				Symbol=0;
			ValueObject = v;
		}

        public void setSymbol(float s)
        {
            Symbol = s;
            Value = s.ToString();
			ValueObject = s;
        }

        public static FactData ParseStringToData(string str)
        {
            try
            {
                float f = float.Parse(str);
                return new FactData(f);
            }
            catch (Exception e)
            {
                //not float
            }
            if (str == "true")
            {
                return new FactData(true);
            }
            if (str == "false")
            {
                return new FactData(false);
            }
            return new FactData(str);
        }
    }
}