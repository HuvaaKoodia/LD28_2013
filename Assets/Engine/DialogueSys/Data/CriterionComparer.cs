using UnityEngine;
using System.Collections;

namespace DialogueSystem{
	public class CriterionComparer{
				
		public float MinBound,MaxBound;
		
		public CriterionComparer(float min,float max){
			MinBound=min;
			MaxBound=max;
		}

		public bool Check(float Value){
			return Value>=MinBound&&Value<=MaxBound;
		}
		
	}
	
}