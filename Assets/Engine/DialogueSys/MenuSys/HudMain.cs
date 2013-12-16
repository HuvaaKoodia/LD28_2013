using UnityEngine;
using System.Collections;
using DialogueSystem;

public class HudMain : MonoBehaviour {

	public GameOverHud go_hud;
	public BackToMenuMain back_to_menu;
	public UIButton BackToMapButton;
	
	public System.Action OnBackToMapPressedEvent;
	
	// Use this for initialization
	void Start () {
		ShowBackToMapButton(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ShowBackToMapButton(bool show)
	{
		BackToMapButton.gameObject.SetActive(show);
	}
	
	void BackToMapButtonPressed(){
		
		ShowBackToMapButton(false);
		if (OnBackToMapPressedEvent!=null)
			OnBackToMapPressedEvent();
		
		Application.LoadLevel("MapGameScene");
	}
	
	//addition text panels
	int x_off=10;
	
	public Transform TextPanelParent;
	public GameObject TextPanelPrefab;
	
	public void AddActionDataTextPanel(DialogueData data,QueryData query){
		var go=Instantiate(TextPanelPrefab,Vector3.zero,Quaternion.identity) as GameObject;
		var ab=go.GetComponent<AnswerButtonMain>();
		
		go.transform.parent=TextPanelParent;
		go.transform.localPosition=Vector3.right*x_off;
		
		ab.SetData(data.ParseText(query),data);
		
		ab.Base.appear();
		
		x_off+=(int)ab.x_size+16;
	}
	
	public void ClearActionDataPanels(){
		x_off=0;
		int c=TextPanelParent.transform.childCount;
		for (int i=0;i<c;i++){
			NGUITools.Destroy(TextPanelParent.transform.GetChild(0).gameObject);
		}
	}
}
