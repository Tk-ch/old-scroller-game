using Nebuloic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{

    [SerializeField] ShipData _shipData;
    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _limitLerpSpeed; //швидкість повернення до лімітів дороги
    [SerializeField] private float _horizontalLimits; //горизонтальні ліміти дороги
    [SerializeField] private float _horizontalRollLimits; //ліміти дороги під час бочки
    [SerializeField] private float _rollDurationInSeconds; //тривалість бочки
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed;

    private ShipLogic _shipLogic;
    public ShipLogic Logic => _shipLogic ?? (_shipLogic = new ShipLogic(_shipData));



    private bool _isRolling;
    public bool IsRolling
    {
        get { return _isRolling; }
        set
        {
            _isRolling = value;
            OnRollChanged?.Invoke();
        }
    }

    public float HorizontalInput { get; set; }

    public event Action OnRollChanged;

    public void Roll()
    {
        if (!IsRolling)
        {
            IsRolling = true;
            StartCoroutine(Utility.ExecuteAfterTime(ResetRoll, _rollDurationInSeconds));
        }
    }

    public bool Shoot()
    {
        if (!_shipLogic.Engine.CanShoot(_bulletPrefab.GetComponent<Projectile>().SpeedReduction)) return false;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.up * _bulletSpeed);
        _shipLogic.Engine.CurrentSpeed -= bullet.GetComponent<Projectile>().SpeedReduction;
        return _shipLogic.Engine.DecreaseGearBySpeed();
    }

    private void FixedUpdate()
    {
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


    private void Update()
    {
        _shipLogic.Update(Time.deltaTime);
    }

}
