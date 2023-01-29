using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    [CreateAssetMenu(fileName = "ShipData", menuName = "Scriptable Objects/Ship Data", order = 1)]

    public class ShipData : ScriptableObject
    {
        [SerializeField] ArmorData armorData;
        [SerializeField] EngineData engineData;
        [SerializeField] ThrusterData thrusterData;

        public ArmorData ArmorData { get => armorData;  }
        public EngineData EngineData { get => engineData;  }
        public ThrusterData ThrusterData { get => thrusterData; }
    }
}