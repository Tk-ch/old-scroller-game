using System;
using UnityEngine;

namespace Nebuloic
{
    public class ArmorLogic 
    {
        #region Fields

        private readonly int[] _gearHPs;
        private readonly float _invulnerabilityTime;

        private int[] _cumulativeGearHPs;
        private int _hp;
        private bool _isVulnerable = true;
        private Timer _invulnerabilityTimer;

        public event Action<int, int> OnHPChanged;
        public event Action<bool> OnVulnerabilityChanged;

        #endregion

        #region Constructor

        public ArmorLogic(ArmorData data) {
            _gearHPs = data.GearHPs;
            _invulnerabilityTime = data.InvulnerabilityTime;
            Init();
        }

        #endregion

        #region Properties

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
                OnHPChanged?.Invoke(_hp, damage);
            }
        }
        public bool IsVulnerable
        {
            get => _isVulnerable;
            set
            {
                if (_isVulnerable == value) return;
                _isVulnerable = value;
                OnVulnerabilityChanged?.Invoke(value);
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
            OnHPChanged += AddInvulnerability;
        }

        public void Update(float deltaTimeInSeconds) {
            if (_invulnerabilityTimer != null) _invulnerabilityTimer.Update(deltaTimeInSeconds);
        }

        private void AddInvulnerability(int _, int damage)
        {
            if (damage <= 0) return;
            IsVulnerable = false;
            _invulnerabilityTimer = new Timer(_invulnerabilityTime);
            _invulnerabilityTimer.OnTimerEnd += () => {
                IsVulnerable = true;
                _invulnerabilityTimer = null;
            };
        }

        public void GenerateCumulativeHPs()
        {
            _cumulativeGearHPs = new int[_gearHPs.Length];
            int sum = 0;
            for (int i = 0; i < _gearHPs.Length; i++)
            {
                sum +=_gearHPs[i];
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