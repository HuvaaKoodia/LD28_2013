using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem{
	public class DialogueDatabase{
		
		public Dictionary<string,DialogueData> DialogueDatas{get;private set;}
		/// <summary>
		/// Call after all dialogue XML files have been read.
		/// Sets direct references for the data files;
		/// DEV. could also get rid of non start dialogues in the dialogue dictionary? -> only access to the beginnings.
		/// </summary>
		public void ParseDataBase(){
			foreach (var dd in DialogueDatas){
				if (dd.Value.TempLinks.Count>0){
					foreach (DialogueLink l in dd.Value.TempLinks){
						dd.Value.AddLink(ParseDialogueData(l.Link));
					}
					dd.Value.TempLinks.Clear();
				}
			}
			/*Debug.Log("After parse");
			foreach (var dd in DialogueDatas){
				Debug.Log("N: "+dd.Key+" d:"+dd.Value.Text);
				if (dd.Value.hasAnswers()){
					foreach (var l in dd.Value.Answers){
						Debug.Log("l: "+l.Link+" d:"+l.Data.Text);
					}	
				}
			}*/
		}
		
		public void AddDialogueData(string name,DialogueData data){
			if (!DialogueDatas.ContainsKey(name))
				DialogueDatas.Add(name,data);
			else{
				Debug.LogError("DialogueData called "+name+" already exists!");
			}
		}
	
		public DialogueData GetDialogueData(string name)
		{
			if (DialogueDatas.ContainsKey(name))
				return DialogueDatas[name];
			Debug.LogError("DialogueData called "+name+" doesn't exist!");
			return null;
		}

		public DialogueData ParseDialogueData(string spr)
		{
			if (spr.StartsWith("\""))
			{
				spr=spr.Remove(0,1);
				var spl = spr.Split(new char[]{'"'},System.StringSplitOptions.RemoveEmptyEntries);
				var text = spl[0];

				if (spl.Length==1)
				{
					return new DialogueData(text);
				}
				else {
					var spl2=spl[1].Split(new char[]{' '},System.StringSplitOptions.RemoveEmptyEntries);
					if (spl2[0] == "to")
					{
						if (spl2.Length==2)
						{
							var d=new DialogueData(text);
							d.AddLink(GetDialogueData(spl2[1]));
							return d;
						}
						else{
							return new DialogueData(text,spl2[1],spl2[2],true);
						}
					}
					
					if (spl2[0] == "type")
					{
						return new DialogueData(text,spl2[1]);
					}

					Debug.LogError("Dialogue command \""+spl2[0]+"\" is faulty.\n(possible commands: \"to\", \"type\")");
				}
			}
			else{
				var spl =Subs.Split(spr," ");
				
				if (spl.Length>1){
					if (spl[0] == "select")
					{
						if (spl.Length==3)
						{
							return new DialogueData("",spl[1],spl[2],false);
						}
					}
				}
			}
			return GetDialogueData(spr);
			

		}
		
		public DialogueData 
			EndDialogueEndConversation,
			EndDialogueNoComment,
			EndDialogueAwkwardSilence;
		
		// Use this for initialization
		public DialogueDatabase(){
			DialogueDatas=new Dictionary<string, DialogueData>();
	
			//end messages
			EndDialogueEndConversation=new DialogueData("<End conversation>","ENDL");
			EndDialogueNoComment=new DialogueData("<No comment>","ENDL");
			EndDialogueAwkwardSilence=new DialogueData("<AwkwardSilence>","ENDL");
		}
	}
}