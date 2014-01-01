using UnityEngine;
using System.Collections;
using System.Reflection;

namespace DialogueSystem{
	public class DialogueSysFunctions:FunctionsBase{

		public SceneManager SceneMan;

		public DialogueSysFunctions():base(){}

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
