using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DialogueSystem{

    public enum Operator {Plus,Minus,Multi,Div,Assign}

    public class AssignData{
		string _operator;
		public string Variable {get;private set;}
		public float Value{get;private set;}
		public AssignData(string Data){
			var spl=Data.Split(' ');
			
			if (spl.Length!=3){
                string s = "Assign Data error.\nShould have 3 parts: [variable,operator,value}. Current Data [";
                for (int i=0;i<spl.Length;i++){
                    s += spl[i];
                    if (i!=spl.Length-1){
                        s += ",";
                    }
                }
                s += "]";
				Debug.LogError(s);
				return;
			}
			Variable=spl[0];
            if (spl[1] != "=") {
                try
                {
                    float.Parse(spl[2]);
                }
                catch (Exception e) {
                    //not float
                    Debug.LogError("Assign: " + Variable + " has a faulty operator! (" + spl[1] + ")\n(Arithmetic operators only work on numbers.)");
                }
            }
            _operator = spl[1];

			Value=SymbolDatabase.StringToSymbol(spl[2]);
		}

        public void Assign(FactData fact) {
            switch (_operator)
            {
                case "=": fact.setSymbol(Value); break;
                case "+": fact.setSymbol(fact.Symbol + Value); break;
                case "-": fact.setSymbol(fact.Symbol - Value); break;
                case "*": fact.setSymbol(fact.Symbol * Value); break;
                case "/": fact.setSymbol(fact.Symbol / Value); break;
            }
        }
	}
}