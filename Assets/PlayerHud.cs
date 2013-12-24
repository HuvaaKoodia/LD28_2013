using UnityEngine;
using System.Collections;

public class PlayerHud : MonoBehaviour {
	
	public UILabel PlayerName,PlayerStats;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void SetPlayer(string playername,CharacterData data)
	{
		PlayerName.text=playername+"\n"+data.Facts.GetString("Name");
		PlayerStats.text=
			"VictoryPoints: "+data.Facts.GetFloat("VP")+"\n"+
			"Money: "+data.Facts.GetFloat("Money")+"\n"+
			"Drugs: "+data.Facts.GetFloat("Drugs")+"\n"+
			"";
	}
}
