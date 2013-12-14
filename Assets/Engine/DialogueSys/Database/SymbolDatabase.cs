using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DialogueSystem{
	
	public class SymbolDatabase{
		
		public static Dictionary<string,float> Symbols=new Dictionary<string, float>();
		private static int current_symbol=1;

		//returns the symbol representation for a given word string.
		public static float GetSymbol(string Value){
			float symbol=0;
			if (Symbols.TryGetValue(Value,out symbol)){
				return symbol;
			}
			Symbols.Add(Value,current_symbol);
			return current_symbol++;
		}
		/// <summary>
		/// Returns the float representation for a string . 
		/// </summary>
		public static float StringToSymbol(string str){
			try{
				float f=float.Parse(str);
				return f;
			}
			catch(Exception e){
				//not float
			}
			if (str=="true"){
				return 1;
			}
			if (str=="false"){
				return 0;
			}
			//is string
			return GetSymbol(str);
		}
	}
}