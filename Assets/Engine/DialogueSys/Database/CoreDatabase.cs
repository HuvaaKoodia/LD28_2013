using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{
	
	public class CoreDatabase:MonoBehaviour{

		public SceneManager SceneMan;

		public DialogueDatabase dialogue_database{get;private set;}
		public RuleDatabase rule_database{get;private set;}
		public CriterionDatabase criterion_database{get;private set;}
        public ObjectDatabase object_database { get; private set; }
        public CharacterDatabase character_database { get; private set; }
        public SceneDatabase scene_database { get; private set; }
        public LocationDatabase location_database { get; private set; }

		public DialogueSysFunctions sys_functions { get;private set; }

		public void Awake(){
			dialogue_database=new DialogueDatabase();
			rule_database=new RuleDatabase(this);
			criterion_database=new CriterionDatabase();
            object_database = new ObjectDatabase();
            character_database = new CharacterDatabase(this);
            scene_database = new SceneDatabase();
            location_database = new LocationDatabase();

			sys_functions=new DialogueSysFunctions();

			sys_functions.SceneMan=SceneMan; 
		}
	}
}