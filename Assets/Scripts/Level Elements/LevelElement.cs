using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class LevelElement : MonoBehaviour
{
       
    protected Player player; // б≥льш≥сть елемент≥в р≥вню потребуватимуть гравц€
    [SerializeField]
    float destroyCoordinate = -10;

    

    protected void FixedUpdate()
    {
        transform.Translate(new Vector2(0, -player.CurrentSpeed * Time.fixedDeltaTime), Space.World);

        if (transform.position.y < destroyCoordinate) Destroy(gameObject);
    }

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
