using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DialogueSystem;

using UnityEngine;

namespace DialogueSystem.FileIO
{
    class XML_WorldDataLoader:XML_Loader
    {
        //individual objects
        public LocationData LoadLocationData(XmlNode node){
            var data = new LocationData();

            data.Facts.AddFact("Location", node.Attributes["Name"].Value);

            foreach (XmlNode n in node) {
                if (n.Name != "Var") continue;
                var spl = n.InnerText.Split(' ');
                data.Facts.AddFact(spl[0], FactData.ParseStringToData(spl[1]));
            }

            return data;
        }

        public ObjectData LoadObjectData(XmlNode node)
        {
            string type = node.Attributes["Type"].Value;
            string name = "";
            if (node.Attributes["Name"]!= null) {
                name = node.Attributes["Name"].Value;
            }
            var data = new ObjectData(type,name);

            foreach (XmlNode n in node)
            {
                if (n.Name != "Var") continue;
                var spl = n.InnerText.Split(' ');
                data.Facts.AddFact(spl[0], FactData.ParseStringToData(spl[1]));
            }
            return data;
        }

        //read all data from a file

        public void ReadObjects(List<XmlNode> nodes, CoreDatabase core_database)
        {
            foreach (var n in nodes) {
                var o = LoadObjectData(n);
                string key = o.NameOrType;
                
               
                core_database.object_database.AddObject(key,o);
            }
        }

        public void ReadCharacters(List<XmlNode> nodes, CoreDatabase core_database)
        {
            foreach (var node in nodes)
            {
                var obj = new CharacterDataLoadObj();

				obj.Type =node.Attributes["Type"].Value;
				obj.Name =node.Attributes["Name"].Value;
                //obj.Facts.Add("Character "+node.Attributes["Type"].Value);
                //obj.Facts.Add("Name " + );

                foreach (XmlNode n in node) {
                    if (n.Name == "Var"){
                        obj.Facts.Add(n.InnerText);
                    }
                    else if (n.Name == "Object") {
                        obj.Objects.Add(n.InnerText);
                    }
                }

				core_database.character_database.AddLoadObj(obj.Name,obj);
            }
        }

        public void ReadLocations(List<XmlNode> nodes, CoreDatabase core_database)
        {
            foreach (var n in nodes)
            {
                var data = LoadLocationData(n);

                core_database.location_database.AddLocation(n.Attributes["Name"].Value, data);
            }
        }

        public void ReadScenes(List<XmlNode> nodes, CoreDatabase core_database)
        {
            foreach (var n in nodes)
            {
                var scene = new SceneData();

                foreach (XmlNode cn in n)
                {
                    if (cn.Name == "Location")
                    {
                        scene.Location=cn.InnerText;
                    }
                    if (cn.Name == "Object")
                    {
                        scene.Objects.Add(cn.InnerText);
                    }
                    if (cn.Name == "Character")
                    {
                        scene.Characters.Add(cn.InnerText);
                    }
                }

                core_database.scene_database.AddScene(n.Attributes["Name"].Value,scene);
            }
        }
    }
}
