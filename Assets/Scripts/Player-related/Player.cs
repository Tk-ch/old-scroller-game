using Nebuloic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Component to handle them inputs and have links to other components
/// </summary>
public class Player : MonoBehaviour
{
    private ShipLogic _ship;


    public ShipLogic Ship => _ship ?? ( _ship= GetComponent<ShipBehaviour>().Logic );

    [SerializeField] float holdDownTimeInSeconds;
    [SerializeField] float repeatShootingInSeconds;
    [SerializeField] public GUIHandler guiHandler;

    Coroutine _stopHoldDownCoro;
    Coroutine _repeatedShootingCoro;

    void Update()
    {
        GetInputs();
    }

    /// <summary>
    /// Gets all the needed inputs from the Input class and updates the variables
    /// </summary>
    private void GetInputs()
    {
        Ship.HorizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Roll") && !Ship.IsRolling)
        {
            Ship.Roll();
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
            Ship.Logic.Engine.CurrentGear += 1;
        }  
    }

    IEnumerator RepeatedShooting()
    {
        if (Ship.Shoot()) // gear has changed when shooting
        {
            StopCoroutine(_stopHoldDownCoro);
            _stopHoldDownCoro = StartCoroutine(StopHoldDown());
        }
        yield return new WaitForSeconds(repeatShootingInSeconds);
        _repeatedShootingCoro = StartCoroutine(RepeatedShooting());
    }

    IEnumerator StopHoldDown() {
        yield return new WaitForSeconds(holdDownTimeInSeconds);
        Ship.Logic.Engine.CurrentGear -= 1;
        _stopHoldDownCoro = StartCoroutine(StopHoldDown());
    }

}
