using UnityEngine;
using System.Collections;
using System;

namespace DialogueSystem{
	public class FactData{
		public object ValueObject{get;private set;}
		public string Value{get;private set;}
		public float Symbol{get;private set;}
		
		public float MinValue{
			get{return min;}
			set{min=value;has_min_value=true;}
		}
		public float MaxValue{
			get{return max;}
			set{max=value;has_max_value=true;}
		}
		
		private float min,max;
		public bool has_min_value{get;private set;}
		public bool has_max_value{get;private set;}
		
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
			SetSymbol(v);
		}
		
		public void SetValue(bool v){
			Value=v.ToString();
			if (v)
				Symbol=1;
			else
				Symbol=0;
			ValueObject = v;
		}

        public void SetSymbol(float s)
        {
			if (has_min_value&&s<min)
				s=min;
			if (has_max_value&&s>max)
				s=max;
			
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