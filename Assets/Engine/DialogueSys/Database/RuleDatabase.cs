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
			var location=query.Location.Facts.Facts["Location"].Value;
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
                    }
                }
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
                    foreach (var c in query.containers)
                    {
                        bool done = false;
                        foreach (var f in c.Facts)
                        {
                            if (f.Key == a.Variable)
                            {
                                a.Assign(f.Value);
                                done = true;
                                break;
                            }
                        }

                        if (done) break;
                        Debug.LogError("Variable [" + a.Variable + "] in rule: [" + best_match.Name + "] not found in the character or the world.");
                    }
                }

				//functions
				
				foreach (var f in best_match.Functions)
				{
					var spl=Subs.Split(f,".");
					if (spl.Length>1){
						if (query.GetEntityData(spl[0]).Entity.Functions.InvokeMethod(query,f)) break;
					}
					else{
						if (Core.sys_functions.InvokeMethod(query,f)) break;
					}

					Debug.LogError("Function [" + f + "] in rule: [" + best_match.Name + "] not valid.");
					//DEV. did you mean?
				}
                
                return best_match;
            }
			return null;
		}

        FactData _temp;//DEV.MICRO.OPT.
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
            return true;//all criterions pass the test
        }

		//Data compile sys
		List<TempRuleData> temp_rules=new List<TempRuleData>();
		
		public void ParseDataBase(CoreDatabase core)
		{
			foreach (var r in temp_rules){

				//incorporate base
				if (r.Base!="")
				{
					var bas=temp_rules.Find(t=>t.Name==r.Base);

					if (r.Location==RuleScope.NoLocation){r.Location=bas.Location;}
					if (r.Actor==RuleScope.NoActor){r.Actor=bas.Actor;}
					if (r.Target==RuleScope.NoTarget){r.Target=bas.Target;}
					if (r._Event==RuleScope.NoEvent){r._Event=bas._Event;}
					if (r.Link==""){r.Link=bas.Link;}

					foreach (var c in bas.temp_criterions){
						r.temp_criterions.Add(c);
					}
					
					foreach (var a in bas.temp_assigns)
					{
						r.temp_assigns.Add(a);
					}
					
					foreach (var f in bas.temp_functions)
					{
						r.temp_functions.Add(f);
					}
				}

				//create data/references
				RuleData rule=new RuleData(r.Name);

				foreach (var c in r.temp_criterions){
					var spl=c.Split(' ');
					
					if (spl.Length==1){
						//reference to a predefined criterion
						CriterionData crit=core.criterion_database.GetCriterion(c);
						rule.AddCriterion(crit);
					}
					else{
						//inline criterion
						core.criterion_database.ParseCriterionCommand(c,rule.Criterions);
					}
				}
				
				foreach (var a in r.temp_assigns)
				{
					AssignData ass = new AssignData(a);
					rule.AddAssign(ass);
				}
				
				foreach (var f in r.temp_functions)
				{
					rule.Functions.Add(f);
				}
			
				if (r.Link!=""){
					var Data=core.dialogue_database.ParseDialogueData(r.Link);
					if (Data==null)
						Debug.LogError("Link: "+r.Link+" is faulty in rule called "+r.Name);
					rule.Data=Data;
				}
				//to correct scope
				AddToScope(new RuleScope(r.Location,r._Event,r.Actor,r.Target),rule);
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
            temp_rules = null;
		}
		
		public void addTempRule(TempRuleData r)
		{
			temp_rules.Add(r);
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