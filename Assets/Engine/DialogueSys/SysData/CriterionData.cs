using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{

    public enum Comparison{ EQUAL, LESS, GREATER, LEQUAL, GEQUAL }

	public class CriterionData{
       
		public const float Precision=0.001f;
		public CriterionComparer Comparer;
		
		public string 	Name	{get;private set;}
    	public bool 	Normal	{get;private set;}
		public string 	Command	{get;private set;}
		public System.Type ValueType{get;private set;}

		public CriterionData(string name,string Value){
			set_criterion(new FactData(Value).Symbol,Comparison.EQUAL);
            set_name(name);
			ValueType=Value.GetType();
		}
		
        public CriterionData(string name, float Value, Comparison operation)
        {
			set_criterion(new FactData(Value).Symbol,operation);
            set_name(name);
			ValueType=Value.GetType();
		}
        public CriterionData(string name, bool Value)
        {
			set_criterion(new FactData(Value).Symbol,Comparison.EQUAL);
            set_name(name);
			ValueType=Value.GetType();
		}
		
		public CriterionData(FactData data,Comparison operation){
			set_criterion(data.Symbol,operation);
			ValueType=data.ValueObject.GetType();
		}
		
		private void set_criterion(float Value,Comparison operation){
			float min=0,max=0;

			if (operation==Comparison.EQUAL){
				min=max=Value;
			}
			else if (operation==Comparison.LESS){
				min=float.NegativeInfinity;max=Value-Precision;
			}
			else if (operation==Comparison.LEQUAL){
				min=float.MinValue;max=Value;
			}
			else if (operation==Comparison.GREATER){
				min=Value+Precision;max=float.PositiveInfinity;
			}
			else if (operation==Comparison.GEQUAL){
				min=Value;max=float.PositiveInfinity;
			}

			Comparer=new CriterionComparer(min,max);
		}

        private void set_name(string name){
            
			Normal=!name.Contains(".");
			if (!Normal){
				var spl=Subs.Split(name,".");
				Command=spl[0];
				Name=spl[1];
				return;
			}
			Name = name;

        }
	}
}