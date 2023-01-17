using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Component to manage ship's HP and check whether the shift can be increased. 
/// </summary>
public class Armor : MonoBehaviour
{

    [SerializeField] private int[] _gearHPs;

    public int[] GearHPs { get => _gearHPs; }

    private int[] _cumulativeGearHPs;
    private int _hp;

    public int HP 
    { 
        get => _hp;
        set 
        {
            int index = Mathf.Max(0, _cumulativeGearHPs.Length - 1); 
            _hp = Mathf.Clamp(value, 0, _cumulativeGearHPs[index]);
            OnHPChanged?.Invoke();
        } 
    }

    public event Action OnHPChanged;

    private void Start()
    {
        GenerateCumulativeHPs();
    }

    private void GenerateCumulativeHPs()
    {
        _cumulativeGearHPs = new int[_gearHPs.Length];
        int sum = 0;
        for (int i = 0; i < _gearHPs.Length; i++)
        {
            sum += _gearHPs[i];
            _cumulativeGearHPs[i] = sum;
        }
        HP = sum;
    }


    /// <summary>
    /// Method to check if the gear can be shifted to the input gear. 
    /// It can only be done if the current HP of the armor is greater than the cumulative HP of the gear.
    /// </summary>
    /// <param name="gear">The gear index</param>
    /// <returns>Whether the gear can be changed to the input gear</returns>
    public bool CheckGearHP(int gear) {
        gear = Mathf.Clamp(gear, 1, _cumulativeGearHPs.Length - 1); 
        return HP > _cumulativeGearHPs[gear - 1];
    }
}