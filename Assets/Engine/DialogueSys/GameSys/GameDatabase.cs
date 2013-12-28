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
	
	//location data
	TileData PoliceStationTile;
	
	void Awake(){
		CharacterGraphics.Add("Policeman",Resources.Load("PoliceGraphics") as GameObject);
		CharacterGraphics.Add("Junkie",Resources.Load("BumGraphics")as GameObject);
		CharacterGraphics.Add("CallGirl",Resources.Load("CallGirlGraphics")as GameObject);
		CharacterGraphics.Add("Politician",Resources.Load("PoliticianGraphics")as GameObject);
		CharacterGraphics.Add("Freelancer",Resources.Load("FreelancerGraphics")as GameObject);
		CharacterGraphics.Add("Dealer",Resources.Load("DealerGraphics")as GameObject);
		
		MapData md = mapload.Maps[0];
		
		int w=md.map_data.GetLength(0);
		int h=md.map_data.GetLength(1);
		
		tiledata_map=new TileData[w,h];
		
		for (int i=0;i<w;i++){
			for (int j=0;j<h;j++){
				tiledata_map[i,j]=new TileData();
				tiledata_map[i,j].TilePosition=new Vector2(i,j);
				
				string LocationName="Street";
				switch (md.map_data[i,j])
				{
					case "p":
						LocationName="PoliceStation";
						PoliceStationTile=tiledata_map[i,j];
					break;
					case "a":
						LocationName="Alley";
					break;
					case "n":
						LocationName="NewsStation";
					break;
					case "u":
						LocationName="DrugStash";
					break;
					case "c":
						LocationName="CityHall";
					break;
					case "f":
						LocationName="FoXXXy";
					break;
				}
				tiledata_map[i,j].SetLocation(new LocationData(LocationName));//DEV. TODO. load location data from locationdatabase
			}
		}
	}
	public string[] EventOrder;
	// Use this for initialization
	void Start () {
		planning_turn=true;
		EventOrder=new string[]{"OnAttack","OnArrest","OnRun","OnSearch","OnWalk","OnSell","OnBuy","OnWait","OnStealActor"};
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	private int SortActions(CharacterActionData a,CharacterActionData b){
		if (a==null) return -1;
		if (b==null) return 1;
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
		if (ai<0) ai=10000;
		if (bi<0) bi=10000;
		
		return ai-bi;
	}
	
	public void ActionTurnStart(){
		CalculateMovements();
		
		//reset data & handle turn start variables
		foreach(var c in Characters)
		{						
			c.RecoverStun(1);
		}
		
		foreach (TileData tile in tiledata_map){
			tile.ActionsThisTurn.Clear();
			List<CharacterActionData> raw_actions=new List<CharacterActionData>();
			if (tile.GameCharacters.Count>0){
				//first pass -> gather actions
				foreach (var c in tile.GameCharacters){
					if (c.CurrentAction!=null){
						raw_actions.Add(c.CurrentAction);
					}
				}
				//second pass -> sort according to event order
				raw_actions.Sort(SortActions);
			
				//third pass -> action consequences & relay information
				for (int i=0;i<raw_actions.Count;i++){
					var a = raw_actions[i];
					tile.ActionsThisTurn.Add(a);
   					if (a.Character.IsStunned()){
						a.Stunned=true;
					}
					if (a.Interrupted||a.Stunned||a.IgnoreThis) continue;
					
					if (a._Event=="OnAttack"){
						
						if (a.Target.IsStunned()){
							//already stunned don't attack
							a.IgnoreThis=true;
							continue;
						}
						else
						if (a.Target.CurrentAction._Event=="OnAttack"){
							if (a.Target.CurrentAction.Target==a.Character){
								//both attack each other -> ignore other
								a.Target.CurrentAction.IgnoreThis=true;
							}else{
								//target attacks someone else-> don't interrupt, maybe they win and get to fight again
							}
						}
						
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
						
						loser.Stun(1);
						
						//victory event
						tile.ActionsThisTurn.Add(
							new CharacterActionData(
								victor,
								loser,
								"OnVictory",
								new QueryData(tile.Location,victor.Data,loser.Data,"OnVictory")
							)
						);
						//defeat event
						tile.ActionsThisTurn.Add(
							new CharacterActionData(
								loser,
								victor,
								"OnDefeat",
								new QueryData(tile.Location,loser.Data,victor.Data,"OnDefeat")	
							)
						);
						continue;
					}
					
					if (a._Event=="OnArrest"){
						//arrest interrups any lower order actions
						a.Target.CurrentAction.Interrupt(a.Character);						
						continue;
					}
					
					if (a._Event=="OnSearch"){							
						//search interrupts any lower order actions the target might do.
						if (SortActions(a.Character.CurrentAction,a.Target.CurrentAction)<0){//DEV. check for 0 if two people can search
							if (a.Target.CurrentAction._Event=="OnWait"){
								a.Target.CurrentAction.IgnoreThis=true;
							}else{
								a.Target.CurrentAction.Interrupt(a.Character);
							}
						}
						continue;
					}
					
					if (a._Event=="OnRun"||a._Event=="OnWalk"){
						
						//interrupts lower order actions targeting the moving character
						for (int j=i+1;j<raw_actions.Count;j++){
							var a2 = raw_actions[j];
							if (a2.Target==a.Character){
								//movement action doesn't interrupt other movement actions
								if (a2._Event!="OnRun"&&a2._Event!="OnWalk"&&a2._Event!="OnWait"){
									a2.Interrupt(a.Character);
								}
							}
						}
						continue;
					}
					
					if (a._Event=="OnStealActor"){
						//just do it?
						
						continue;
					}
				}
			}
		}
	}

	public void ActionTurnEnd(){
		
		//action effects
		foreach (var c in Characters){
			if (c.OnMovingAwayFromTile){
				c.OnMovingAwayFromTile=false;
				c.CurrentTile().Data.AddCharacter(c);
			}
								
			//c.RecoverStun(1);
			c.RecoverArrest(1);

			if (c.CurrentAction!=null){
				if (!c.CurrentAction.Interrupted){

					if (c.CurrentAction._Event=="OnWalk"||c.CurrentAction._Event=="OnRun"){
						c.OnMovingAwayFromTile=true;
						c.CurrentTile().Data.RemoveCharacter(c);
					}
					
					//effect
					var r=Core.rule_database.CheckQuery(new QueryData(
						c.CurrentAction.Query.Location,
						c.CurrentAction.Character.Data,
						c.CurrentAction.Target.Data,
						c.CurrentAction._Event+"Effect"
					));
					
					if (c.CurrentAction._Event=="OnArrest"){
						//and lock the target up
						c.CurrentAction.Target.Arrest(3);
						c.CurrentAction.Target.MoveToPosition(PoliceStationTile);
					}
					
				}
				c.CurrentAction=null;
			}
		}
	}
	
	public void CalculateMovements(){
		foreach(var c in Characters)
		{
			c.StarMovement();
		}
		while(true){
			bool chars_still_moving=false;
			foreach(var c in Characters)
			{
				if (c.TempMovement){
					var t=c.CurrentTile().Data;
					
					if (t.HasOtherCharacters(c)){
						if (c.OnMovingAwayFromTile){
							//can move away no matter what
						}
						else{
							
							if (c.CurrentPos==c.TurnStartPos){
								bool stopped=false;
								foreach(var c2 in t.GameCharacters)
								{
									if (c2.OldPos==c.NextPos()){
										//other character comes from characters destination -> stay in this tile
										c.EndPathToCurrentPos();
										c.EndMovement();
										stopped=true;
									}
								}
								if (stopped)
									continue;
							}
							else{
								c.EndPathToCurrentPos();
								c.EndMovement();
								continue;
							}
						}					
					}
					//move to next tile
					t.RemoveCharacter(c);
					c.OldPos=t.TilePosition;
					
					c.MoveToNextTempPos();
					
					t=c.CurrentTile().Data;
					t.AddCharacter(c);
					
					if (c.CurrentPos==c.TurnStartPos||c.CurrentPosIsLastPathPos()){
						//still in startpos or in last pos -> moving done
						c.EndPathToCurrentPos();
						c.EndMovement();
					}
					else{
						//still moving
						chars_still_moving=true;
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
			c.SetTurnStartPosToPathEnd();
			c.Path_positions.Clear();

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
		CurrentTileData=CurrentCharacter.CurrentTile().Data;
	}
}
