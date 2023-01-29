using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    [CreateAssetMenu(fileName = "ThrusterData", menuName = "Scriptable Objects/Thruster Data", order = 1)]
    public class ThrusterData : ScriptableObject
    {
        [SerializeField] float _accelerationModifier;
        [SerializeField] float _accelerationChangeSpeed = 0.1f;
        [SerializeField] float _perfectSwitchBoost;

        public float AccelerationModifier { get => _accelerationModifier; }
        public float AccelerationChangeSpeed { get => _accelerationChangeSpeed; }
        public float PerfectSwitchBoost { get => _perfectSwitchBoost; }
    }
}