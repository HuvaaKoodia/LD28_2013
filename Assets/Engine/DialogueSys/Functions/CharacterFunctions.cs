using UnityEngine;
using System.Collections;
using System.Reflection;

namespace DialogueSystem{

	public class CharacterFunctions:FunctionsBase {

		public CharacterFunctions():base()
		{

		}
		
		public void StealFromTarget(){
			Debug.Log(current_query.Actor.Name+ " stealing from "+ current_query.Target.Name);
			
			Debug.Log("Actor money "+ current_query.Actor.Facts.GetFloat("Money"));
			Debug.Log("Target money "+ current_query.Target.Facts.GetFloat("Money"));
			
			current_query.Actor.Facts.AddFactValue("Money",current_query.Target.Facts.GetFloat("Money"));
			current_query.Target.Facts.SetFact("Money",0);
			
			Debug.Log("Actor money "+ current_query.Actor.Facts.GetFloat("Money"));
			Debug.Log("Target money "+ current_query.Target.Facts.GetFloat("Money"));
		}
		
		public bool HasInfoFreelancer()
		{
			return
				current_query.Actor.Facts.GetBool("DealerMadeDealPolitician")||
				current_query.Actor.Facts.GetBool("PoliticianBuySex")||
				current_query.Actor.Facts.GetBool("PoliceBlackmailPolitician")||
				current_query.Actor.Facts.GetBool("PoliceArrestPolitician");
		}
	}
}
