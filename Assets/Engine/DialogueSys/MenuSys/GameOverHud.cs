using UnityEngine;
using System.Collections;

public class GameOverHud : MonoBehaviour {
	
	public UIPanel This;
	public UISprite sprite;
	public UILabel gameover_label,desc_label,other_label;
	
	// Use this for initialization
	void Start (){
		This.alpha=0;
		sprite.transform.localScale=new Vector3(Screen.width*2+64,Screen.height*2+64,0);
	}
	
	// Update is called once per frame
	void Update () {}
	
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
