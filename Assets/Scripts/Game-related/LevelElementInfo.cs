using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Contains serializable information about a level element, that is then used during level loading process
/// </summary>
public class LevelElementInfo : IComparable<LevelElementInfo>
{
    public string PrefabName;
    public float X, Y;
    /// <summary>
    /// Properties of a particular Level Element are saved there as (fieldName): (fieldValue)
    /// </summary>
    public LevelElementData Data; 


    // Sorting purposes
    public int CompareTo(LevelElementInfo obj)
    {
        return Y.CompareTo(obj.Y);
    }
}
