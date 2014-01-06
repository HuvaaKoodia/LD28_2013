using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public enum CharacterMovementType {Normal,Hiding,Running}

public class GameCharacterData{
	
	public CharacterMovementType CurrentMovementType{get;private set;}
	
	public bool AI{get;private set;}
	
	public CharacterData Data;
	public MapCharacter Main{get;private set;}
	public CharacterMain ActionMain{get;private set;}
	
	public TileData CurrentTileData {get;private set;}
	public TileData TurnStartTileData {get;private set;}
	
	public Vector2 TurnStartPos {get;private set;}
	public Vector2 CurrentPos {get;private set;}
	public Vector2 OldPos {get;set;} 
	
	public bool Inactive{get{return IsArrested()||IsStunned();}}

	public bool IsHiding{get {return CurrentMovementType==CharacterMovementType.Hiding;}}
	public bool IsRunning{get {return CurrentMovementType==CharacterMovementType.Running;}}
	
	public string Name{get;private set;}
	public int ArrestedTurns{get;private set;}
	
	public CharacterActionData CurrentAction;
	
	int temp_index;
	bool temp_movement;
	
	public List<Vector2> Path_positions=new List<Vector2>();
	
	public bool OnMovingAwayFromTile{get;set;}

	public GameCharacterData(string name,bool ai){
		Name=name;
		OnMovingAwayFromTile=false;
		ArrestedTurns=0;
		
		AI=ai;
		
		CurrentMovementType=CharacterMovementType.Normal;
	}
	public float Money_TurnStart,VP_TurnStart,Drugs_TurnStart;
	
	public void SetTurnStartValues()
	{
		Money_TurnStart=Data.Facts.GetFloat("Money");
		VP_TurnStart=Data.Facts.GetFloat("VP");
		Drugs_TurnStart=Data.Facts.GetFloat("Drugs");
	}
	
	public void ToggleHiding(){
		CurrentMovementType=CurrentMovementType==CharacterMovementType.Hiding?CharacterMovementType.Normal:CharacterMovementType.Hiding;
	}
	
	public void ToggleRunning(){
		CurrentMovementType=CurrentMovementType==CharacterMovementType.Running?CharacterMovementType.Normal:CharacterMovementType.Running;
	}
	
	public void Stun(int turns){
		Data.Facts.SetFact("Stun",turns);
	}
	
	public bool IsStunned()
	{
		return Data.Facts.GetFloat("Stun")>0;
	}
	
	public void Arrest(int turns)
	{

		ArrestedTurns+=turns;
	}

	public bool IsArrested ()
	{
		return ArrestedTurns>0;
	}
	
	public void RecoverStun(int turns)
	{
		Data.Facts.AddFactValue("Stun",-turns);	
	}
	
	public void RecoverArrest(int turns)
	{
		if (!IsArrested()) return;
		
		ArrestedTurns--;
		
		if (ArrestedTurns==0){
			CurrentTileData.AddCharacter(this);
		}
	}
	
	public void EndPathToCurrentPos()
	{
		if (Path_positions.Count>0){
			while (!CurrentPosIsLastPathPos()){
				Path_positions.RemoveAt(Path_positions.Count-1);
			}
		}
	}
	
	public void SetMain(MapCharacter main){
		Main=main;
		//Main.on_path_end_Event+=EndMoving;
		Main.SetCharacterData(Data);
	}
	
	public void SetActionMain(CharacterMain main){
		ActionMain=main;
		main.SetCharacterData(this);
	}

	public bool TempMovement{get{return temp_movement;}}
	
	public void StartMovement(){
		temp_index=0;
		temp_movement=true;
		CurrentPos=TurnStartPos;
	}
	public void EndMovement(){
		temp_movement=false;
	}
	
	public void MoveToNextTempPos(GameDatabase GDB){
		if (Path_positions.Count>1){
			CurrentPos=Path_positions[++temp_index];
			CurrentTileData=GDB.GetTile(CurrentPos);
		}
	}

	public Vector2 NextPos ()
	{
		if (temp_index+1<=Path_positions.Count-1)
			return Path_positions[temp_index+1];
		return CurrentPos;
	}

	public bool CurrentPosIsLastPathPos ()
	{
		return CurrentPos==Path_positions[Path_positions.Count-1];
	}
	
	public bool CurrentPosIsTurnStartPos()
	{
		return CurrentPos==TurnStartPos;
	}
	
	//path 
	public void RemovePath()
	{
		Path_positions.Clear();
	}
	
	int max_movement=0;
	public void CalculatePath(Vector2 endPos,GameDatabase GDB)
	{
		int ex=(int)endPos.x,ey=(int)endPos.y;
		int tx=(int)GetPathEndPos().x;
		int ty=(int)GetPathEndPos().y;
		
		if (max_movement>0){
			if (GDB.tiledata_map[tx,ty].Blocked()){
				max_movement=0;
			}
		}
		
		if (max_movement==0){
			max_movement=GetMovementSpeed();
			Path_positions.Clear();
			
			tx=(int)TurnStartPos.x;
			ty=(int)TurnStartPos.y;
		}
		
			
		while (true)
		{
			var pos=new Vector2(tx,ty);
			if (!Path_positions.Contains(pos))
 			Path_positions.Add(pos);
			
			int x_dif=ex-tx;
			int y_dif=ey-ty;
			
			if (x_dif==0&&y_dif==0){
				break;
			}
			
			if (max_movement==0) break;
			
			max_movement--;
			
			int y_abs=(int)(Mathf.Sign(y_dif)*Mathf.Min(1,Mathf.Abs(y_dif)));
			int x_abs=(int)(Mathf.Sign(x_dif)*Mathf.Min(1,Mathf.Abs(x_dif)));
			
			if (x_abs!=0&&y_abs!=0){
				if (Mathf.Abs(y_dif)>Mathf.Abs(x_dif)){
					x_abs=0;
				}
				else{
					y_abs=0; 
				}
			}
			var NEXT_POS=GDB.tiledata_map[tx+x_abs,ty+y_abs];
			
			if (NEXT_POS.TilePosition==endPos){
				tx+=x_abs;
				ty+=y_abs;
				continue;
			}
			 
			if (NEXT_POS.Blocked()){
				//move to other direction
				
 				if (x_abs==0)
				{
					y_abs=0;
					x_abs=(int)Mathf.Sign(x_dif);

					if (tx+x_abs>GDB.MapWidth-1||tx+x_abs<0){
						x_abs*=-1;
					}
				}
				else if (y_abs==0)
				{
					y_abs=(int)Mathf.Sign(y_dif);
					x_abs=0;
					
					if (ty+y_abs>GDB.MapHeight-1||ty+y_abs<0){
						y_abs*=-1; 
					}
				}
			}
			
			tx+=x_abs;
			ty+=y_abs;	
		}
	}

	public void StartMovingGraphics()
	{
		Main.Move(Path_positions);
	}
	
	public void SetTurnStartPosToPathEnd(GameDatabase GDB)
	{
		TurnStartPos=GetPathEndPos();
		TurnStartTileData=GDB.GetTile(TurnStartPos);
	}
	
	public Vector2 GetPathEndPos()
	{
		if (Path_positions.Count>0)
			return Path_positions[Path_positions.Count-1];
		return CurrentPos;
	}

	public void SetStartPosition(TileData tile)
	{
		SetCurrentTile(tile);
		TurnStartPos=CurrentPos;
		TurnStartTileData=tile;
	}
	
	public void SetCurrentTile(TileData tile)
	{
		CurrentTileData=tile;
		CurrentPos=tile.TilePosition;
	}

	public void MoveToPosition(TileData tile)
	{
		CurrentTileData.RemoveCharacter(this);
		SetStartPosition(tile);
	}

	public int GetMovementSpeed()
	{
		if (CurrentMovementType==CharacterMovementType.Hiding)
			return (int)Data.Facts.GetFloat("HidingSpeed");
		if (CurrentMovementType==CharacterMovementType.Running)
			return (int)Data.Facts.GetFloat("VehicleSpeed");
		return (int)Data.Facts.GetFloat("MovementSpeed");
	}

	public bool MustInterract()
	{
		return !OnMovingAwayFromTile&&TurnStartTileData.HasOtherCharactersNotMoving(this);
	}
}
