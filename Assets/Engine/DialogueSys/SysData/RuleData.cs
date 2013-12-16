using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{
	
	public class RuleData{
		
		public List<CriterionData> Criterions;
        public string Name { get; private set; }

		public RuleData(string name){
			Criterions=new List<CriterionData>();
            Assignments=new List<AssignData>();
			Functions=new List<string>();
            Name = name;
		}
		
		public void AddCriterion(CriterionData Data){
			Criterions.Add(Data);
		}

		//result
		public DialogueData Data;
        public List<AssignData> Assignments;
		public List<string> Functions;

        public void AddAssign(AssignData ass)
        {
            Assignments.Add(ass);
        }
    }
	
	public class TempRuleData{
		public string Link = "", Name, Base="",
		Location = RuleScope.NoLocation, _Event = RuleScope.NoEvent, Actor = RuleScope.NoActor,Target=RuleScope.NoTarget;
		public List<string> temp_criterions=new List<string>();
		public List<string> temp_assigns = new List<string>();
		public List<string> temp_functions = new List<string>();
	}
}