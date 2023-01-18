using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A class to handle the UI of the game, has references to various gameobjects and UI elements
/// </summary>
public class GUIHandler : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] public Color[] _gearColors;
    [SerializeField] private Color[] _gearColorsSelected;
    [SerializeField] Text _levelTime;
    [SerializeField] Transform gearParent;
    [SerializeField] GameObject gearPrefab;
    [SerializeField] GameObject HPPrefab;
    private List<GearUI> gears = new List<GearUI>();

    [SerializeField] GameObject warningPanel;
    Coroutine resetWarning;

    [SerializeField] Image accel;
    [SerializeField] Image accel2;
    [SerializeField] Image speed;

    [SerializeField] GameObject endPanel;
    [SerializeField] Text endText;

    [SerializeField] float mapStart;
    [SerializeField] float mapEnd;
    [SerializeField] GameObject playerOnMap;
    public float tValueMap;

    public float time;

    public Color[] GearColorsSelected { get => _gearColorsSelected; set => _gearColorsSelected = value; }

    /// <summary>
    /// Sets the warningPanel to a gear color when non-newtonian cloud appears or whatever
    /// </summary>
    /// <param name="color">The color to set :) </param>
    /// <param name="durationInSeconds">Duration before it is reset to white</param>
    public void SetWarning(Color color, float durationInSeconds) {
        warningPanel.GetComponent<Image>().color = color;
        if (resetWarning != null) StopCoroutine(resetWarning);
        resetWarning = StartCoroutine(Utility.ExecuteAfterTime(ResetWarning, durationInSeconds));
    }

    void ResetWarning() {
        warningPanel.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    private void Start()
    {
        _player.ArmorComponent.OnHPChanged += OnHPChanged;
        _player.EngineComponent.OnGearChanged += OnGearChanged;
        CreateGears();
    }
    /// <summary>
    /// Generates UI elements for each gear
    /// </summary>
    private void CreateGears()
    {
        int k = 0;
        foreach (int hps in _player.ArmorComponent.GearHPs)
        {
            GameObject gear = Instantiate(gearPrefab, gearParent);
            gear.transform.SetSiblingIndex(0);
            gear.GetComponent<Image>().color = _gearColors[k];
            GearUI gearUI = gear.GetComponent<GearUI>();
            gearUI.gearColor = _gearColors[k];
            gearUI.gearColorSelected = GearColorsSelected[k];
            gearUI.CreateHPs(hps);

            gears.Add(gearUI);
            k++;
        }
    }
    /// <summary>
    /// Updates the HPs after taken damage/healed
    /// </summary>
    private void OnHPChanged() {
        int HPs = _player.ArmorComponent.HP;
        foreach (GearUI gear in gears)
        {
            HPs -= gear.UpdateHPs(HPs);
        }
    }
    /// <summary>
    /// Updates the current shift with a color
    /// </summary>
    private void OnGearChanged() {
        for (int i = 0; i < gears.Count; i++)
        {
            gears[i].SelectGear(i == _player.EngineComponent.CurrentGear);
        }
        speed.color = Color.white;
        StartCoroutine(Utility.ExecuteAfterTime(ChangeSpeedColor, 0.07f));
    }

    private void ChangeSpeedColor() => speed.color = GearColorsSelected[_player.EngineComponent.CurrentGear];
    

    void Update()
    {
        _levelTime.text = string.Format("Time: {0:f2}s / 60.00s", time);
        UpdateAcceleration();
        UpdateSpeed();
        
        playerOnMap.transform.localPosition = new Vector3(playerOnMap.transform.localPosition.x, Mathf.Lerp(mapStart, mapEnd, tValueMap));   
    }

    void UpdateSpeed() {
        speed.fillAmount = _player.EngineComponent.SpeedPercentage;

    }

    void UpdateAcceleration() {
        accel.fillAmount = Mathf.Sqrt(_player.EngineComponent.AccelerationPercentage);
        accel2.fillAmount = _player.EngineComponent.AccelerationPercentage - 1;
    }

    public void ShowFinishGame(string text) {
        endText.text = text;
        endPanel.SetActive(true);
    }
 
}
