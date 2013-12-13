using UnityEngine;
using System.Collections;
 
class Sprite_DestroyOnEnd : MonoBehaviour
{   
#pragma warning disable
	public Sprite_ spr;
	
    void Start(){
		spr.on_last=Destroy;
	}
	
	void Destroy(){
		Destroy(gameObject);
	}
}