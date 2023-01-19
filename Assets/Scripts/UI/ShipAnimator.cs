using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAnimator : MonoBehaviour
{
    [SerializeField] GameObject _deceleration;
    [SerializeField] float _decelerationShowTimeInSeconds;
    [SerializeField] SpriteRenderer _thruster;
    [SerializeField] Player _player;
    [SerializeField] float _horizontalRotationMultiplier;
     
    public void ShowDeceleration()
    {
        _deceleration.SetActive(true);
        StartCoroutine(HideDeceleration());
    }

    IEnumerator HideDeceleration()
    {
        yield return new WaitForSeconds(_decelerationShowTimeInSeconds);
        _deceleration.SetActive(false);
    }

    private void Start()
    {
        _player.EngineComponent.OnGearChanged += UpdateThruster;
        _player.EngineComponent.OnGearDecreased += ShowDeceleration;

    }
    void UpdateThruster()
    {
        _thruster.material.SetColor("_Color", _player.guiHandler.GearColorsSelected[_player.EngineComponent.CurrentGear]);
    }
    private void Update()
    {
        if (_player.ShipComponent.IsRolling)
        {
            transform.Rotate(0, 720 * Time.deltaTime, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity * Quaternion.Euler(new Vector3(0, _player.ShipComponent.HorizontalInput * _horizontalRotationMultiplier));
        }
        _thruster.material.SetFloat("_tValue", Mathf.Pow(Mathf.Clamp01(Mathf.Lerp(_thruster.material.GetFloat("_tValue"), _player.EngineComponent.SpeedPercentage, 0.5f)) + 0.1f, 2));
    }
}
