using System;
using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    public class EngineLogic 
    {

        #region Fields

        private float _currentSpeed;
        private int _currentGear = 0;
        private bool _perfectSwitch;
        private readonly EngineData _engineData;
        private readonly ThrusterData _thrusterData;
        private readonly ArmorLogic _armor;


        public event EventHandler<int> GearChanged;
        public event Action PerfectSwitched;
        public event EventHandler<bool> PerfectSwitchChanged;

        #endregion

        public EngineLogic(ShipData data,  ArmorLogic armor ) {
            _engineData = data.EngineData;
            _armor = armor;
            _thrusterData = data.ThrusterData;
            _armor.HPChanged += RecheckGears;
            CurrentGear = 0;
            CurrentSpeed = _engineData.MinimumSpeed;
        }


        private void RecheckGears(int _, int damage)
        {
            if (damage <= 0) return;
            CurrentGear = Mathf.Min(_armor.MaxGear, CurrentGear);
        }


        public float CurrentAcceleration { get; set; } = 0;

        public float CurrentGearSpeed { get => _engineData.GearSpeeds[CurrentGear]; }
        public float CorrectGearSpeed { get => _engineData.GearSpeeds[CorrectGear]; }
        public float AccelerationPercentage { get => CurrentAcceleration / _thrusterData.AccelerationModifier; }
        public float CurrentSpeed
        {
            get => _currentSpeed;
            set => _currentSpeed = Mathf.Clamp(value, _engineData.MinimumSpeed, CurrentGearSpeed);
        }
        public float SpeedPercentage
        {
            get
            {
                float speedDifference = CurrentGear == 0 ? _engineData.MinimumSpeed : _engineData.GearSpeeds[CurrentGear - 1];
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
                    CurrentAcceleration += _thrusterData.PerfectSwitchBoost;
                    PerfectSwitched?.Invoke();
                    PerfectSwitch = false;
                }
                int diff = _currentGear - newGear;
                _currentGear = newGear;
                GearChanged?.Invoke(this, diff);
            }
        }
        public float Acceleration
        {
            get
            {
                if (Mathf.Abs(CurrentGearSpeed - CurrentSpeed) < 0.05f) return 0;
                return (CurrentGear == CorrectGear) ? _thrusterData.AccelerationModifier : _thrusterData.AccelerationModifier / (1 + Mathf.Abs(CurrentGear - CorrectGear));
            }
        }
        public int CorrectGear
        {
            get
            {
                if (CurrentSpeed < _engineData.GearSpeeds[0]) return 0;
                for (int i = 0; i < _engineData.GearSpeeds.Length - 2; i++)
                {
                    if (_engineData.GearSpeeds[i] < CurrentSpeed && _engineData.GearSpeeds[i + 1] > CurrentSpeed)
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
                PerfectSwitchChanged?.Invoke(this, value);
            }
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
            return CurrentSpeed - _engineData.MinimumSpeed >= speedReduction;
        }

        public void Update(float deltaTime)
        {
            CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, Acceleration, _thrusterData.AccelerationChangeSpeed * deltaTime);
            CurrentSpeed += CurrentAcceleration * deltaTime;

            if (Mathf.Abs(CurrentSpeed - CorrectGearSpeed) < 0.05f)
            {
                PerfectSwitch = true;
            }
        }
    }
}