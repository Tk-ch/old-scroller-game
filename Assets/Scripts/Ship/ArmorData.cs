using UnityEngine;

namespace Nebuloic
{
    [CreateAssetMenu(fileName = "ArmorData", menuName = "Scriptable Objects/Armor Data", order = 1)]
    public class ArmorData : ScriptableObject
    {
        [SerializeField] private int[] _gearHPs;
        [SerializeField] float _invulnerabilityTime;

        public int[] GearHPs { get => _gearHPs; }
        public float InvulnerabilityTime { get => _invulnerabilityTime; }
    }
}