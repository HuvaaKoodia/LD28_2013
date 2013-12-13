using UnityEngine;
using System.Collections;
using NotificationSys;

public class ReceiveExplosions : MonoBehaviour {
	
	public OnNotificationDelegate onExplosion;
	// Use this for initialization
	void Start () {
		NotificationCenter.Instance.addListener(OnExplosion,NotificationType.Explode);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnExplosion(Notification note){
		var exp=(Explosion_note)note;
		rigidbody.AddExplosionForce(exp.Force,exp.Position,exp.Radius);
	}
	
	void OnDestroy(){
		NotificationCenter.Instance.removeListener(OnExplosion,NotificationType.Explode);
	}
}
