using System;
using UnityEngine;

namespace Nebuloic
{

    /// <summary>
    /// Class to manage ship's HP and check whether the shift can be increased. 
    /// </summary>
    [CreateAssetMenu(fileName = "Armor", menuName = "Scribptable Objects/Armor", order = 1)]
    public class Armor : ScriptableObject
    {
        [SerializeField] private int[] _gearHPs;


        private int[] _cumulativeGearHPs;
        private int _hp;

        public int[] GearHPs { get => _gearHPs; }

        public int HP
        {
            get => _hp;
            set
            {
                if (_gearHPs.Length <= 0) return; //there's no point in setting HP when gearHPs are not initialized 
                if (_hp == value) return; // why change hp and invoke events if there's no change
                if (!IsVulnerable && value < _hp) return; // invulnerability setup 
                int damage = _hp - value;
                _hp = Mathf.Clamp(value, 0, _cumulativeGearHPs[_cumulativeGearHPs.Length - 1]);
                HPChanged?.Invoke(this, damage);
            }
        }
        private bool _isVulnerable = true;
        public bool IsVulnerable
        {
            get => _isVulnerable; 
            set
            {
                if (_isVulnerable == value) return;
                _isVulnerable = value;
                VulnerabilityChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<int> HPChanged;
        public event EventHandler<bool> VulnerabilityChanged;


        public Armor(int[] gearHPs) // constructor for the tests 
        { 
            _gearHPs = gearHPs;
            Init();
        }

        public void Init() {
            GenerateCumulativeHPs();
            IsVulnerable = true;
            HPChanged += AddInvulnerability;
        }

        private void AddInvulnerability(object _, int damage)
        {
            if (damage <= 0) return;
            IsVulnerable = false;
        }

        public void GenerateCumulativeHPs()
        {
            _cumulativeGearHPs = new int[_gearHPs.Length];
            int sum = 0;
            for (int i = 0; i < _gearHPs.Length; i++)
            {
                sum += _gearHPs[i];
                _cumulativeGearHPs[i] = sum;
            }
            HP = sum;
        }

        public bool CheckGearHP(int gear)
        {
            gear = Mathf.Clamp(gear, 1, _cumulativeGearHPs.Length);
            return HP > _cumulativeGearHPs[gear - 1];
        }
    }
}