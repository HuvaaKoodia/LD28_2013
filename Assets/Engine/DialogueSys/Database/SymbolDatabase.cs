using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DialogueSystem{
	
	public class SymbolDatabase{
		
		public static Dictionary<string,float> Symbols=new Dictionary<string, float>();
		private static int current_symbol=1;
		private static float _symbol;//DEV.micro opt
		
		/// <summary>
		/// Returns the symbol representation for a given word string.
		/// Add the word if no string found.
		/// </summary>
		public static float GetSymbol(string Value){
			
			if (Symbols.TryGetValue(Value,out _symbol)){
				return _symbol;
			}
			Symbols.Add(Value,current_symbol);
			return current_symbol++;
		}
		/// <summary>
		/// Returns the float representation for a string.
		/// e.g.
		/// "5" -> 5,
		/// "true" -> 1,
		/// "false" -> 0,
		/// "any other string" -> correct symbol
		/// </summary>
		public static float StringToSymbol(string str){
			try{
				return float.Parse(str);
			}
			catch(Exception e){
				//not float
			}
			if (str.ToLower()=="true"){
				return 1;
			}
			if (str.ToLower()=="false"){
				return 0;
			}
			//is string
			return GetSymbol(str);
		}
	}
}