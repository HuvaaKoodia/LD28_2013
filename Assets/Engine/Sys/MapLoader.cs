using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;
using System.Xml;

public class MapData{
	public string[,] map_data;

	public MapData(int w,int h){
		map_data=new string[w,h];
	}

}

public class MapLoader : MonoBehaviour{

	public List<MapData> Maps=new List<MapData>();
	
	public void Awake(){
		XML_sys.OnRead+=read;
	}
	
	void read()
	{
		XML_Loader.checkFolder("Data/Maps");

		var files=Directory.GetFiles("Data/Maps");

		foreach (var f in files){
			var Xdoc=new XmlDocument();
			//Debug.Log("f "+f);
			Xdoc.Load(f);

			//read xml
			var root=Xdoc["Root"];

			foreach (XmlNode node in root){
				if (node.Name=="Map"){
					var map=new MapData(5,7);

					var spl=node.InnerText.Replace(" ","").Replace("\r","").Split('\n');
					int i=0,j=0;
					foreach (var line in spl){
						if (line=="") continue;
						while (j<map.map_data.GetLength(1)){
							var ss=line.Substring(j,1).ToLower();
							map.map_data[i,j]=ss;
							j++;
						}
						i++;
						j=0;
					}
					
					Maps.Add(map);
				}
			}
		}
	}
}
