using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{
   
	public class RuleDatabase{
        Dictionary<RuleScope, List<RuleData>> Scopes = new Dictionary<RuleScope, List<RuleData>>();
		CoreDatabase Core;

		void AddToScope(RuleScope scope,RuleData rule){
			if (!Scopes.ContainsKey(scope)){
				Scopes.Add(scope,new List<RuleData>());
			}
			Scopes[scope].Add(rule);
		}
		
		public RuleDatabase(CoreDatabase core){
			Core=core;
			Scopes.Add(new RuleScope(RuleScope.NoLocation, RuleScope.NoEvent, RuleScope.NoActor,RuleScope.NoTarget), new List<RuleData>());
		}

		//Query sys.
		/// <summary>
		/// Finds the best match for this query.
        /// Deals with assignments as well
		/// </summary>
		public RuleData CheckQuery(QueryData query){
			var location=query.Location.Name;
            string _event = RuleScope.NoEvent,_actor=RuleScope.NoActor,_target=RuleScope.NoTarget;
            //FactData fact;

			_event =  query._Event;

            if (query.Actor!=null)
            {
				_actor = query.Actor.Type;
            }
			if (query.Target!=null)
			{
				_target = query.Target.Type;
			}

            int count_of_checks = 0;//DEV.TEMP
			RuleData best_match=null;
			
            //add all relevant scopes (target doesn't need a global scope check -> allways specific)
            List<RuleScope> scopes = new List<RuleScope>();
            scopes.Add(new RuleScope(location, _event, _actor,_target));//most specific scope

            if (_actor != RuleScope.NoActor)
				scopes.Add(new RuleScope(location, _event, RuleScope.NoActor,_target));//global actor scope

            if (location != RuleScope.NoLocation)//globals location scopes
            {
                scopes.Add(new RuleScope(RuleScope.NoLocation, _event, _actor,_target));
                if (_actor!=RuleScope.NoActor)
                    scopes.Add(new RuleScope(RuleScope.NoLocation, _event, RuleScope.NoActor,_target));
            }

            //check all rules in said scopes
#if DEBUG
            Debug.Log("\nChecking for rules in possible scopes:");
#endif
            foreach (var s in scopes)
            {
				bool break_scopes_check=false;
                if (!Scopes.ContainsKey(s))
                {
//#if DEBUG
					Debug.Log("Not Found: " + s);
//#endif
                    continue;
                }
//#if DEBUG
				Debug.Log("Found: " + s +" size: "+Scopes[s].Count);
//#endif
                foreach (var r in Scopes[s])
                {
                    if (best_match != null && r.Criterions.Count < best_match.Criterions.Count)
                    {
                        break;
                    }
                    count_of_checks++;
                    if (CheckRule(r,query))
                    {
                        best_match = r;
						break_scopes_check=true;//take bestmatch from the most specific scope only. 
                    }
                }
				
				if (break_scopes_check) break;//DEV. TEMP rewiring the system.
            }
//#if DEBUG
            Debug.Log("Rule check count total:"+count_of_checks+"\n");
//#endif
			//results
            if (best_match != null)
            {
                //assignments
                foreach (var a in best_match.Assignments)
                {
					var fact=query.FindFact(a.Target,a.Variable);
					if (fact!=null){
						a.Assign(fact);
					}
					else{
						Debug.LogError("Variable [" + a.Variable + "] in rule: [" + best_match.Name + "] not found in ["+a.Target+"] or the location.");
					}
                }

				//functions
				
				foreach (var f in best_match.Functions)
				{
					var spl=Subs.Split(f,".");
					if (spl.Length>1){
						var cont=query.GetEntityData(spl[0]);
						
						if (cont.Entity.Functions.InvokeMethod(query,spl[1],out _obj)) break;
					}
					else{
						if (Core.sys_functions.InvokeMethod(query,f,out _obj)) break;
					}

					Debug.LogError("Function [" + f + "] in rule: [" + best_match.Name + "] not valid.");
					//DEV. did you mean?
				}
				
                //calls
				
				foreach(var call in best_match.Calls){
					var spl=Subs.Split(call," ");
					
					foreach (var character in query.Location.Characters){
						if (character.Type==spl[0]){
							CheckQuery(new QueryData(query.Location,character,query.Target,spl[1]));
						}
					}
				}
				
                return best_match;
            }
			return null;
		}
		//DEV.MICRO.OPT.
		object _obj;
		FactData _temp;
		
        private bool CheckRule(RuleData rule,QueryData query) {
            foreach (var c in rule.Criterions)
            {
                if (c.Normal)
                {
                    if (query.Facts.TryGetValue(c.Name, out _temp))
                    {
                        if (c.Comparer.Check(_temp.Symbol))
                            continue;//query has the fact with a correct value
                        return false;//query has the fact with a different value
                    }

                    bool found = false;
                    foreach (var con in query.containers)
                    {
                        if (con.Facts.TryGetValue(c.Name, out _temp))
                        {
                            if (c.Comparer.Check(_temp.Symbol))
                            {
                                found = true;
                                break;
                            }
                            return false;
                        }
                        else
                            continue;//next container
                    }
                    if (found) continue;
                    return false;//criterion not found in the query
                }
                else {
					// criterion in a specific container (e.g Location.Raining, Character.HP etc.)
					var find_fact=query.FindFact(c.Command,c.Name);
					if (find_fact!=null){
						if (!c.Comparer.Check(find_fact.Symbol))
							return false;//check fails -> rule fails!
					}

                    //special criterion DEV. obsolete?
                    var spl=c.Name.Split('.');
                    var container=spl[0].ToLower();

                    if (container == "inventory"){
                        if (spl.Length == 2) {
							CharacterData cha=(CharacterData)query.Actor;
                            //check the amount of the item in inventory
                            if (!c.Comparer.Check(cha.Inventory.Amount(spl[1])))
                            {
                                return false;//not the same amount -> Rule fail
                            }    
                        }
                        else if (spl.Length == 3)
                        {
                            //check for a variable of the item in inventory (e.g. Inventory.Item.Durability)
                            //DEV.IMP.
                        }
                    }
                }
            }
			
			//criterion functions
			foreach (var c in rule.CriterionFunctions)
            {
				//function criterion
				_obj=null;
				if (c.Command!=""){
					var cont=query.GetEntityData(c.Command);
					
					cont.Entity.Functions.InvokeMethod(query,c.Name,out _obj);
				}
				else{
					Core.sys_functions.InvokeMethod(query,c.Name,out _obj);
				}
				if (_obj==null){
					Debug.LogWarning("Criterion function: "+c.Name+" returned nothing!");
					return false;
				}
				
				var str=_obj.ToString();
				float symbol=SymbolDatabase.StringToSymbol(_obj.ToString());
			
//				if (c.ValueType==typeof(float)){
//					symbol=(float)_obj;
//				}
//				else if (c.ValueType==typeof(string)){
//					symbol=SymbolDatabase.get (string)_obj;
//				}
//				else if (c.ValueType==typeof(bool)){
//					
//				}
				
				
				if (!c.Comparer.Check(symbol))
					return false;//check fails -> rule fails!
            }
			
            return true;//all criterions pass the test
        }

		//Data compile sys
		Dictionary<string,TempRuleData> temp_rules=new Dictionary<string,TempRuleData>();
		
		void CompileRule(TempRuleData r){
			if (r.already_compiled) return;
			r.already_compiled=true;
			
			if (r.Base!="")
			{
				TempRuleData _base=temp_rules[r.Base];
			
				//recursively compile base rule
				CompileRule(_base);
			
				if (r.Location==RuleScope.NoLocation){r.Location=_base.Location;}
				if (r.Actor==RuleScope.NoActor){r.Actor=_base.Actor;}
				if (r.Target==RuleScope.NoTarget){r.Target=_base.Target;}
				if (r._Event==RuleScope.NoEvent){r._Event=_base._Event;}
				if (r.Link==""){r.Link=_base.Link;}

				foreach (var c in _base.temp_criterions){
					r.temp_criterions.Add(c);
				}
				
				foreach (var c in _base.temp_criterion_functions){
					r.temp_criterion_functions.Add(c);
				}
				
				foreach (var a in _base.temp_assigns)
				{
					r.temp_assigns.Add(a);
				}
				
				foreach (var f in _base.temp_functions)
				{
					r.temp_functions.Add(f);
				}
				
				foreach (var c in _base.temp_calls)
				{
					r.temp_calls.Add(c);
				}
			}
		}
		
		public void ParseDataBase(CoreDatabase core)
		{
			foreach (var r in temp_rules){
				
				var rule_temp=r.Value;
				
				//incorporate base rule
				CompileRule(rule_temp);
				
				//create data
				RuleData rule_data=new RuleData(rule_temp.Name);

				foreach (var c in rule_temp.temp_criterions){
					var spl=c.Split(' ');
					
					if (spl.Length==1){
						//reference to a predefined criterion
						CriterionData crit=core.criterion_database.GetCriterion(c);
						rule_data.AddCriterion(crit);
					}
					else{
						//inline criterion(s) 
						core.criterion_database.ParseCriterionCommand(c,rule_data.Criterions);
					}
				}
				
				foreach (var c in rule_temp.temp_criterion_functions){
					//inline criterion function
					core.criterion_database.ParseCriterionCommand(c,rule_data.CriterionFunctions);
				}
				
				foreach (var a in rule_temp.temp_assigns)
				{
					AssignData ass = new AssignData(a);
					rule_data.AddAssign(ass);
				}
				
				foreach (var f in rule_temp.temp_functions)
				{
					rule_data.Functions.Add(f);
				}
				
				foreach (var c in rule_temp.temp_calls)
				{
					rule_data.Calls.Add(c);
				}
			
				if (rule_temp.Link!=""){
					var Data=core.dialogue_database.ParseDialogueData(rule_temp.Link);
					if (Data==null)
						Debug.LogError("Link: "+rule_temp.Link+" is faulty in rule called "+rule_temp.Name);
					rule_data.Link=Data;
				}
				
				//add to correct scope
				AddToScope(new RuleScope(rule_temp.Location,rule_temp._Event,rule_temp.Actor,rule_temp.Target),rule_data);
			}

            //sorting rule lists
            foreach (var s in Scopes) {
                s.Value.Sort(comparer);
            }

			/*
			Debug.Log("Scopes");
			foreach (var s in Scopes) {
				Debug.Log(s.Key +" size: "+s.Value.Count);
			}
			*/
			
			//get rid of temp data
            temp_rules.Clear();
			temp_rules=null;
		}
		
		public void addTempRule(TempRuleData r)
		{
			temp_rules.Add(r.Name,r);
		}

        private int comparer(RuleData r1, RuleData r2) {
            if (r1.Criterions.Count < r2.Criterions.Count) {
                return 1;
            }
            if (r1.Criterions.Count > r2.Criterions.Count)
            {
                return -1;
            }
            return 0;
        }
	}


    class RuleScope
    {
		public const string NoLocation = "#Global#", NoEvent = "#NoEvent#", NoActor = "#AnyChar#",NoTarget = "#AnyTarget#";
        string Location,_Event,Actor,Target;

        public RuleScope(string location, string _event, string actor,string target)
        {
            Location = location;
            _Event = _event;
			Actor = actor;
			Target=target;
        }

        public override bool Equals(object other)
        {
            RuleScope o = other as RuleScope;
			return Location == o.Location && _Event == o._Event && Actor == o.Actor&&Target==o.Target;
        }

        public override int GetHashCode()
        {
			var hash = (Location + _Event + Actor + Target).GetHashCode();
            return hash;
        }

        public override string ToString()
        {
			return "[Location:" + Location + ",Event:" + _Event + ",Character:" + Actor + ",Target:" + Target + "]";
        }
    }
}