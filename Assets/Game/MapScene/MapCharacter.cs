using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCharacter : CharacterMain {
	
	public MapManager mapman;
	bool moving=false;
	
	public bool Moving{get{return moving;}}
	
	List<Vector2> CurrentPath;
	
	public float speed=0.1f;
	public System.Action on_path_end_Event;
	
	private Tile target;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (moving){
			var dir=(target.transform.position-transform.position).normalized;
			transform.position+=dir*speed*Time.deltaTime;
			
			if (Vector3.Distance(transform.position,target.transform.position)<speed*Time.deltaTime*2){
				transform.position=target.transform.position;
				SelectNextMovePos();
			}
		}
	}
	int cur_path_pos=0;
	public void Move(List<Vector2> Path)
	{
		moving=true;
		CurrentPath=Path;
		
		cur_path_pos=0;
		
		SelectNextMovePos();
	}
	
	void SelectNextMovePos()
	{
		if (cur_path_pos>CurrentPath.Count-1){
			moving=false;
			return;
		}
		
		var pos=CurrentPath[cur_path_pos++];
		int x=(int)pos.x;
		int y=(int)pos.y;
		
		target=mapman.tiles_map[x,y];
		
		if (on_path_end_Event!=null){
			on_path_end_Event();
		}
		var dir=(target.transform.position-transform.position).normalized;
		if (dir.magnitude!=0)
			graphics_offset.transform.rotation=Quaternion.LookRotation(dir,Vector3.up);
	}
}
