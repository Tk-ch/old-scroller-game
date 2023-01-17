using System;
using UnityEngine;

/// <summary>
/// Component to manage ship's gears - the main point is to calculate acceleration and speed here and then use it in Ship class. 
/// </summary>
public class Engine : MonoBehaviour
{
    [SerializeField] float[] _gearSpeeds;
    [SerializeField] float _minimumSpeed;

    [SerializeField] float _accelerationModifier;
    [SerializeField] float _accelerationChangeSpeed = 0.1f;

    [SerializeField] float _perfectSwitchTiming;
    [SerializeField] float _perfectSwitchBoost;

    private float _currentSpeed;
    private int _currentGear = 0;
    private Armor _armorComponent;
    private bool _perfectSwitch;

    Armor ArmorComponent => _armorComponent ?? (_armorComponent = GetComponent<Armor>());

    public float CurrentSpeed
    {
        get => _currentSpeed;
        set => _currentSpeed = Mathf.Clamp(value, _minimumSpeed, CurrentGearSpeed);
    }
    public float SpeedPercentage
    { // TODO fix the jumps in speed when the CurrentSpeed changes
        get
        {
            float speedDifference = CurrentGear == 0 ? _minimumSpeed : _gearSpeeds[CurrentGear - 1];
            return (CurrentSpeed - speedDifference) / (CurrentGearSpeed - speedDifference);
        }
    }

    public int CurrentGear {  // Clamped between 0 and gear number
        get => _currentGear; 
        set { 
            int newGear = Mathf.Clamp(value, 0, _gearSpeeds.Length - 1);
            if (ArmorComponent.CheckGearHP(newGear)) {

                if (_perfectSwitch && newGear > _currentGear)
                {
                    CurrentAcceleration += _perfectSwitchBoost;
                    OnPerfectSwitch?.Invoke();
                    _perfectSwitch = false;
                }
                _currentGear = newGear;
                OnGearChanged?.Invoke();
            }
        }
    }
    public float Acceleration
    {
        get
        {
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

    public event Action OnGearChanged;
    public event Action OnPerfectSwitch;

    // Decreases gear by one if correct gear is lower than current gear
    public bool DecreaseGearBySpeed() {
        if (CorrectGear >= CurrentGear) return false;
        CurrentGear--;
        return true;    
    }

    // Checks if ship can shoot at current speed
    public bool CanShoot(float speedReduction) {
        return CurrentSpeed - _minimumSpeed >= speedReduction;
    }

    private void Start()
    {
        OnGearChanged?.Invoke();
    }

    private void FixedUpdate()
    {
        CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, Acceleration, _accelerationChangeSpeed * Time.fixedDeltaTime);
        CurrentSpeed += CurrentAcceleration * Time.fixedDeltaTime;

        if (Mathf.Abs(CurrentSpeed - CorrectGearSpeed) < 0.05f) {
            _perfectSwitch = true;
            StartCoroutine(Utility.ExecuteAfterTime(ResetPerfectSwitch, _perfectSwitchTiming));
        }
    }

    private void ResetPerfectSwitch() {
        _perfectSwitch = false;
    }
}
