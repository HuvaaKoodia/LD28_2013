using UnityEngine;
using System.Collections;
using System.Reflection;

namespace DialogueSystem{

	public class FunctionsBase {

		protected QueryData current_query;
		MethodInfo[] methods;

		public FunctionsBase(){
			LoadMethods();
		}

		public void LoadMethods(){
			methods=GetType().GetMethods();
		}
		/// <summary>
		/// Invokes the method.
		/// Returns true if method was found and returned
		/// </param>
		public bool InvokeMethod (QueryData query,string f,out object obj)
		{
			current_query=query;
			var spl=Subs.Split(f," ");
			foreach (var m in methods)
			{
				if (m.Name==spl[0]){
					if (spl.Length==1){
						obj=m.Invoke(this,null);
						return true;
					}
					else{
						string[] args=new string[spl.Length-1];
						for (int i=0;i<args.GetLength(0);i++){
							args[i]=spl[i];
						}
						obj=m.Invoke(this,args);
						return true;
					}
				}
			}
			obj=null;
			return false;
		}
	}
}
