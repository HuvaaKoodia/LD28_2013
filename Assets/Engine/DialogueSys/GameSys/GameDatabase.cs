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
		CharacterGraphics.Add("Freelancer",Resources.Load("FreelancerGraphics")as GameObject);
		CharacterGraphics.Add("Dealer",Resources.Load("DealerGraphics")as GameObject);
		
		tiledata_map=new TileData[mapload.Maps[0].map_data.GetLength(0),mapload.Maps[0].map_data.GetLength(1)];
		
		for (int i=0;i<tiledata_map.GetLength(0);i++){
			for (int j=0;j<tiledata_map.GetLength(1);j++){
				tiledata_map[i,j]=new TileData();
				tiledata_map[i,j].TilePosition=new Vector2(i,j);
			}
		}
	}
	public string[] EventOrder;
	// Use this for initialization
	void Start () {
		planning_turn=true;
		EventOrder=new string[]{"OnAttack","OnArrest","OnRun","OnSearch","OnWalk","OnSell","OnBuy"};
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
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
			if (ai>=0&&bi>=0) break;//both have value
		}
		//is other
		if (ai<0) ai=100;
		if (bi<0) bi=100;
		
		return ai-bi;
	}
	
	public void ActionTurnStart(){
		//calculates movement paths
		CalculateMovements();
		
		//reset movement out from tiles
		foreach(var c in Characters)
		{
			if (c.OnMovingAwayFromTile){
				c.OnMovingAwayFromTile=false;
				c.CurrentAction=null;
			}
		}
		
		foreach (var tile in tiledata_map){
			tile.ActionsThisTurn.Clear();
			List<CharacterActionData> raw_actions=new List<CharacterActionData>();
			if (tile.characters.Count>0){
				//first pass -> gather actions
				foreach (var c in tile.characters){
					if (c.CurrentAction!=null){
						raw_actions.Add(c.CurrentAction);
					}
				}
				//second pass -> sort according to event order
				raw_actions.Sort(SortActions);
			
				//third pass -> interrup actions
				for (int i=0;i<raw_actions.Count;i++){
					var a = raw_actions[i];
					tile.ActionsThisTurn.Add(a);
					if (a.Interrupted) continue;
					
					if (a._Event=="OnAttack"){
						
						
						if (a.Target.CurrentAction._Event=="OnAttack"){
							if (a.Target.CurrentAction.Target==a.Character){
								//both attack each other -> ignore other
								a.IgnoreThis=true;
							}else{
								//target attacks someone else ->don't interrupt
							}
						}
						else
							a.Target.CurrentAction.Interrupted=true;
						
						GameCharacterData victor,loser;
						//calculate winner
						if (Subs.RandomPercent()<50){
							//attacker is victorious
							victor=a.Character;
							loser=a.Target;
						}
						else{
							//defender is victorious
							victor=a.Target;
							loser=a.Character;
						}
						//victory
						tile.ActionsThisTurn.Add(
							new CharacterActionData(
								victor,
								loser,
								"OnVictory",
								new QueryData(new LocationData("TEMP"),victor.Data,loser.Data,"OnVictory")
							)
						);
						//defeat
						tile.ActionsThisTurn.Add(
							new CharacterActionData(
								loser,
								victor,
								"OnDefeat",
								new QueryData(new LocationData("TEMP"),loser.Data,victor.Data,"OnDefeat")	
							)
						);
						
						//DEV.TODO get locationdata from current tile
						
						continue;
					}
					
					if (a._Event=="OnArrest"){
						//arrest interrups any lower order actions
						a.Target.CurrentAction.Interrupted=true;
						continue;
					}
					
					if (a._Event=="OnSearch"){							
						//search interrupts any lower order actions the target might do.
						if (SortActions(a.Character.CurrentAction,a.Target.CurrentAction)<-1){//DEV. check for 0 if two people can search
							a.Target.CurrentAction.Interrupted=true;							
						}
						continue;
					}
					
					if (a._Event=="OnRun"||a._Event=="OnWalk"||a._Event=="OnSearch"){
						//movement action doesn't interrupt other movement actions
						if (a.Target.CurrentAction._Event!=a._Event)
						a.Target.CurrentAction.Interrupted=true;
						
						//it interrupts actions targeting the moving character
						for (int j=i+1;j<raw_actions.Count;j++){
							var a2 = raw_actions[j];
							if (a2.Target==a.Character){
								a2.Interrupted=true;
							}
						}
						continue;
					}
				}
			}
		}
	}
	//EventOrder=new string[]{"OnAttack","OnArrest","OnRun","OnSearch","OnWalk","OnSell","OnBuy"};
	public void ActionTurnEnd(){
		foreach (var c in Characters){
			if (c.CurrentAction!=null){
				if (!c.CurrentAction.Interrupted){
					if (c.CurrentAction._Event=="OnWalk"||c.CurrentAction._Event=="OnRun"){
						c.OnMovingAwayFromTile=true;
					}
				}
			}
		}
	}
	
	public void CalculateMovements(){
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
					
					if (t.HasOtherCharacters(c)){
						
						if (c.CurPos!=c.TempPos){
							//not in staring tile -> stay in this tile
							c.EndPathToTempPos();
							c.EndTempMovement();
						}
						//else can always move away from starting tile
					}
					else{
						//move to next tile
						t.characters.Remove(c);
						
						c.MoveToNextTempPos();
						
						t=c.CurrentTempTile().Data;
						t.characters.Add(c);
						
						if (t.HasOtherCharacters(c)){
							//stop_here
							c.EndPathToTempPos();
							c.EndTempMovement();
						}
						else
						if (c.TempPos==c.CurPos||c.TempPosIsLastPathPos()){
							//still in startpos or in last pos -> moving done
							c.EndTempMovement();
						}
						else{
							//still moving
							chars_still_moving=true;
						}
					}
				}
			}
			
			if (!chars_still_moving)
				break;
		}
	}
	
	public void PlanningTurnStart(){
		
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
				ActionTurnStart();
			}
			else {
				ActionTurnEnd();
				PlanningTurnStart();
			}
			
			current_player_index=0;
		}
		
		CurrentCharacter=Characters[current_player_index++];
		CurrentTileData=CurrentCharacter.CurrentTempTile().Data;
	}
}
