using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCharacterData : MonoBehaviour {
	
	public MapManager mapman;	
	
	public CharacterData Data;
	public MapCharacter Main;
	
	public Vector2 CurPos,MovePos;
	
	List<Vector2> Path_positions;
	
	void SetMovePos(Vector2 endPos)
	{
		MovePos=endPos;
		
		Path_positions=new List<Vector2>();
		
		var temp_pos=CurPos;
		//find path
		
		int ex=(int)endPos.x,ey=(int)endPos.y;
		
		while (true){
			
			Path_positions.Add(new Vector2(temp_pos.x,temp_pos.y));
			
			int tx=(int)temp_pos.x;
			int ty=(int)temp_pos.y;
			
			int x_dif=ex-tx;
			int y_dif=ey-ty;
			
			if (x_dif==0&&y_dif==0){
				break;
			}
			
			int x_abs=(int)Mathf.Sign(x_dif);
			int y_abs=(int)Mathf.Sign(y_dif);
			
			if (mapman.tiles_map[tx,ty].Blocked()){
				
				//move to other direction
			}
			
			
		}
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
