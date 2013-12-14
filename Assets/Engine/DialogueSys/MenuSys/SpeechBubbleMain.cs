using UnityEngine;
using System.Collections;

public class SpeechBubbleMain : MonoBehaviour {

	public UILabel text_label;
	public UISprite sprite;
	public StartScaler scaler;
	
	public float appear_speed,disappear_speed;
	
	float start_y,start_x;
	// Use this for initialization
	void Awake() {
		
		scaler.stopAll();
		scaler.destroy_when_shrunk=false;
		scaler.linear_add=false;
		scaler.speed_multi=2;
	}
	
	void Start(){
		start_x=Screen.width/2-sprite.transform.localScale.x/2-20;
		start_y=Screen.height/2-sprite.transform.localScale.y/2-20;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void setText (string text)
	{
		text_label.text=Subs.autofit_text(text,(int)sprite.width,text_label.font);
	}
	
	public void appear()
	{
		scaler.speed_multi=appear_speed;
		scaler.Enlarge();
	}
	public void disappear ()
	{
		scaler.speed_multi=disappear_speed;
		scaler.Shrink();
	}
	
	public void setPositionRelative(int x_s,int y_s){
		transform.localPosition=new Vector3(x_s*start_x,y_s*start_y,0);
	}
	
	public void setPosition(int x_s,int y_s){
		transform.localPosition=new Vector3(x_s,y_s,0);
	}
	
	public void setPosition(Vector3 pos){
		transform.localPosition=pos;
	}
}
