using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DialogueSystem
{

    public class DialogueData
    {
        public string Text { get { return _text; } set { _text = value.Replace("\\n", "\n"); } }
        public string _text, Type = "";

		public float RandomChance { get; private set; }
		public bool LinksToRule;
		public string ToEntity;
		public string ToEvent;

		public List<DialogueLink> TempLinks = new List<DialogueLink>();
		public List<DialogueData> Links { get; private set; }

        public DialogueData(string text)
        {
			Text=text;
			LinksToRule=false;
			Links = new List<DialogueData>();
        }

        public DialogueData(string text, string type):this(text)
        {
            Type = type;
        }

		public DialogueData(string text,string toEntity,string toEvent):this(text)
		{
			ToEntity=toEntity;
			ToEvent=toEvent;
			LinksToRule=true;
		}
	
        public bool HasLinks()
        {
            return Links.Count > 0;
        }

        public void AddLink(DialogueLink link)
        {
			TempLinks.Add(link);
        }
		public void AddLink(DialogueData link)
		{
			Links.Add(link);
		}

        public DialogueData GetRandom()
        {
            List<DialogueData> links = new List<DialogueData>();
            bool even_random = true;
			foreach (var l in Links)
            {
                links.Add(l);
                if (l.RandomChance != 0)
                {
                    even_random = false;
                }
            }
            if (even_random)
            {
                return links[Random.Range(0, links.Count)];
            }
            else
            {
                links.OrderByDescending(l => l.RandomChance);

                foreach (var l in links)
                {
                    var per = Subs.RandomPercent();
                    Debug.Log("" + per);
                    if (per < l.RandomChance)
                    {
                        return l;
                    }
                }
                return links.Last();
            }
        }

        public string ParseText(QueryData query)
        {
            string text="",vari="";
            bool reading_var = false;
            foreach (var s in _text) {
                if (s == '#') {
                    reading_var =!reading_var;
                    if (!reading_var) {//second # -> read variable
                        var fact=query.FindFact(vari);
                        if (fact != null)
                        {
                            text += fact.Value;
                        }
                        else {
                            Debug.LogError("Fact with the name: " + vari + " does not exist.");
                        }
                        vari = "";
                    }
                    continue;
                }

                if (reading_var)
                {
                    vari += s;
                }
                else
                    text += s;
            }
            return text;
        }
    }

	public class DialogueLink
	{
		public float RandomChance;
		public string Link;
	}
}