using UnityEngine;
using System.Collections;

public class PlayerHud : MonoBehaviour {
	
	public UILabel PlayerName,PlayerStats;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void SetPlayer(GameCharacterData GC)
	{
		var data=GC.Data;
		PlayerName.text=GC.Name+"\n"+data.Facts.GetString("Name");
		var text=
			"VictoryPoints: "+data.Facts.GetFloat("VP")+"\n"+
			"Money: "+data.Facts.GetFloat("Money")+"\n"+
			"Drugs: "+data.Facts.GetFloat("Drugs")+"\n"+
			"\n";
		if (GC.CurrentMovementType==CharacterMovementType.Hiding){
			text+="HIDING";
		}
		else if (GC.CurrentMovementType==CharacterMovementType.Running){
			text+="SIRENS ON";
		}
		PlayerStats.text=text;
	}
}
