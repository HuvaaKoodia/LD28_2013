using UnityEngine;
using System.Collections;

public class TurnReportPanel : MonoBehaviour {

	public UIPanel Panel;
	public UILabel StatsLabel;
	
	public bool TurnReportPanelOn{get{return Panel.alpha==1;}}
	
	
	public void SetStats(GameCharacterData character){
		var text="";
		
		int points=(int)(character.Data.Facts.GetFloat("VP")-character.VP_TurnStart);
		if (points!=0)
			text+="Victory Points: "+points+"\n";
		
		points=(int)(character.Data.Facts.GetFloat("Money")-character.Money_TurnStart);
		if (points!=0)
			text+="Money: "+points+"\n";
		
		points=(int)(character.Data.Facts.GetFloat("Drugs")-character.Drugs_TurnStart);
		if (points!=0)
			text+="Drugs: "+points+"\n";
		
		StatsLabel.text=text;
		
		if (text!=""){
			Panel.alpha=1;
		}
	}

	public void ClosePanel(){
		Panel.alpha=0;
	}

}