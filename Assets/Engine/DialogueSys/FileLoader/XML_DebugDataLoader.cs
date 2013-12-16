using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using System.Linq;
using DialogueSystem;


namespace DialogueSystem.FileIO
{
    public class XML_DebugDataLoader : XML_Loader
    {

        public LocationData ReadDebugData(CoreDatabase core)
        {
            var scene = core.scene_database.GetScene("Debug");

            if (scene == null) {
                Debug.Log("Debug Scene not found!");
                return null;
            }

            var loc = new LocationData(scene.Location);

            foreach(var c in scene.Characters)
                loc.Characters.Add(core.character_database.GetCharacterLazy(c));
            foreach(var o in scene.Objects)
                loc.Objects.Add(core.object_database.GetObject(o));

            return loc;
        }
            /*
            var path=DataFolder+"/DebugData";
            if (Directory.Exists(path)){
                foreach (var f in Directory.GetFiles(path)){
                    if (!f.EndsWith("Debug.xml")) continue;

                    var Xdoc=new XmlDocument();
                    Xdoc.Load(f);
                    return ReadDebugData(Xdoc["Root"]);
                }
            }
            return null;
        }

        LocationData ReadDebugData(XmlNode root){
            var xml_loader = new XML_WorldDataLoader();
            
            var world = xml_loader.LoadLocationData(root["Location"]);

            foreach (XmlNode n in root) {
                if (n.Name!="Character") continue;

                var Char=xml_loader.LoadCharacterData(n);
                world.Characters.Add(Char);
            }

            return world;
        }
             * */
    }
}