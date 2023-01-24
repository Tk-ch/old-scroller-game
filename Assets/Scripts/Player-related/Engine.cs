using System;
using UnityEngine;

namespace Nebuloic
{
    [CreateAssetMenu(fileName = "Engine", menuName = "Scribptable Objects/Engine", order = 1)]
    public class Engine : ScriptableObject
    {

        [SerializeField] float[] _gearSpeeds;
        [SerializeField] float _minimumSpeed;

        [SerializeField] float _accelerationModifier;
        [SerializeField] float _accelerationChangeSpeed = 0.1f;

        [SerializeField] public float PerfectSwitchTiming;
        [SerializeField] float _perfectSwitchBoost;

        private float _currentSpeed;
        private int _currentGear = 0;
        private bool _perfectSwitch;

        public Armor Armor { get; private set; }


        public void Init(Armor armor) {
            Armor = armor;
            armor.HPChanged += RecheckGears;
            CurrentGear = 0;
            CurrentSpeed = _minimumSpeed;
        }

        private void RecheckGears(object _, int damage) {
            if (damage <= 0) return;
            CurrentGear = Mathf.Min(Armor.MaxGear, CurrentGear);
        }

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
                int newGear = Mathf.Clamp(value, 0, Armor.MaxGear);
                if (PerfectSwitch && newGear > _currentGear)
                {
                    CurrentAcceleration += _perfectSwitchBoost;
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
        public float CurrentAcceleration { get; set; } = 0;

        public float CurrentGearSpeed { get => _gearSpeeds[CurrentGear]; }
        public float CorrectGearSpeed { get => _gearSpeeds[CorrectGear]; }
        public float AccelerationPercentage { get => CurrentAcceleration / _accelerationModifier; }
        public bool PerfectSwitch { get => _perfectSwitch;
            set
            {
                if (_perfectSwitch == value) return;
                _perfectSwitch = value;
                PerfectSwitchChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<int> GearChanged;
        public event Action PerfectSwitched;
        public event EventHandler<bool> PerfectSwitchChanged;

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

        public void UpdateEngine(float deltaTime)
        {
            CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, Acceleration, _accelerationChangeSpeed * deltaTime);
            CurrentSpeed += CurrentAcceleration * deltaTime;

            if (Mathf.Abs(CurrentSpeed - CorrectGearSpeed) < 0.05f)
            {
                PerfectSwitch = true;
            }
        }
    }
}