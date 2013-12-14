using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {
	
	public GameObject CreditsPanel;
	public GameObject HelpPanel;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void PlayPressed(){
		Application.LoadLevel("GameScene");
	}
	
	void CreditsPressed(){
		if (CreditsPanel.activeSelf)
			CreditsPanel.SetActive(false);
		else{
			CreditsPanel.SetActive(true);
			HelpPanel.SetActive(false);
		}
	}
	
	void ControlsPressed(){
		if (HelpPanel.activeSelf)
			HelpPanel.SetActive(false);
		else{
			CreditsPanel.SetActive(false);
			HelpPanel.SetActive(true);
		}
	}
}
