using UnityEngine;

namespace NotificationSys{
	// Each notification type should gets its own enum
	public enum NotificationType {
		Explode,
		CameraZoom,
		HaxKnockback
	};
	
	
// Standard notification class. For specific needs subclass
	public class Explosion_note:Notification
	{
	    public Vector3 Position;
		public float Force,Radius;
		
	    public Explosion_note(Vector3 position,float force,float radius):base(NotificationType.Explode)
	    {
			Position=position;
			Force=force;
			Radius=radius;
	    }
		
		public void addForce(Rigidbody rbody){
			
			rbody.AddExplosionForce(Force,Position,Radius);
		}
	}
	
	public class CameraZoom_note:Notification
	{
	    public float Amount;
		public Transform Target;
		public bool haxhax_middlezoom_haxhax=false;
		
	    public CameraZoom_note(float amount):base(NotificationType.CameraZoom)
	    {
			Amount=amount;
	    }
		 public CameraZoom_note(float amount,Transform target):base(NotificationType.CameraZoom)
	    {
			Amount=amount;
			Target=target;
	    }
		
		public CameraZoom_note(float amount,bool middlehax):base(NotificationType.CameraZoom)
	    {
			Amount=amount;
			haxhax_middlezoom_haxhax=middlehax;
	    }
	}
}