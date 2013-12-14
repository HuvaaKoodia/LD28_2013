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
    public class XML_DialogueSystemLoader : XML_Loader
    {
        public void ReadDialogues(List<XmlNode> dialogueNodes, CoreDatabase core_database)
        {
            foreach (XmlNode d in dialogueNodes)
            {
                try
                {
                    var name = d.Attributes["Name"].Value;

                    var type = "";
                    if (d.Attributes["Type"] != null)
                        type = d.Attributes["Type"].Value;
                    var text = "";
                    if (d["Text"] != null)
                        text = d["Text"].InnerText;

                    var data = new DialogueData(text, type);

                    //reading links
                    foreach (XmlNode n in d.ChildNodes)
                    {
                        if (n.Name != "Link") continue;
                        int chance = 0;
                        if (n.Attributes["Random"] != null)
                        {
                            chance = int.Parse(n.Attributes["Random"].Value);
                        }
						var link=new DialogueLink();
						link.Link=n.InnerText;
						link.RandomChance=chance;
                        data.AddLink(link);

                    }
                    core_database.dialogue_database.AddDialogueData(name, data);
                }
                catch (Exception e)
                {
                    Debug.LogError("Dialogue data is faulty!\n" + e.Message);
                    break;
                }
            }
        }

        public void ReadCriterions(List<XmlNode> dialogueNodes, CoreDatabase core_database)
        {

            foreach (XmlNode d in dialogueNodes)
            {
                try
                {
                    var name = d.Attributes["Name"].Value;
                    var test = d.Attributes["Test"].Value;

                    core_database.criterion_database.AddCriterion(name, test);
                }
                catch (Exception e)
                {
					//IXmlLineInfo info=(IXmlLineInfo)d;
					Debug.LogError("Criterion data is faulty!\n" + e.Message +" [file: "+d.BaseURI+ " ]");//", line: "+info.LineNumber+"
                }
            }
        }

        public void ReadRules(List<XmlNode> dialogueNodes, CoreDatabase core_database)
        {
            foreach (XmlNode d in dialogueNodes)
            {
                try
                {
                    TempRuleData r = new TempRuleData();
                    var name = d.Attributes["Name"].Value;
                    r.Name = name;
					var att=d.Attributes["Base"];
					if (att!=null){
						r.Base = att.Value;
					}
                    //reading criterions
                    foreach (XmlNode n in d.ChildNodes)
                    {
                        if (n.Name == "Criterion")
                        {
							var spl = Subs.Split(n.InnerText,";");
                            foreach (var s in spl)
                                r.temp_criterions.Add(s);
                        }
                        else if (n.Name == "Location")
                        {
                            r.Location = n.InnerText;
                        }
                        else if (n.Name == "Event")
                        {
                            r._Event = n.InnerText;
                        }
						else if (n.Name == "Actor"||n.Name == "Character")
                        {
                            r.Actor = n.InnerText;
                        }
						else if (n.Name == "Target")
						{
							r.Target = n.InnerText;
						}
                        else if (n.Name == "Result")
                        {
							if (n["Link"]!=null)
								r.Link = n["Link"].InnerText;
                            foreach (XmlNode a in n.ChildNodes)
                            {
                                if (a.Name == "Assign")
                                {
									var spl = Subs.Split(a.InnerText,";");
                                    foreach (var s in spl)
                                        r.temp_assigns.Add(s);
                                }
								if (a.Name == "Function")
								{
									var spl = Subs.Split(a.InnerText,";");
									foreach (var s in spl)
										r.temp_functions.Add(s);
								}
                            }
                        }
                    }
                    core_database.rule_database.addTempRule(r);
                }
                catch (Exception e)
                {
					Debug.LogError("Rule data is faulty!\n" + e.Message+" [file: "+d.BaseURI+ " ]");
                    break;
                }
            }
        }

    }
}