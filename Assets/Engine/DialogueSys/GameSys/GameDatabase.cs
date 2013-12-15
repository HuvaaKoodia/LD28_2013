using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameDatabase : MonoBehaviour {
	
	public CoreDatabase Core;
	public MapLoader mapload;
	
	public GameCharacterData CurrentCharacter;
	public List<GameCharacterData> Characters=new List<GameCharacterData>();
	
	public int current_player_index=0;
	
	public Dictionary<string,GameObject> CharacterGraphics=new Dictionary<string, GameObject>();
	
	public bool planning_turn=true,action_turn;
	
	public TileData[,] tiledata_map;
	public TileData CurrentTileData;
	
	void Awake(){
		CharacterGraphics.Add("Policeman",Resources.Load("PoliceGraphics") as GameObject);
		CharacterGraphics.Add("Junkie",Resources.Load("BumGraphics")as GameObject);
		CharacterGraphics.Add("CallGirl",Resources.Load("CallGirlGraphics")as GameObject);
		CharacterGraphics.Add("Politician",Resources.Load("PoliticianGraphics")as GameObject);
		
		tiledata_map=new TileData[mapload.Maps[0].map_data.GetLength(0),mapload.Maps[0].map_data.GetLength(1)];
		
		for (int i=0;i<tiledata_map.GetLength(0);i++){
				
			for (int j=0;j<tiledata_map.GetLength(1);j++){
				tiledata_map[i,j]=new TileData();
				tiledata_map[i,j].TilePosition=new Vector2(i,j);
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		planning_turn=true;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public string[] EventOrder=new string[]{"OnAttack","OnArrest","OnRun","OnInterrogate","OnLeave","OnSell","OnBuy"};
	
	private int SortActions(CharacterActionData a,CharacterActionData b){
		int ai=-1,bi=-1;
		for (int i=0;i<EventOrder.Length;i++){
			var e = EventOrder[i];
			if (a._Event==e){
				ai=i;
			}
			if (b._Event==e){
				bi=i;
			}
			if (ai>0&&bi>0) break;//both have value
		}
		//is other
		if (ai<0) ai=100;
		if (bi<0) bi=100;
		
		return ai-bi;
	}
	
	public void CalculateActionsAll(){
		foreach (var t in tiledata_map){
			t.ActionsThisTurn.Clear();
			if (t.characters.Count>0){
				foreach (var c in t.characters){
					if (c.CurrentAction!=null){
						t.ActionsThisTurn.Add(c.CurrentAction);
					}
				}
				//
				t.ActionsThisTurn.Sort(SortActions);
				int i=0;
			}
		}
	}
	
	public void CalculateMovementsAll(){
		foreach(var c in Characters)
		{
			c.StarTempMovement();	
		}
		while(true){
			bool chars_still_moving=false;
			foreach(var c in Characters)
			{
				if (c.TempMovement){
					var t=c.CurrentTempTile().Data;
					
					if (c.TempPos!=c.CurPos&&t.HasOtherCharacters(c)){
						//stop_here
						c.EndPathToTempPos();
						c.EndTempMovement();
					}
					else{
						t.characters.Remove(c);
						
						c.MoveToNextTempPos();
						
						t=c.CurrentTempTile().Data;
						t.characters.Add(c);
						chars_still_moving= true;
					}
				}
			}
			
			if (!chars_still_moving)
				break;
			
			foreach(var c in Characters)
			{
				if (c.TempMovement){
					
					var t=c.CurrentTempTile();
					
					if (c.TempPos==c.CurPos){
						c.EndTempMovement();
					}
					
					//whole path traversed
					if (c.OnTheMove&&c.TempPosIsLastPathPos()){
						c.EndTempMovement();
					}
				}
			}
		}
	}
	
	public void ResetDataAll(){
		foreach(var c in Characters)
		{
			c.SetToPathEnd();
			c.Path_positions.Clear();
			c.EndMoving();
		}
	}
	
	public void MoveAll(){
		foreach(var c in Characters)
		{
			c.StartMoving();
		}
	}
	
	public bool AllMoved(){
		foreach(var c in Characters)
		{
			if (c.Main.Moving)
				return false;
		}
		return true;
	}

	public void NextPlayersTurn ()
	{				
		if (current_player_index+1>Characters.Count){
			
			planning_turn=!planning_turn;
			action_turn=!action_turn;
			
			if (action_turn){
				//calculate temporary effects for action playback
				CalculateMovementsAll();
				CalculateActionsAll();
			}
			else {
				//reset data for the next planning turn
				ResetDataAll();
			}
			
			current_player_index=0;
		}
		
		CurrentCharacter=Characters[current_player_index++];
		CurrentTileData=CurrentCharacter.CurrentTempTile().Data;
	}
}
