using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCharacterData{
	
	public MapManager mapman;	
	
	public CharacterData Data;
	public MapCharacter Main;
	
	public Vector2 CurPos,MovePos;
	
	public List<Vector2> Path_positions=new List<Vector2>();
	
	public void SetMovePos(Vector2 endPos)
	{
		MovePos=endPos;
		
		Path_positions.Clear();
		//find path
		
		int ex=(int)endPos.x,ey=(int)endPos.y;
		int tx=(int)CurPos.x;
		int ty=(int)CurPos.y;
	
		
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
			
			/*
			 * if (NEXT_POS.TilePosition==endPos){
					tx+=x_abs;
					ty+=y_abs;
					continue;
				}
			 * */
			
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
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Move ()
	{
		Main.Move(Path_positions);
	}
	
	public void SetToPathEnd ()
	{
		if (Path_positions.Count>0)
			CurPos=Path_positions[Path_positions.Count-1];
	}
}
