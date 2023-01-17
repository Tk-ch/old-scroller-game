using System;
using UnityEngine;

/// <summary>
/// Component to manage ship's gears - the main point is to calculate acceleration here and then use it in Ship class. 
/// Changing the thing to return speed instead of acceleration
/// </summary>
public class Engine : MonoBehaviour
{
    [SerializeField] float[] _gearSpeeds;

    [SerializeField] private float _minimumSpeed;
    
    private float _currentSpeed;
    public float CurrentSpeed
    {
        get => _currentSpeed;
        set => _currentSpeed = Mathf.Clamp(value, _minimumSpeed, CurrentGearSpeed);
    }
    public float SpeedPercentage
    { // TODO fix the jumps in speed when the CurrentSpeed changes
        get
        {
            return CurrentSpeed / CurrentGearSpeed;
        }
    }



    [SerializeField] float _accelerationModifier;
    [SerializeField] float _accelerationChangeSpeed = 0.1f;

    [SerializeField] float perfectSwitchTiming;
    [SerializeField] float perfectSwitchBoost;



    private int _currentGear = 0;
    private Ship _shipComponent;
    private Armor _armorComponent;
    
    public int CurrentGear {  // Clamped between 0 and gear number
        get => _currentGear; 
        set { 
            if (_armorComponent.CheckGearHP(value)) {
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
    public float CurrentAcceleration { get; set; } = 0; // needs to be public for 

    public float CurrentGearSpeed { get => _gearSpeeds[CurrentGear]; }
    public float CorrectGearSpeed { get => _gearSpeeds[CorrectGear]; }
    public float AccelerationPercentage { get => CurrentAcceleration / _accelerationModifier; }

    public event Action OnGearChanged;


    public bool DecreaseGearBySpeed() {
        if (CorrectGear < CurrentGear) return false;
        CurrentGear--;
        return true;    
    }


    private void Start()
    {
        OnGearChanged?.Invoke();
        _armorComponent = GetComponent<Armor>();
        _shipComponent = GetComponent<Ship>();
    }

    private void FixedUpdate()
    {
        CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, Acceleration, _accelerationChangeSpeed * Time.fixedDeltaTime);
        CurrentSpeed += CurrentAcceleration * Time.fixedDeltaTime;
    }
}
