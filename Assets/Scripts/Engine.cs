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

    [SerializeField] float perfectSwitchTiming;
    [SerializeField] float perfectSwitchBoost;

    private float _currentSpeed;
    private int _currentGear = 0;
    private Armor _armorComponent;

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
            if (ArmorComponent.CheckGearHP(value)) {
                _currentGear = Mathf.Clamp(value, 0, _gearSpeeds.Length - 1);
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


    public bool DecreaseGearBySpeed() {
        if (CorrectGear >= CurrentGear) return false;
        CurrentGear--;
        return true;    
    }

    private void Start()
    {
        OnGearChanged?.Invoke();
    }

    private void FixedUpdate()
    {
        CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, Acceleration, _accelerationChangeSpeed * Time.fixedDeltaTime);
        CurrentSpeed += CurrentAcceleration * Time.fixedDeltaTime;
    }
}
