using System;
using UnityEngine;

/// <summary>
/// A class to manage ship's gears - the main point is to calculate acceleration here and then use it in Ship class. 
/// </summary>
public class Engine : MonoBehaviour
{
    [SerializeField] float[] _gearSpeeds;
    [SerializeField] float _accelerationModifier;
    [SerializeField] float _accelerationChangeSpeed = 0.1f;

    private int _currentGear = 0;
    private Ship _shipComponent;
    private Armor _armorComponent;
    
    

    public int CurrentGear {
        get => _currentGear; 
        set { 
            if (_armorComponent.CheckGearHP(value)) {
                _currentGear = value;
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
            if (_shipComponent.CurrentVerticalSpeed < _gearSpeeds[0]) return 0;
            for (int i = 0; i < _gearSpeeds.Length - 2; i++)
            {
                if (_gearSpeeds[i] < _shipComponent.CurrentVerticalSpeed && _gearSpeeds[i + 1] > _shipComponent.CurrentVerticalSpeed)
                {
                    return i + 1;
                }
            }
            return CurrentGear;
        }
    }
    public float CurrentAcceleration { get; set; } = 0;

    public event Action OnGearChanged;

    private void Start()
    {

        _shipComponent = GetComponent<Ship>();
    }

    private void FixedUpdate()
    {
        CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, Acceleration, _accelerationChangeSpeed * Time.fixedDeltaTime);            
    }
}
