using System;
using UnityEngine;

namespace Nebuloic
{
    public class ArmorLogic 
    {
        #region Fields
        private readonly ArmorData _data;

        private int[] _cumulativeGearHPs;
        private int _hp;
        private bool _isVulnerable = true;
        private Timer _invulnerabilityTimer;

        public event Action<int, int> HPChanged;
        public event Action<bool> VulnerabilityChanged;

        #endregion

        #region Constructor

        public ArmorLogic(ArmorData data) {
            _data = data;
            Init();
        }

        #endregion

        #region Properties

        public int HP
        {
            get => _hp;
            set
            {
                if (_data.GearHPs.Length <= 0) return; //there's no point in setting HP when gearHPs are not initialized 
                if (_hp == value) return; // why change hp and invoke events if there's no change
                if (!IsVulnerable && value < _hp) return; // invulnerability setup 
                int damage = _hp - value;
                _hp = Mathf.Clamp(value, 0, _cumulativeGearHPs[_cumulativeGearHPs.Length - 1]);
                HPChanged?.Invoke(_hp, damage);
            }
        }
        public bool IsVulnerable
        {
            get => _isVulnerable;
            set
            {
                if (_isVulnerable == value) return;
                _isVulnerable = value;
                VulnerabilityChanged?.Invoke(value);
            }
        }
        public int MaxGear
        {
            get
            {
                if (HP < _cumulativeGearHPs[0]) return 0;
                for (int i = 1; i <= _cumulativeGearHPs.Length; i++)
                {
                    if (HP <= _cumulativeGearHPs[i - 1] + 1) return i;
                }
                return _cumulativeGearHPs.Length - 1;
            }
        }

        #endregion

        #region Methods
        public void Init()
        {
            GenerateCumulativeHPs();
            IsVulnerable = true;
            HPChanged += AddInvulnerability;
        }

        public void Update(float deltaTimeInSeconds) {
            if (_invulnerabilityTimer != null) _invulnerabilityTimer.UpdateTimer(deltaTimeInSeconds);
        }

        private void AddInvulnerability(int _, int damage)
        {
            if (damage <= 0) return;
            IsVulnerable = false;
            _invulnerabilityTimer = new Timer(_data.InvulnerabilityTime);
            _invulnerabilityTimer.OnTimerEnd += () => {
                IsVulnerable = true;
                _invulnerabilityTimer = null;
            };
        }


        public void GenerateCumulativeHPs()
        {
            _cumulativeGearHPs = new int[_data.GearHPs.Length];
            int sum = 0;
            for (int i = 0; i < _data.GearHPs.Length; i++)
            {
                sum += _data.GearHPs[i];
                _cumulativeGearHPs[i] = sum;
            }
            HP = sum;
        }

        public bool CheckGearHP(int gear)
        {
            gear = Mathf.Clamp(gear, 1, _cumulativeGearHPs.Length);
            return HP > _cumulativeGearHPs[gear - 1];
        }
        #endregion
    }
}