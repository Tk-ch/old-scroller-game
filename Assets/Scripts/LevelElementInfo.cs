using System;
using System.Collections;
using System.Collections.Generic;

public class LevelElementInfo : IComparable<LevelElementInfo>
{
    public string PrefabName;
    public float X, Y;
    public Dictionary<string, object> Properties = new Dictionary<string, object>();

    public int CompareTo(LevelElementInfo obj)
    {
        return Y.CompareTo(obj.Y);
    }
}
