using UnityEngine;


/// <summary>
/// Component that handles ship's position in space
/// </summary>
public class Ship : MonoBehaviour
{

    [SerializeField] private float _minimumVerticalSpeed;
    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _limitLerpSpeed;

    private Engine _engine;

    public float CurrentVerticalSpeed { get; set; }
    public bool IsRolling { get; set; }


    private void Start()
    {
        _engine = GetComponent<Engine>();
    }

    private void FixedUpdate()
    {
        CurrentVerticalSpeed += _engine.CurrentAcceleration * Time.fixedDeltaTime;

    }

}