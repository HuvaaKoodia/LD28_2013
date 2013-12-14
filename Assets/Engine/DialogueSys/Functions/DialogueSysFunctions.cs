using UnityEngine;
using System.Collections;
using System.Reflection;

namespace DialogueSystem{
	public class DialogueSysFunctions {

		public SceneManager SceneMan;
		QueryData current_query;
		public MethodInfo[] methods;

		public DialogueSysFunctions(){
			LoadMethods();
		}

		public void LoadMethods(){
			methods=GetType().GetMethods();

//			foreach (var m in methods){
//				Debug.Log(m.ToString());
//			}
		}

		public bool InvokeMethod (QueryData query,string f)
		{
			current_query=query;
			var spl=Subs.Split(f," ");
			foreach (var m in methods)
			{
				if (m.Name==spl[0]){
					if (spl.Length==1){
						m.Invoke(this,null);
						return true;
					}
					else{
						string[] args=new string[spl.Length-1];
						for (int i=0;i<args.GetLength(0);i++){
							args[i]=spl[i];
						}
						m.Invoke(this,args);
						return true;
					}
				}
			}
			return false;
		}

		public static void TestFunction(){
			Debug.Log("Function called!");
		}

		public void RemoveTargetFromWorld(){
			SceneMan.Location.RemoveEntity(current_query.Target);
			
		}

		public void AISpotTarget(){

		}

		public static void InventoryAdd(string[] args){

		}
	}
}
