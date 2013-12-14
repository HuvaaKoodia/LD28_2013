using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class InventoryData
{
    public Dictionary<string, List<ObjectData>> Objects { get; private set; }//inventory system?

    public InventoryData() {
        Objects = new Dictionary<string, List<ObjectData>>();
    }

    public void Add(ObjectData obj)
    {
        if (!Objects.ContainsKey(obj.NameOrType))
        {
            Objects.Add(obj.NameOrType, new List<ObjectData>());
        }

        Objects[obj.NameOrType].Add(obj);
    }

    public void Remove(ObjectData obj)
    {
        if (Objects.ContainsKey(obj.NameOrType))
        {
            Objects[obj.NameOrType].Remove(obj);
        }
    }

    public void Remove(string name)
    {
        if (Objects.ContainsKey(name))
        {
            Objects[name].RemoveAt(0);
        }
    }
    public int Amount(string name) {
        if (Objects.ContainsKey(name))
        {
            return Objects[name].Count;
        }
        return -1;
    }
}

