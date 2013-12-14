using UnityEngine;
using System.Collections;

public class StartScaler : MonoBehaviour {
	
	public Transform Target;
	public float speed_multi=0.1f;
	bool starting=false,ending=false;
	
	public float min=0.001f;
	public bool destroy_when_shrunk=true,linear_add=true,enlarge_in_start=true;
	// Use this for initialization
	void Start () {
		setToMin();
		if(enlarge_in_start){
			Enlarge();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Target==null){
			return;
		}
		if (starting){
			if (Target.localScale.y<1){
				if (linear_add)
					Target.localScale+=Vector3.up*Time.deltaTime*speed_multi;
				else
					Target.localScale+=Vector3.up*(1-Target.localScale.y)*Time.deltaTime*speed_multi;
			}
			else{
				starting=false;
				setToMax();
			}
		}
		else if (ending){
			if (Target.transform.localScale.y>min){
				if (linear_add)
					Target.localScale-=Vector3.up*Time.deltaTime*speed_multi;
				else
					Target.localScale-=Vector3.up*(Target.localScale.y)*Time.deltaTime*speed_multi;
					
			}
			else{
				ending=false;
				setToMin();
				if (destroy_when_shrunk)
					Destroy(gameObject);
			}
		}
	}
	
	public void Enlarge ()
	{
		setToMin();
		Target.gameObject.SetActive(true);
		starting=true;
	}
	
	public void Shrink ()
	{
		setToMax();
		ending=true;
	}

	public void stopAll ()
	{
		starting=ending=false;
	}

	public void setToMin()
	{
		Target.gameObject.SetActive(false);
		Target.localScale=new Vector3(1,min,1);
	}
	public void setToMax()
	{
		Target.localScale=new Vector3(1,1,1);
	}
}
