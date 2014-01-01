using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DialogueSystem{

	public class CriterionDatabase{
		public Dictionary<string,CriterionData> Criterions{get;private set;}
		
		public CriterionDatabase(){
			Criterions=new Dictionary<string,CriterionData>();
		}
		
		public void AddCriterion(string Key,string Value){

			var spl=Value.Split(' ');

			Criterions.Add(Key,ParseCriterionCommand(spl[0],spl[1]+" "+spl[2]));//LAZY HAX
		}
		/// <summary>
		/// Parses a full criterion command (e.g. Criterion1 L 100 G 10) and a adds all required Criterions to the 
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public void ParseCriterionCommand(string Value,List<CriterionData> Criterions){

			var spl=Value.Split(' ');
            
            string name=spl[0];

            int start_with = 1;
            int times = 1;
            if (spl.Length == 5)
                times = 2;

            for (int i = 0; i < times; i++)
            {
                Criterions.Add(ParseCriterionCommand(name,spl[start_with]+" "+spl[start_with+1]));
                start_with+=2;
            }
		}

        /// <summary>
        /// Parses a criterion command (e.g. GE 10) with a variable name to a Criterion data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CriterionData ParseCriterionCommand(string var_name,string _value)
        {
            Comparison o = Comparison.EQUAL;

            var spl = _value.Split(' ');

            switch (spl[0])
            {
                case "E": o = Comparison.EQUAL; break;
                case "L": o = Comparison.LESS; break;
                case "G": o = Comparison.GREATER; break;
                case "LE": o = Comparison.LEQUAL; break;
                case "GE": o = Comparison.GEQUAL; break;
            }

            try
            {
                float f = float.Parse(spl[1]);
				return new CriterionData(var_name, f, o);
			}
            catch (Exception e)
            {
                //not float
            }
            if (spl[1] == "true")
            {
				return new CriterionData(var_name, true);
            }
            if (spl[1] == "false")
            {
				return new CriterionData(var_name, false);
            }
            //is string
			return new CriterionData(var_name, spl[1]);
        }
        
		CriterionData crit;
		public CriterionData GetCriterion (string Key)
		{
			Criterions.TryGetValue(Key,out crit);
			if (crit==null){
				Debug.LogError("Requested criterion "+Key+" does not exist.");
				return null;
			}
			return crit;
		}
		
		public void Print(){
			foreach (var c in Criterions){
				Debug.Log("Criterion: "+c.Key+", "+c.Value);
			}
			
		}
	}
}