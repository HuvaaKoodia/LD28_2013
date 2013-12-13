using UnityEngine;
using System.Collections;
using NotificationSys;

public class EngineController : MonoBehaviour {
	
	public bool enable_Restart=true,enable_Quit=true;
	
	// Use this for initialization
	void Awake () {
		Timer.clearTimers();
		NotificationCenter.resetInstance();
	}
	
	//Update is called once per frame
	void Update (){
		Timer.UpdateTimers();

		if (enable_Restart&&Input.GetKeyDown(KeyCode.R)){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (enable_Quit&&Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}
}
