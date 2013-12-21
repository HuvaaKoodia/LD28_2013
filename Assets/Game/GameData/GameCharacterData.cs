using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;

public class GameCharacterData{
	
	public MapManager mapman;	
	
	public int stunned_for_turns;
	
	public CharacterData Data;
	public MapCharacter Main{get;private set;}
	
	public Vector2 TurnStartPos {get;private set;}
	public Vector2 CurrentPos {get;private set;}
	public Vector2 OldPos {get;set;} 
	
	//private Vector2 MoveToPos;
	
	public string Name{get;private set;}
	
	public CharacterActionData CurrentAction;
	 
	int temp_index;
	bool temp_movement;
	
	public List<Vector2> Path_positions=new List<Vector2>();
	
	public bool OnMovingAwayFromTile{get;set;}

	public GameCharacterData(string name){
		Name=name;
		OnMovingAwayFromTile=false;
	}
	
	public void Stun(int turns){
		stunned_for_turns=turns;
	}
	
	public bool IsStunned()
	{
		return stunned_for_turns>0;
	}
	
	public void RecoverStun(int turns)
	{
		stunned_for_turns-=turns;
	}
	
	public Tile TurnStartTile ()
	{
		return mapman.tiles_map[(int)TurnStartPos.x,(int)TurnStartPos.y];
	}
	public Tile CurrentTempTile ()
	{
		return mapman.tiles_map[(int)CurrentPos.x,(int)CurrentPos.y];
	}

	public void EndPathToCurrentPos ()
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
		Main.mapman=mapman;
		Main.SetCharacterData(Data);
	}

	
	public bool TempMovement{get{return temp_movement;}}
	
	public void StarMovement(){
		temp_index=0;
		temp_movement=true;
		CurrentPos=TurnStartPos;
	}
	public void EndMovement(){
		temp_movement=false;
	}
	
	public void MoveToNextTempPos(){
		if (Path_positions.Count>1)
		CurrentPos=Path_positions[++temp_index];
	}

	public Vector2 NextPos ()
	{
		if (temp_index+1<=Path_positions.Count-1)
			return Path_positions[temp_index+1];
		return CurrentPos;
	}
	
	public Vector2 CurrentTempPos(){
		return CurrentPos;
	}

	public bool CurrentPosIsLastPathPos ()
	{
		return CurrentPos==Path_positions[Path_positions.Count-1];
	}
	
	//path 
	public void CalculatePath(Vector2 endPos)
	{


		
		Path_positions.Clear();
		//find path
		
		int ex=(int)endPos.x,ey=(int)endPos.y;
		int tx=(int)TurnStartPos.x;
		int ty=(int)TurnStartPos.y;
	
		
		while (true)
		{
 			Path_positions.Add(new Vector2(tx,ty));
			
			int x_dif=ex-tx;
			int y_dif=ey-ty;
			
			if (x_dif==0&&y_dif==0){
				break;
			}
			
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
			
			var NEXT_POS=mapman.tiles_map[tx+x_abs,ty+y_abs];
			
			
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
					
					//if (tx+x_abs>1)
					
					if (tx+x_abs>mapman.gridX-1||tx+x_abs<0){
						x_abs*=-1;
					}
				}
				else if (y_abs==0)
				{
					y_abs=(int)Mathf.Sign(y_dif);
					x_abs=0;
					
					if (ty+y_abs>mapman.gridY-1||ty+y_abs<0){
						y_abs*=-1; 
					}
					
					//if (x_abs==0&&y_abs==0){
							
					//}
				}
//				else{
//					if (Mathf.Abs(y_dif)>Mathf.Abs(x_dif)){
//						x_abs=0;
//					}
//					else{
//						y_abs=0; 
//					}
//				}
			}
			
			tx+=x_abs;
			ty+=y_abs;
		}
	}

	public void StartMoving()
	{
		Main.Move(Path_positions);
	}
	
	public void SetTurnStartPosToPathEnd ()
	{
		if (Path_positions.Count>0)
			TurnStartPos=Path_positions[Path_positions.Count-1];
		else
			TurnStartPos=CurrentPos;

	}

	public void SetStartPosition (Vector2 tilePosition)
	{
		TurnStartPos=CurrentPos=tilePosition;
	}
}
