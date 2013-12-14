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
    public class XML_ExampleDataWriter : XML_Loader
    {
        
        public void WriteDialogueExample(string DataFolder)
        {
            var path = DataFolder + "Examples";
            checkFolder(path);

            var file = "/DialogueExample.xml";

            var Xdoc = new XmlDocument();

            var root = Xdoc.CreateElement("Root");
            Xdoc.AppendChild(root);

            addComment(root, "Dialogue requires a name but doesn't need a type.\nCommon types: ENDL, RANDOM.");

            var data = addElement(root, "Dialogue");

            addAttribute(data, "Name", "DialogueName");
            addAttribute(data, "Type", "DialogueType");
			addComment(data, "The dialogue text can refer to variables in the query. (syntax: #[variable name]#)");
            addElement(data, "Text", "The character is called #Name#");

            addComment(data, "The link's random value has an effect only if the dialogue type is RANDOM\nLinks are chosen with a random test from 0 to 100, starting from the one with the highest random value.");

            addElement(data, "Link", "DialogueName1");
            var link = addElement(data, "Link", "DialogueName2");
            addAttribute(link, "Random", "80");
            addElement(data, "Link", "DialogueName3");
			addComment(data, "The link can directly call a dialogue like this.(syntax: \"[text]\" to [Dialogue])");
			addElement(data, "Link", "\"Hello friend!\" to GreetFriend");
			addComment(data, "The link can directly call a rule check like this.(syntax: \"[text]\" to [CharacterScope] [EventScope])");
			addElement(data, "Link", "\"Hello friend!\" to Friend OnGreet");

            Xdoc.Save(path + file);
        }

        public void WriteCriterionExample(string DataFolder)
        {
            var path = DataFolder + "Examples";
            checkFolder(path);

            var file = "/CriterionExample.xml";

            var Xdoc = new XmlDocument();

            var root = Xdoc.CreateElement("Root");
            Xdoc.AppendChild(root);

            addComment(root, "The following operations are legal: E=equal,L=less,G=greater,LE=lessorequal,GE=greaterorequal");

            var data = addElement(root, "Criterion");
            addAttribute(data, "Name", "InTown");
            addAttribute(data, "Test", "Location E Town");

            data = addElement(root, "Criterion");
            addAttribute(data, "Name", "PlayerNearDeath");
            addAttribute(data, "Test", "Player_HP LE 10");

            /*
            data = addElement(root, "Criterion");
            addAttribute(data, "Name", "PlayerNotGood");
            addAttribute(data, "Test", "Player_HP G 10 LE 40");
            */

            data = addElement(root, "Criterion");
            addAttribute(data, "Name", "IsNight");
            addAttribute(data, "Test", "IsNight E true");

            Xdoc.Save(path + file);
        }

        public void WriteRuleExample(string DataFolder)
        {
            var path = DataFolder + "Examples";
            checkFolder(path);

            var file = "/RuleExample.xml";

            var Xdoc = new XmlDocument();

            var root = Xdoc.CreateElement("Root");
            Xdoc.AppendChild(root);

            addComment(root, "The name of the rule is currently only used for debugging purposes.");
            var data = addElement(root, "Rule");
            addAttribute(data, "Name", "PlayerNotOkDaytimeInCity");
            addComment(data, "Location doubles as a query scope value for the rule.\nA rule without a location is assigned to the Global scope which is checked for every query.");
            addElement(data, "Location", "City");
            addComment(data, "Character doubles as a query scope value for the rule.\nA rule without a character is checked for every query.");
            addElement(data, "Character", "Player");
            addComment(data, "Event doubles as a query scope value for the rule.\nRules with events override those without.");
            addElement(data, "Event", "OnStart");
            addComment(data, "Criterions can be declared outside or inside the rule.");
            addElement(data, "Criterion", "IsDay");
            addElement(data, "Criterion", "Player_HP L 50");
            addComment(data, "You can also add multiple criterions in the same tag.");
            addElement(data, "Criterion", "Player_HP G 10;HighMorale");
            var r = addElement(data, "Result");
            addComment(r, "Result contains the piece of dialogue this rule links to. See dialogue example for full link syntax.");
            addElement(r, "Link", "DialogueName2");
            addComment(r, "It can also have multiple variable assignments");
            addElement(r, "Assign", "Afraid = true");
            addComment(r, "Which can also be added in the same tag.");
            addElement(r, "Assign", "Morale - 1;EnemyAggression + 1");
            addComment(r, "Result can also handle objects in the current characters inventory.");
            addComment(r, "You can add them ( 1, n or random amount between min max)");
            addElement(r, "Object", "Add Gem");
            addElement(r, "Object", "Add Coin 5");
            addElement(r, "Object", "Add Coin 2 6");
            addComment(r, "Or Remove them with the same syntax.");
            addElement(r, "Object", "Remove Pants");
            addElement(r, "Object", "Remove Horse 5");
            addElement(r, "Object", "Remove Fingers 2 6");

            data = addElement(root, "Criterion");
            addAttribute(data, "Name", "IsDay");
            addAttribute(data, "Test", "IsNight E false");

			data = addElement(root, "Criterion");
			addAttribute(data, "Name", "HighMorale");
            addAttribute(data, "Test", "Morale G 90");

            Xdoc.Save(path + file);
        }

        public void WriteLocationExample(string DataFolder)
        {
            var path = DataFolder + "Examples";
            checkFolder(path);

            var file = "/LocationExample.xml";

            var Xdoc = new XmlDocument();

            var root = Xdoc.CreateElement("Root");
            addComment(root, "You can disable the whole file with the disabled attribute.");
            addAttribute(root, "disabled", "true");
            Xdoc.AppendChild(root);

            var data = addElement(root, "Location");

            addAttribute(data, "Name", "Forest");
            addElement(data, "Var", "IsDay True");
            addElement(data, "Var", "IsCold True");

            Xdoc.Save(path + file);
        }

        public void WriteCharacterExample(string DataFolder)
        {
            var path = DataFolder + "Examples";
            checkFolder(path);

            var file = "/CharacterExample.xml";

            var Xdoc = new XmlDocument();

            var root = Xdoc.CreateElement("Root");
            Xdoc.AppendChild(root);

            var data = addElement(root, "Character");

            addAttribute(data, "Type", "Player");
            addAttribute(data, "Name", "Tester");
            addElement(data, "Var", "InDanger false");
            addElement(data, "Var", "HP 45");
            addElement(data, "Var", "Warmness 10");

            data = addElement(root, "Character");

            addAttribute(data, "Type", "Ally");
            addAttribute(data, "Name", "Testee");
            addElement(data, "Var", "InDanger true");
            addElement(data, "Var", "HP 55");
            addElement(data, "Var", "Warmness 3");
            addElement(data, "Object", "Sword");

            Xdoc.Save(path + file);
        }

        public void WriteObjectExample(string DataFolder)
        {
            var path = DataFolder + "Examples";
            checkFolder(path);

            var file = "/ObjectExample.xml";

            var Xdoc = new XmlDocument();

            var root = Xdoc.CreateElement("Root");
            Xdoc.AppendChild(root);

            var data = addElement(root, "Object");

            addAttribute(data, "Type", "Sword");
            addAttribute(data, "Name", "Excalibur");
            addElement(data, "Var", "Magical True");

            addComment(root, "An object without a name uses the type instead when needed.");
            data = addElement(root, "Object");
            addAttribute(data, "Type", "Rock");
            addElement(data, "Var", "Magical False");

            Xdoc.Save(path + file);
        }

        public void WriteSceneExample(string DataFolder)
        {
            var path = DataFolder + "Examples";
            checkFolder(path);

            var file = "/SceneExample.xml";

            var Xdoc = new XmlDocument();

            var root = Xdoc.CreateElement("Root");
            Xdoc.AppendChild(root);

            var data = addElement(root, "Scene");

            addAttribute(data, "Name", "Scene1");
            addElement(data, "Location", "Forest");
            addElement(data, "Character", "Player");
            addElement(data, "Character", "Ally");
            addElement(data, "Object", "Rock");

            Xdoc.Save(path + file);
        }

        internal void writeAll(string DataFolder)
        {
            WriteDialogueExample(DataFolder);
            WriteCriterionExample(DataFolder);
            WriteRuleExample(DataFolder);
            WriteCharacterExample(DataFolder);
            WriteLocationExample(DataFolder);
            WriteObjectExample(DataFolder);
            WriteSceneExample(DataFolder);
        }
    }
}