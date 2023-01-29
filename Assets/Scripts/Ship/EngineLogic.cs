using System;
using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    public class EngineLogic 
    {

        #region Fields

        private readonly float[] _gearSpeeds;
        private readonly float _minimumSpeed;

        private readonly float _accelerationModifier;
        private readonly float _accelerationChangeSpeed;

        private readonly float _perfectSwitchBoost;

        private float _currentSpeed;
        private int _currentGear = 0;
        private bool _perfectSwitch;
        private Timer _perfectSwitchTimer;
        private readonly ArmorLogic _armor;
        private readonly float _perfectSwitchTiming;


        public event Action<int, int> OnGearChanged;
        public event Action OnPerfectSwitch;
        public event Action<bool> OnPerfectSwitchChanged;

        #endregion

        #region Constructor
        public EngineLogic( ShipData data, ArmorLogic armor, float perfectSwitchTiming ) {
            _gearSpeeds = data.EngineData.GearSpeeds;
            _minimumSpeed = data.EngineData.MinimumSpeed;
            _accelerationModifier = data.ThrusterData.AccelerationModifier;
            _accelerationChangeSpeed = data.ThrusterData.AccelerationChangeSpeed;
            _perfectSwitchBoost = data.ThrusterData.PerfectSwitchBoost;
            _perfectSwitchTiming = perfectSwitchTiming;
            _armor = armor;
            _armor.OnHPChanged += RecheckGears;
            CurrentGear = 0;
            CurrentSpeed = _minimumSpeed;
        }

        #endregion



        #region Properties

        public float CurrentAcceleration { get; set; } = 0;

        public float CurrentGearSpeed { get =>_gearSpeeds[CurrentGear]; }
        public float CorrectGearSpeed { get => _gearSpeeds[CorrectGear]; }
        public float AccelerationPercentage { get => CurrentAcceleration / _accelerationModifier; }
        public float CurrentSpeed
        {
            get => _currentSpeed;
            set => _currentSpeed = Mathf.Clamp(value, _minimumSpeed, CurrentGearSpeed);
        }
        public float SpeedPercentage
        {
            get
            {
                float speedDifference = CurrentGear == 0 ? _minimumSpeed : _gearSpeeds[CurrentGear - 1];
                return (CurrentSpeed - speedDifference) / (CurrentGearSpeed - speedDifference);
            }
        }

        public int CurrentGear
        {  // Clamped between 0 and gear number
            get => _currentGear;
            set
            {
                int newGear = Mathf.Clamp(value, 0, _armor.MaxGear);
                if (PerfectSwitch && newGear > _currentGear)
                {
                    CurrentAcceleration += _perfectSwitchBoost;
                    OnPerfectSwitch?.Invoke();
                    PerfectSwitch = false;
                }
                int diff = _currentGear - newGear;
                _currentGear = newGear;
                OnGearChanged?.Invoke(_currentGear, diff);
            }
        }
        public float Acceleration
        {
            get
            {
                if (Mathf.Abs(CurrentGearSpeed - CurrentSpeed) < 0.05f) return 0;
                return (CurrentGear == CorrectGear) ? _accelerationModifier : _accelerationModifier / (1 + Mathf.Abs(CurrentGear - CorrectGear));
            }
        }
        public int CorrectGear
        {
            get
            {
                if (CurrentSpeed < _gearSpeeds[0]) return 0;
                for (int i = 0; i < _gearSpeeds.Length - 2; i++)
                {
                    if (_gearSpeeds[i] < CurrentSpeed && _gearSpeeds[i + 1] > CurrentSpeed)
                    {
                        return i + 1;
                    }
                }
                return CurrentGear;
            }
        }
        public bool PerfectSwitch
        {
            get => _perfectSwitch;
            set
            {
                if (_perfectSwitch == value) return;
                _perfectSwitch = value;
                OnPerfectSwitchChanged?.Invoke(value);
            }
        }
        #endregion


        #region Methods

        private void RecheckGears(int _, int damage)
        {
            if (damage <= 0) return;
            CurrentGear = Mathf.Min(_armor.MaxGear, CurrentGear);
        }

        // Decreases gear by one if correct gear is lower than current gear
        public bool DecreaseGearBySpeed()
        {
            if (CorrectGear >= CurrentGear) return false;
            CurrentGear--;
            return true;
        }

        // Checks if ship can shoot at current speed
        public bool CanShoot(float speedReduction)
        {
            return CurrentSpeed - _minimumSpeed >= speedReduction;
        }

        public void Update(float deltaTime)
        {
            CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, Acceleration, _accelerationChangeSpeed * deltaTime);
            CurrentSpeed += CurrentAcceleration * deltaTime;

            _perfectSwitchTimer?.Update(deltaTime);
            if (Mathf.Abs(CurrentSpeed - CorrectGearSpeed) < 0.05f)
            {
                PerfectSwitch = true;
                _perfectSwitchTimer = new Timer(_perfectSwitchTiming);
                _perfectSwitchTimer.OnTimerEnd += () => PerfectSwitch = false;
            }
        }
        #endregion
    }
}