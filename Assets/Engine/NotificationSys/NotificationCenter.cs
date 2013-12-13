using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace NotificationSys{
	
	public delegate void OnNotificationDelegate( Notification note );
	
	//singleton
	public class NotificationCenter
	{
	    private static NotificationCenter instance;
	    public static NotificationCenter Instance{get {return instance;}}
	
		private Dictionary<NotificationType,List<OnNotificationDelegate>> listeners=new Dictionary<NotificationType, List<OnNotificationDelegate>>();
		
		public static void resetInstance(){
			instance=new NotificationCenter();
		}
		
	    private NotificationCenter()
	    {
			//Fill dictionary with empty lists. Not lazy. Problems?
			foreach (NotificationType t in Enum.GetValues(typeof(NotificationType))){
				listeners.Add(t,new List<OnNotificationDelegate>());
			}
	    }
		
	    public void addListener( OnNotificationDelegate newListenerDelegate, NotificationType type )
	    {
	        listeners[type].Add(newListenerDelegate);
	    }
	
	    public void removeListener( OnNotificationDelegate listenerDelegate, NotificationType type )
	    {
	        listeners[type].Remove( listenerDelegate );
	    }
	
	    public void sendNotification( Notification note )
	    {
	        foreach(var delegateCall in listeners[note.type] )
	        {
	            delegateCall( note );
	        }
	    }
	}
	
	// Standard notification class.  For specific needs subclass
	public class Notification
	{
	    public NotificationType type;
	    public object Data;
	
	    public Notification( NotificationType type )
	    {
	        this.type = type;
	    }
	
	
	    public Notification( NotificationType type, object Data)
	    {
	        this.type = type;
	        this.Data = Data;
	    }
	}
}