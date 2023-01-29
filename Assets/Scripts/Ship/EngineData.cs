using UnityEngine;

namespace Nebuloic
{
    [CreateAssetMenu(fileName = "EngineData", menuName = "Scriptable Objects/Engine Data", order = 1)]
    public class EngineData : ScriptableObject
    {
        [SerializeField] float[] _gearSpeeds;
        [SerializeField] float _minimumSpeed;

        public float[] GearSpeeds { get => _gearSpeeds; }
        public float MinimumSpeed { get => _minimumSpeed; }
    }
}