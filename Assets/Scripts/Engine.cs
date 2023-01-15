using System;
using UnityEngine;

public class Engine : MonoBehaviour
{
    // Serialized stuff
    [SerializeField] float[] _gearSpeeds;
    [SerializeField] float _accelerationModifier;


    // Private stuff
    int _currentGear;
    
    
    // Properties
    public int CurrentGear {
        get => _currentGear; 
        set {
            
            if (TryGetComponent(out Armor a) && a.CheckGearHP(value)) {
                _currentGear = value;
                OnGearChanged?.Invoke();
            }
        }
    }

    // Events
    public event Action OnGearChanged;



}
