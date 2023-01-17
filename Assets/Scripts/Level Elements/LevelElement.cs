using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/// <summary>
/// A base class for all the elements of the levels
/// </summary>
public abstract class LevelElement : MonoBehaviour
{
       
    [SerializeField] protected Game game; // б≥льш≥сть елемент≥в р≥вню потребуватимуть гравц€
    [SerializeField] float destroyCoordinate = -10;

    
    // Most level elements move downward with the speed of the player, so this method does this
    protected void FixedUpdate()
    {
        transform.Translate(new Vector2(0, -game.player.EngineComponent.CurrentSpeed * Time.fixedDeltaTime), Space.World);

        if (transform.position.y < destroyCoordinate) Destroy(gameObject);
    }

    /// <summary>
    /// Initializes a level element with given properties
    /// Uses reflection to fill in all non-serialized properties
    /// Serialized properties (except player) should be filled in the prefab
    /// Hence, it only makes sense to create non-serialized fields for values that need to be changed per same objects 
    /// (for example, text on a UI element, player shift, or length of a Field Obstacle)
    /// </summary>
    /// <param name="properties">A dictionary of properties with keys as field names</param>
    public void Init(Dictionary<string, object> properties) {
        foreach (var el in GetType().GetRuntimeFields())
        {
            object value;
            if (properties.TryGetValue(el.Name, out value))
            {
                if (el.FieldType == typeof(float)) value = Convert.ToSingle(value);
                el.SetValue(this, value);
            }
        }
    }
}
