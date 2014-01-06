using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public delegate void OnAnswerButtonPressed(AnswerButtonMain button);

public class DialogueManager : MonoBehaviour {
	
	public CoreDatabase core_database;

	public bool DIALOGUE_ON{get;private set;}
    public DialogueData CurrentDialogue { get; private set; }
    public QueryData CurrentQuery { get; private set; }

	//graphics
	public SpeechBubbleMain speech_bubble;
	public GameObject answer_button_prefab,answer_buttons_parent;

	List<AnswerButtonMain> answer_buttons=new List<AnswerButtonMain>();
	
	public OnAnswerButtonPressed OnAnswerButtonPressedEvent;

	void Start () {}

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.F3)){
			StopDialogue();
		}
		
		if (DIALOGUE_ON){
			if (Input.GetKeyDown(KeyCode.Alpha1)){
				if (answer_buttons.Count>0){
					AnswerButtonPressed(answer_buttons[0]);
				}
			}
			
			if (Input.GetKeyDown(KeyCode.Alpha2)){
				if (answer_buttons.Count>1){
					AnswerButtonPressed(answer_buttons[1]);
				}
			}
			
			if (Input.GetKeyDown(KeyCode.Alpha3)){
				if (answer_buttons.Count>2){
					AnswerButtonPressed(answer_buttons[2]);
				}
			}
		}
	}

    public void OpenDialogue(DialogueData data, QueryData query)
    {
		if (speech_bubble!=null)
			speech_bubble.appear();

		DIALOGUE_ON=true;
        CurrentQuery = query;
		ChangeDialogue(data,query);
	}

    public void StopDialogue()
    {
        DIALOGUE_ON = false;
        CurrentDialogue = null;
        ClearAnswers();
        Debug.Log("Conversation ends.");
		
		if (speech_bubble!=null)
			speech_bubble.disappear();
    }

    void ChangeDialogue(DialogueData data, QueryData query)
    {
        if (data.Type == "ENDL")
        {
            StopDialogue();
            return;
        }

		if (data.Type=="RANDOM"){
			if (!data.HasLinks()){
				Debug.LogError("No anwers in "+data.Text+". type: RANDOM.");
				return;
			}
			data=data.GetRandom();
		}

        //Debug.Log("Text: " + data.ParseText(query) + "\n");
		if (speech_bubble!=null)
			speech_bubble.setText(data.ParseText(query));

        CurrentDialogue = data;

		SetAnswers(data,query);
	}
    void SetAnswers(DialogueData data, QueryData query)
    {
		ClearAnswers();
		
		if (data==null) return;

		/*
		if (data.HasLinks()){
            int i=0;
            Debug.Log("Answers:");
			foreach (var a in data.Links){
                Debug.Log((++i)+":\n"+a.ParseText(query) + "\n");
            }
		}*/
		
		var links=GetLinks(data);
		
		if (links.Count>0){

			foreach (var d in links){
				AddAnswer(d);
			}	
		}
		else{
			ClearAnswers();
			AddAnswer(core_database.dialogue_database.EndDialogueEndConversation);
		}
	}
	
	/// <summary>
	/// Dev.Reloc. to dialogue database (static)
	/// </param>
	public List<DialogueData> GetLinks(DialogueData data){
		var list=new List<DialogueData>();
		
		if (data.HasLinks()){

			foreach (var d in data.Links){
				if (d.Type=="SKIP") continue;
				
				if (d.LinksToQuery){
					var query=new QueryData(CurrentQuery.Location,CurrentQuery.Actor,CurrentQuery.Target,d.ToEvent);
					var rule=core_database.rule_database.CheckQuery(query);
					if (rule!=null)
						list.Add(rule.Link);
				}
				else
					list.Add(d);
			}	
		}
		return list;
	}
	
	int y_off=0;

	void AddAnswer(DialogueData data){
		
		var go=Instantiate(answer_button_prefab,Vector3.zero,Quaternion.identity) as GameObject;
		var ab=go.GetComponent<AnswerButtonMain>();
		
		go.transform.parent=answer_buttons_parent.transform;
		go.transform.localPosition=Vector3.down*y_off;
		
		if (data.Type=="RANDOM"){
			ab.SetData(data.ParseText(CurrentQuery),data.GetRandom());
		}
		else{
			ab.SetData(data.ParseText(CurrentQuery),data);
		}
		
		ab.Base.appear();
		ab.ButtonMessage.target=gameObject;
		
		y_off+=(int)ab.y_size+16;
		
		answer_buttons.Add(ab);
	}

    public void CheckQuery(QueryData query)
    {
        var data = core_database.rule_database.CheckQuery(query);
		
        if (data != null)
            OpenDialogue(data.Link,query);
        else
        {
            Debug.Log("Query " + query + " yielded no result");
        }
    }

    public void SelectLink(DialogueData link)
    {
        if (link.LinksToRule)
        {
			//Dev. broken target finder should check every target in area not just one?
            EntityData target=null;
			foreach (var c in CurrentQuery.Location.Characters){
                if (c.Type==link.ToEntity){
                    target=c;
                    break;
                }
            }
			if (target==null){
				foreach (var c in CurrentQuery.Location.Objects){
					if (c.Type==link.ToEntity){
						target=c;
						break;
					}
				}
			}
			var q=new QueryData(CurrentQuery.Location,CurrentQuery.Actor,target,link.ToEvent);
            CheckQuery(q);
        }
        else {
           //Direct link -> just playout the dialogue
			ChangeDialogue(link.Links[0],CurrentQuery);
        }
    }
	
	//graphics

	void ClearAnswers(){
		answer_buttons.Clear();
		y_off=0;
		
		int c=answer_buttons_parent.transform.childCount;
		for (int i=0;i<c;i++){
			NGUITools.Destroy(answer_buttons_parent.transform.GetChild(0).gameObject);
		}
	}

	void AnswerButtonPressed(AnswerButtonMain ans){
		if (OnAnswerButtonPressedEvent!=null)
			OnAnswerButtonPressedEvent(ans);
		
		if (ans.Data.Type=="ENDL"){
			StopDialogue();
			ClearAnswers();
			return;
		}
		
		//DEV.temp disabled. realtime dialogue
		//SelectLink(ans.Data);
	}
	
	void OnAnswerButtonClick(GameObject gameobject){
		AnswerButtonPressed(gameobject.GetComponent<AnswerButtonMain>());
	}
}
