using System.Collections;
using UnityEngine;

public class ShipAnimator : MonoBehaviour
{
    [SerializeField] GameObject _deceleration;
    [SerializeField] float _decelerationShowTimeInSeconds;
    [SerializeField] SpriteRenderer _thruster;
    [SerializeField] Player _player;
    [SerializeField] float _horizontalRotationMultiplier;
    [SerializeField] float _blinkTime;

    public void ShowDeceleration(int _, int damage)
    {
        if (damage <= 0) return;
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
    }

    public void UpdateVulnerability(bool vuln) {
        if (!vuln) StartCoroutine(Utility.ExecuteAfterTime(Blink, _blinkTime));
        else SetShipAlpha(1);
        
    }

    void Blink() {
        var col = GetComponent<SpriteRenderer>().color;
        if (col.a == 0)
            SetShipAlpha(1);
        else
            SetShipAlpha(0);
        if (!_player.ArmorComponent.Armor.IsVulnerable) StartCoroutine(Utility.ExecuteAfterTime(Blink, _blinkTime));
        else SetShipAlpha(1);

    }

    void SetShipAlpha(float a)
    {
        var renderer = GetComponent<SpriteRenderer>();
        var col = renderer.color;
        col.a = a;
        renderer.color = col;
    }

    
    

    public void UpdateThruster(int gear, int _)
    {
        _thruster.material.SetColor("_Color", _player.guiHandler.GearColorsSelected[gear]);
    }

    private void Update()
    {
        if (_player.Ship.IsRolling)
        {
            transform.Rotate(0, 720 * Time.deltaTime, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity * Quaternion.Euler(new Vector3(0, _player.Ship.HorizontalInput * _horizontalRotationMultiplier));
        }
        _thruster.material.SetFloat("_tValue", Mathf.Pow(Mathf.Clamp01(Mathf.Lerp(_thruster.material.GetFloat("_tValue"), _player.EngineComponent.Engine.SpeedPercentage, 0.5f)) + 0.1f, 2));
    }
}
