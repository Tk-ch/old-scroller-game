using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// Component that handles ship's position in space
/// </summary>
public class Ship : MonoBehaviour
{
    [SerializeField] private float _minimumVerticalSpeed; 
    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _limitLerpSpeed; //швидкість повернення до лімітів дороги
    [SerializeField] private float _horizontalLimits; //горизонтальні ліміти дороги
    [SerializeField] private float _horizontalRollLimits; //ліміти дороги під час бочки
    [SerializeField] private float _rollDurationInSeconds; //тривалість бочки
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed; 

    private Engine _engine;
    private float _currentVerticalSpeed;

    public float SpeedPercentage
    { // TODO fix the jumps in speed when the CurrentSpeed changes
        get
        {
            return CurrentVerticalSpeed / _engine.CurrentGearSpeed;
        }
    }
    public float CurrentVerticalSpeed { 
        get => _currentVerticalSpeed; 
        set => _currentVerticalSpeed = Mathf.Clamp(value, _minimumVerticalSpeed, _engine.CurrentGearSpeed); 
    }
    public bool IsRolling { get; set; }
    public float HorizontalInput { get; set; }
    
    public void Roll() {
        if (!IsRolling)
        {
            IsRolling = true;
            StartCoroutine(Utility.ExecuteAfterTime(ResetRoll, _rollDurationInSeconds));
        }
    }

    public bool Shoot()
    {
        if (CurrentVerticalSpeed - _minimumVerticalSpeed < _bulletPrefab.GetComponent<Projectile>().SpeedReduction) return false;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.up * _bulletSpeed);
        CurrentVerticalSpeed -= bullet.GetComponent<Projectile>().SpeedReduction;
        return _engine.DecreaseGearBySpeed();
    }

    private void Start()
    {
        _engine = GetComponent<Engine>();
    }


    private void FixedUpdate()
    {
        CurrentVerticalSpeed += _engine.CurrentAcceleration * Time.fixedDeltaTime;
        transform.Translate(new Vector2(HorizontalInput * _horizontalSpeed * Time.fixedDeltaTime, 0), Space.World);
        float diff = Mathf.Abs(transform.position.x) - _horizontalLimits;
        if (diff > 0)
        {
            if (!IsRolling)
            {
                transform.Translate(new Vector2(-diff * _limitLerpSpeed * Mathf.Sign(transform.position.x), 0), Space.World);
            }
            else if (Mathf.Abs(transform.position.x) > _horizontalRollLimits)
            {
                transform.position = new Vector2(-(_horizontalRollLimits - 0.1f) * Mathf.Sign(transform.position.x), transform.position.y);
            }
        }
    }

    void ResetRoll() => IsRolling = false;
    
}