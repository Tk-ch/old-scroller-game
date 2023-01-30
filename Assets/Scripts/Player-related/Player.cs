using Nebuloic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Component to handle them inputs and have links to other components
/// </summary>
public class Player : MonoBehaviour
{

    public static Player instance { get; private set; }

    private ShipLogic _ship;
    private ShipBehaviour  _shipBehaviour;

    public ShipBehaviour ShipBehaviour => _shipBehaviour ?? (_shipBehaviour = GetComponent<ShipBehaviour>()); 
    public ShipLogic Ship => _ship ?? ( _ship = ShipBehaviour.Logic );

    [SerializeField] float holdDownTimeInSeconds;
    [SerializeField] float repeatShootingInSeconds;

    Coroutine _stopHoldDownCoro;
    Coroutine _repeatedShootingCoro;

    private void Awake()
    {
        instance = this;
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
        ShipBehaviour.HorizontalInput = Input.GetAxis("Horizontal");
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
            Ship.Engine.CurrentGear += 1;
        }  
    }

    IEnumerator RepeatedShooting()
    {
        if (ShipBehaviour.Shoot()) // gear has changed when shooting
        {
            StopCoroutine(_stopHoldDownCoro);
            _stopHoldDownCoro = StartCoroutine(StopHoldDown());
        }
        yield return new WaitForSeconds(repeatShootingInSeconds);
        _repeatedShootingCoro = StartCoroutine(RepeatedShooting());
    }

    IEnumerator StopHoldDown() {
        yield return new WaitForSeconds(holdDownTimeInSeconds);
        Ship.Engine.CurrentGear -= 1;
        _stopHoldDownCoro = StartCoroutine(StopHoldDown());
    }

}
