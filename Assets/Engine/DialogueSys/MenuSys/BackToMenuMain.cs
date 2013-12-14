using UnityEngine;
using System.Collections;

public delegate void OnPauseMenuToggle(bool on);

public class BackToMenuMain : MonoBehaviour {
	
	public GameObject Menu;
	public OnPauseMenuToggle OnToggle;
	
	void OnYes(){
		Time.timeScale=1;
		Application.LoadLevel("MainMenuScene");
	}
	
	void OnNo(){
		ToggleMenu();
	}
	
	public void ToggleMenu(){
		if (OnToggle!=null){
			OnToggle(!Menu.activeSelf);
		}
		
		if (Menu.activeSelf){
			Menu.SetActive(false);
			Time.timeScale=1;
			return;
		}
		Menu.SetActive(true);
		Time.timeScale=0.00000001f;
	}
	
	public bool IsOn(){
		return Menu.activeSelf;
	}
}
