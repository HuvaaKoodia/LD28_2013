using UnityEngine;
using System.Collections;

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
}
