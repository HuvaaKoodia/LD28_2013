using UnityEngine;
using System.Collections;
using DialogueSystem;

public class AnswerButtonMain : MonoBehaviour {
	
	public SpeechBubbleMain Base;
	public DialogueData Data;
	public UIButtonMessage ButtonMessage;
	
	public int x_size=300,y_size;
	
	// Use this for initialization
	void Awake () {
		x_size=300;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetData(string text,DialogueData data){
		Data=data;
		
		Base.text_label.text=Subs.autofit_text(text,x_size,Base.text_label.bitmapFont);
		var size=Base.text_label.bitmapFont.CalculatePrintedSize(Base.text_label.text);
		
		//x_size=(int)size.x;
		y_size=6+(int)size.y+6;
		Base.sprite.width=310;
		Base.sprite.height=y_size;
		Base.GetComponent<BoxCollider>().size=new Vector3(310,y_size,0);
		Base.GetComponent<BoxCollider>().center=new Vector3(310/2,-y_size/2,0);
	}
	
	
}
