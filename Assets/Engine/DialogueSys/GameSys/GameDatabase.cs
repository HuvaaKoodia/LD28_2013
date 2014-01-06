using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameDatabase : MonoBehaviour {
	
	public CoreDatabase Core;
	public MapLoader mapload;
	public GameController GC;
	
	public GameCharacterData CurrentCharacter{get;private set;}
	public List<GameCharacterData> Characters=new List<GameCharacterData>();
	
	public int current_player_index=0;
	
	public Dictionary<string,GameObject> CharacterGraphics=new Dictionary<string, GameObject>();
	
	public bool planning_turn=true,action_turn;
	
	public TileData[,] tiledata_map{get;private set;}
	public TileData CurrentTileData;
	
	public int MapWidth{get{return tiledata_map.GetLength(0);}}
	public int MapHeight{get{return tiledata_map.GetLength(1);}}
	
	//location data
	TileData PoliceStationTile,AlleyTile,FoXXXyTile,DrugStashTile,CityHallTile,NewsStationTile;

	public TileData GetTile(Vector2 pos)
	{
		return tiledata_map[(int)pos.x,(int)pos.y];
	}
	
	void Awake(){
		//graphics assets
		CharacterGraphics.Add("Policeman",Resources.Load("PoliceGraphics") as GameObject);
		CharacterGraphics.Add("Junkie",Resources.Load("BumGraphics")as GameObject);
		CharacterGraphics.Add("CallGirl",Resources.Load("CallGirlGraphics")as GameObject);
		CharacterGraphics.Add("Politician",Resources.Load("PoliticianGraphics")as GameObject);
		CharacterGraphics.Add("Freelancer",Resources.Load("FreelancerGraphics")as GameObject);
		CharacterGraphics.Add("Dealer",Resources.Load("DealerGraphics")as GameObject);
		
		//creating map
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
						AlleyTile=tiledata_map[i,j];
					break;
					case "n":
						LocationName="NewsStation";
						NewsStationTile=tiledata_map[i,j];
					break;
					case "u":
						LocationName="DrugStash";
						DrugStashTile=tiledata_map[i,j];
					break;
					case "c":
						LocationName="CityHall";
						CityHallTile=tiledata_map[i,j];
					break;
					case "f":
						LocationName="FoXXXy";
						FoXXXyTile=tiledata_map[i,j];
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
	
	public void SetPlayers(int amount){
		
		int temp_index=0;
		foreach (var cg in CharacterGraphics){
			var name=cg.Key;
			++temp_index;
			
			var c=new GameCharacterData("Player "+temp_index,temp_index>amount);
			c.Data=Core.character_database.GetCharacterLazy(name);
			
			//DEV. TEMP. move to XML data + look for the correct tile (random if many tiles);
			TileData t=null;
			if (name=="Policeman")
				t=PoliceStationTile;
			if (name=="CallGirl")
				t=FoXXXyTile;
			if (name=="Politician")
				t=CityHallTile;
			if (name=="Freelancer")
				t=NewsStationTile;
			if (name=="Dealer")
				t=DrugStashTile;
			if (name=="Junkie"){
				List<TileData> tiles=new List<TileData>();
				foreach (var tt in tiledata_map){
					if (tt.Location.Name=="Street"){
						tiles.Add(tt);
					}
				}
				t=Subs.GetRandom(tiles);
			}
			c.SetStartPosition(t);
					
			Characters.Add(c);
			t.AddCharacter(c);
		}
		GameStart();
		
		NextPlayersTurn();
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
					
					if (a.Interrupted||a.IgnoreThis) continue;
					
					if (a._Event=="OnAttack"){
						
						if (a.Target.CurrentAction._Event=="OnStun"){
							//already stunned don't attack
							a.IgnoreThis=true;
						}
						else{
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
							
							tile.ActionsThisTurn.Add(a);
							
							if (a.Character!=loser)
								loser.CurrentAction.IgnoreThis=true;
							
							loser.CurrentAction=new CharacterActionData(loser,victor,"OnStun",new LocationData("ActionTexts"));
							loser.CurrentAction.ShowOnlyForCurrentCharacter=true;
							raw_actions.Add(loser.CurrentAction);
							
							//victory event
							tile.ActionsThisTurn.Add(
								new CharacterActionData(
									victor,
									loser,
									"OnVictory",
									tile.Location
								)
							);
							//defeat event
							tile.ActionsThisTurn.Add(
								new CharacterActionData(
									loser,
									victor,
									"OnDefeat",
									tile.Location
								)
							);
						}
						continue;
					}
					else
					if (a._Event=="OnArrest"){
						//arrest interrups any lower order actions
						a.Target.CurrentAction.Interrupt(a.Character);						
					}
					else
					if (a._Event=="OnSearch"){							
						//search interrupts any lower order actions the target might do.
						if (SortActions(a.Character.CurrentAction,a.Target.CurrentAction)<0){//DEV. check for 0 if two people can search
							if (a.Target.CurrentAction._Event=="OnWait"){
								a.Target.CurrentAction.IgnoreThis=true;
							}else{
								a.Target.CurrentAction.Interrupt(a.Character);
							}
						}
					}
					else
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
					}
					else
					if (a._Event=="OnStealActor"){
						//just do it?
						
					}
					
					if (a.IgnoreThis)
						continue;
					
					tile.ActionsThisTurn.Add(a);
				}
			}
		}
	}
	
	public void GameStart(){
		foreach (var c in Characters){
			c.SetTurnStartValues();
		}
	}
	
	public void ActionTurnEnd(){
		//action effects
		foreach (var c in Characters){
			
			c.SetTurnStartValues();
			
			if (c.OnMovingAwayFromTile){
				c.OnMovingAwayFromTile=false;
				c.CurrentTileData.AddCharacter(c);
			}
								
			c.RecoverStun(1);
			c.RecoverArrest(1);

			if (c.CurrentAction!=null){
				if (!c.CurrentAction.Interrupted){

					if (c.CurrentAction._Event=="OnWalk"||c.CurrentAction._Event=="OnRun"){
						c.OnMovingAwayFromTile=true;
						c.CurrentTileData.RemoveCharacter(c);
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
					
					if (c.CurrentAction._Event=="OnStun"){
						c.Stun(1);
					}
				}
				c.CurrentAction=null;
			}
		}
	}
	
	public void CalculateMovements(){
		foreach(var c in Characters)
		{
			c.StartMovement();
		}
		while(true){
			bool chars_still_moving=false;
			foreach(var c in Characters)
			{
				if (c.TempMovement){
					var t=c.CurrentTileData;
					
					if (c.CurrentMovementType==CharacterMovementType.Normal){
						//check for other characters on path
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
					}
					//move to next tile
					t.RemoveCharacter(c);
					c.OldPos=t.TilePosition;
					
					c.MoveToNextTempPos(this);
					
					t=c.CurrentTileData;
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
		
		foreach(var c in Characters)
		{
			if (c.CurrentMovementType==CharacterMovementType.Running&&c.CurrentTileData.HasOtherCharacters(c)){
				c.ToggleRunning();
			}
		}
	}
	
	public void PlanningTurnStart(){
		
		foreach(var c in Characters)
		{
			c.SetTurnStartPosToPathEnd(this);
			c.Path_positions.Clear();
			
			if (c.CurrentMovementType==CharacterMovementType.Running){
				c.ToggleRunning();
			}
		}
	}
	
	public void MoveAll(){
		foreach(var c in Characters)
		{
			c.StartMovingGraphics();
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
		CurrentTileData=CurrentCharacter.CurrentTileData;
	}
	
	public QueryData OnMapActionsQuery{get{return new QueryData(CurrentTileData.Location,CurrentCharacter.Data,CurrentCharacter.Data,"OnMapActions");}}
	public QueryData OnClickBasicActionsQuery{get{return new QueryData(CurrentTileData.Location,CurrentCharacter.Data,CurrentCharacter.Data,"OnClickBasic");}}
	
	/// <summary>
	/// Iterates through all the AIs and simutates their AI thoughts.
	/// </summary>
	public void SimulateAIs ()
	{
		while(CurrentCharacter.AI){
			
			if (CurrentCharacter.Inactive){
				NextPlayersTurn();
				continue;
			}
			
			if (CurrentCharacter.MustInterract()){
				
				bool use_basic_action=true;
				if (Random.Range(0,100)<50){
					use_basic_action=false;
				}

				if (!use_basic_action){//character actions
					//get target
					var target=CurrentCharacter;
					while(target==CurrentCharacter){
						target=Subs.GetRandom(CurrentTileData.GameCharacters);
					}
					
					//get all possible actions
					var links=GC.dial_man_1.GetLinks(
						Core.rule_database.CheckQuery(new QueryData(CurrentTileData.Location,CurrentCharacter.Data,target.Data,"OnClick")).Link
					);
					
					//select one randomly
					if (links.Count>0){
						var random=Subs.GetRandom(links);
						CurrentCharacter.CurrentAction=new CharacterActionData(CurrentCharacter,CurrentCharacter,random.ToEvent,CurrentTileData.Location);
					}
					else
						use_basic_action=true;
				}
				
				if (use_basic_action){//basic interractions
					var links=GC.dial_man_1.GetLinks(
						Core.rule_database.CheckQuery(OnClickBasicActionsQuery).Link
					);
					var random=Subs.GetRandom(links);
					CurrentCharacter.CurrentAction=new CharacterActionData(CurrentCharacter,CurrentCharacter,random.ToEvent,CurrentTileData.Location);
				}
			}
			else{
				//map actions
				var links=GC.dial_man_1.GetLinks(
					Core.rule_database.CheckQuery(OnMapActionsQuery).Link
				);
				while (true){
					//effects DEV.reloc to one central place (XML system?)
					var random=Subs.GetRandom(links);
					var ToEvent=random.ToEvent;
					
					if (CurrentCharacter.OnMovingAwayFromTile)
						ToEvent="OnMapMove";
					
					if (ToEvent=="OnMapWait"){
						CurrentCharacter.RemovePath();
						break;
					}
					if (ToEvent=="OnMapMove"){
						//random pos for player
						TileData t=tiledata_map[Subs.GetRandom(tiledata_map.GetLength(0)),Subs.GetRandom(tiledata_map.GetLength(1))];
						CurrentCharacter.CalculatePath(t.TilePosition,this);
						break;
					}
					
					if (ToEvent=="OnMapUseVehicle"){
						CurrentCharacter.ToggleRunning();
					}
					
					if (ToEvent=="OnMapHide"){
						CurrentCharacter.ToggleHiding();	
					}
					
					links.Remove(random);
					continue;
				}
			}
			NextPlayersTurn();
		}
	}
	/// <summary>
	/// Jumps to the next non AI player.
	/// In otherwords skips all AIs.
	/// </summary>
	public void JumpToNextPlayer ()
	{
		while(CurrentCharacter.AI){
			NextPlayersTurn();
		}
	}
}
