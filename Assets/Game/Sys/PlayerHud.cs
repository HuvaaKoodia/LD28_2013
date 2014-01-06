using UnityEngine;
using System.Collections;

public class PlayerHud : MonoBehaviour {
	
	public UIPanel Panel;
	public UILabel PlayerName,PlayerStats;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void ShowPanel(bool show){
		Panel.gameObject.SetActive(show);
	}
	
	public void SetPlayer(GameCharacterData GC)
	{
		var data=GC.Data;
		PlayerName.text=GC.Name+"\n"+data.Facts.GetString("Name")+
		"\nMovement points: "+GC.GetMovementSpeed();
		var text="";
		
		text+=
			"VictoryPoints: "+data.Facts.GetFloat("VP")+"\n"+
			"Money: "+data.Facts.GetFloat("Money")+"\n"+
			"Drugs: "+data.Facts.GetFloat("Drugs")+"\n"+
			"\n";
		if (GC.CurrentMovementType==CharacterMovementType.Hiding){
			text+="HIDING";
			text+="\n";
		}
		else if (GC.CurrentMovementType==CharacterMovementType.Running){
			text+="SIRENS ON";
			text+="\n";
		}
		
		PlayerStats.text=text;
	}
}
