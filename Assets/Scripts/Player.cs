using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to handle them inputs and have links to other components
/// </summary>
public class Player : MonoBehaviour
{   

    public Ship ShipComponent { get; set; }
    public Engine EngineComponent { get; set; }
    public Armor ArmorComponent { get; set; }

    [SerializeField] SimpleAnimator myAnimator;
    [SerializeField] float holdDownTimeInSeconds;
    [SerializeField] float repeatShootingInSeconds;

    Coroutine _stopHoldDownCoro;
    Coroutine _repeatedShootingCoro;

    private void Start()
    {
        ShipComponent = GetComponent<Ship>(); 
        EngineComponent = GetComponent<Engine>();
        ArmorComponent = GetComponent<Armor>();
    }

    void Update()
    {
        GetInputs();
    }

    /// <summary>
    /// Gets all the needed inputs from the Input class and updates the variables
    /// </summary>
    private void GetInputs()
    {
        ShipComponent.HorizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Roll") && !ShipComponent.IsRolling)
        {
            ShipComponent.Roll();
        }
        if (Input.GetButtonDown("ShiftDown"))
        {
            _stopHoldDownCoro = StartCoroutine(StopHoldDown());
            _repeatedShootingCoro = StartCoroutine(RepeatedShooting());
        }
        if (Input.GetButtonUp("ShiftDown"))
        {
            if (_stopHoldDownCoro != null) StopCoroutine(_stopHoldDownCoro);
            if (_repeatedShootingCoro != null) StopCoroutine(_repeatedShootingCoro);
        }
        if (Input.GetButtonDown("ShiftUp"))
        {
            EngineComponent.CurrentGear += 1;
        }  
    }

    IEnumerator RepeatedShooting()
    {
        if (ShipComponent.Shoot()) // gear has changed when shooting
        {
            StopCoroutine(_stopHoldDownCoro);
            _stopHoldDownCoro = StartCoroutine(StopHoldDown());
        }
        yield return new WaitForSeconds(repeatShootingInSeconds);
        _repeatedShootingCoro = StartCoroutine(RepeatedShooting());
        _stopHoldDownCoro = StartCoroutine(StopHoldDown());
    }

    IEnumerator StopHoldDown() {
        yield return new WaitForSeconds(holdDownTimeInSeconds);
        EngineComponent.CurrentGear -= 1;
        _stopHoldDownCoro = StartCoroutine(StopHoldDown());
    }

}
