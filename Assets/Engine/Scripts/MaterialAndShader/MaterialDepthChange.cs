using UnityEngine;
using System.Collections;

public class MaterialDepthChange : MonoBehaviour {
	
	public bool RealtimeUpdate=false;
	public int QueueDepth=-1;
	public int RelativeQueueDepth=0;

	void Start () {
		UpdateDepth();
	}
	
	void Update(){
		if (RealtimeUpdate){
			UpdateDepth();
		}
	}
	
	public void UpdateDepth(){
		if (QueueDepth>=0)
			renderer.material.renderQueue= QueueDepth;
		renderer.material.renderQueue+=RelativeQueueDepth;
	}

}
