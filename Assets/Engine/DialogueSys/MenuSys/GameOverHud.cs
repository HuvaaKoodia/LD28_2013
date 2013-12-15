using UnityEngine;
using System.Collections;

public class GameOverHud : MonoBehaviour {
	
	public UIPanel This;
	public UISprite sprite;
	public UILabel gameover_label,desc_label,other_label;
	
	// Use this for initialization
	void Start (){
		This.alpha=0;
		sprite.width=Screen.width*2+64;
		sprite.height=Screen.height*2+64;
	}
	
	// Update is called once per frame
	void Update () {}
	
	public void SetText(string text,bool movement_phase){
		This.alpha=1;
		desc_label.text=text;
		
		if (movement_phase){
			gameover_label.text="Movement phase";
		}
		else
		{
			gameover_label.text="Action phase";
		}
	}
	
	public void RemoveText(){
		This.alpha=0;
	}
	
	public void GAMEOVER(string description){
		StartCoroutine(FadeAlpha());
		desc_label.text=description;
	}
	
	IEnumerator FadeAlpha(){
		
		while(true){
			if (This.alpha>=1){
				This.alpha=1;
				break;	
			}
			
			yield return new WaitForSeconds(Time.deltaTime);
		
			This.alpha+=0.01f;
			
		}
	}
}
