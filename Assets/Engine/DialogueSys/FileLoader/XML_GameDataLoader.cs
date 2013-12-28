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
    public class XML_GameDataLoader : MonoBehaviour
    {
        public CoreDatabase core_database;
        public string DataFolder = "Data/",XMLDataFolder="Data/XMLData";

        void Awake()
        {
            XML_sys.OnRead += read;
            XML_sys.OnWrite += write;
        }

        public void read()
        {
            var xml_dial = new XML_DialogueSystemLoader();
            var xml_world = new XML_WorldDataLoader();
			
			ReadDirectory(XMLDataFolder,xml_dial,xml_world);
		
            //finalize databases
            core_database.dialogue_database.ParseDataBase();
            core_database.rule_database.ParseDataBase(core_database);
        }

        //examples
        public void write()
        {
            var xml_ex = new XML_ExampleDataWriter();
            xml_ex.writeAll(DataFolder);
        }
		
		/// <summary>
		/// Recursively reads the directory for data.
		/// </summary>
		void ReadDirectory(string folder,XML_DialogueSystemLoader xml_dial,XML_WorldDataLoader xml_world){

			var path = folder + "\\";

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (!file.EndsWith(".xml")) continue;

                var Xdoc = new XmlDocument();
                Xdoc.Load(file);
                var root=Xdoc["Root"];
                
                //disabled?
                if (root.Attributes["Disabled"] != null && root.Attributes["Disabled"].InnerText == "true") continue;

				xml_dial.ReadCriterions(XML_Loader.getChildrenByTag(root, "Criterion"), core_database);
				xml_dial.ReadDialogues(XML_Loader.getChildrenByTag(root, "Dialogue"), core_database);
				xml_dial.ReadRules(XML_Loader.getChildrenByTag(root, "Rule"), core_database);
				xml_world.ReadObjects(XML_Loader.getChildrenByTag(root, "Object"), core_database);
				xml_world.ReadCharacters(XML_Loader.getChildrenByTag(root, "Character"), core_database);
				xml_world.ReadLocations(XML_Loader.getChildrenByTag(root, "Location"), core_database);
				xml_world.ReadScenes(XML_Loader.getChildrenByTag(root, "Scene"), core_database);
            }
			
			//Recursively check all folders
			var dirs = Directory.GetDirectories(folder);
            foreach (var d in dirs)
            {	
				ReadDirectory(d,xml_dial,xml_world);   
            }
			
		}
    }
}
