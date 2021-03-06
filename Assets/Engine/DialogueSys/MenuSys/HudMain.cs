﻿using UnityEngine;
using System.Collections;
using DialogueSystem;

public class HudMain : MonoBehaviour {

	public GameOverHud go_hud;
	public BackToMenuMain back_to_menu;
	public UIButton BackToMapButton,ContinueButton;
	
	public PlayerHud PlayerHud_;
	public TurnReportPanel Turn_Report_Panel;
	
	public System.Action OnBackToMapPressedEvent,OnContinuePressedEvent;
	
	// Use this for initialization
	void Start () {}
	
	public void ShowBackToMapButton(bool show)
	{
		BackToMapButton.gameObject.SetActive(show);
	}
	
	public void ShowContinueButton(bool show)
	{
		ContinueButton.gameObject.SetActive(show);
	}
	
	void ContinueButtonPressed(){
		
		if (OnContinuePressedEvent!=null)
			OnContinuePressedEvent();
	}
	
	void BackToMapButtonPressed(){
		ShowBackToMapButton(false);
		if (OnBackToMapPressedEvent!=null)
			OnBackToMapPressedEvent();
	}
	
	//addition text panels
	int x_off=10,y_off=10;
	
	public Transform TextPanelParent;
	public GameObject TextPanelPrefab;
	
	public void AddActionDataTextPanel(DialogueData data,QueryData query){
		
		AddActionDataTextPanel(data.ParseText(query));
	}
	
	public void AddActionDataTextPanel(string text){
		var go=Instantiate(TextPanelPrefab,Vector3.zero,Quaternion.identity) as GameObject;
		var ab=go.GetComponent<AnswerButtonMain>();
		
		go.transform.parent=TextPanelParent;
		go.transform.localPosition=new Vector3(x_off,y_off,0);
		
		ab.SetText(text);
		ab.Base.appear();
		
		var uir=GetComponent<UIRoot>();
		var i =uir.GetPixelSizeAdjustment(Screen.height);
		var width=Screen.width*i;
		
		x_off+=(int)ab.x_size+16;
		
		if (x_off+20>width)
		{
 			x_off=10 ;
			y_off-=40;
		}
	}
	
	public void ClearActionDataPanels(){
		x_off=10;
		y_off=0;
		int c=TextPanelParent.transform.childCount;
		for (int i=0;i<c;i++){
			NGUITools.Destroy(TextPanelParent.transform.GetChild(0).gameObject);
		}
	}
}
