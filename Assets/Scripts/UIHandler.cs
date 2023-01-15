using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A class to handle the UI of the game, has references to various gameobjects and UI elements
/// </summary>
public class UIHandler : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] public Color[] gearColors;

    [SerializeField] Text playerSpeed;

    [SerializeField] Transform gearParent;

    [SerializeField] GameObject gearPrefab;
    [SerializeField] GameObject HPPrefab;
    List<GameObject> gears = new List<GameObject>();
    List<GameObject> HPs = new List<GameObject>();

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

    /// <summary>
    /// Sets the warningPanel to a gear color when non-newtonian cloud appears or whatever
    /// </summary>
    /// <param name="color">The color to set :) </param>
    /// <param name="durationInSeconds">Duration before it is reset to white</param>
    public void SetWarning(Color color, float durationInSeconds) {
        warningPanel.GetComponent<Image>().color = color;
        if (resetWarning != null) StopCoroutine(resetWarning);
        resetWarning = StartCoroutine(ResetWarning(durationInSeconds));
    }

    IEnumerator ResetWarning(float duration) {
        yield return new WaitForSeconds(duration);
        warningPanel.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    private void Start()
    {
        player.ArmorComponent.OnHPChanged += OnHPChanged;
        player.EngineComponent.OnGearChanged += OnGearChanged;
        CreateGears();
    }
    /// <summary>
    /// Generates UI elements for each gear
    /// </summary>
    private void CreateGears()
    {
        int k = 0;
        foreach (int hps in player.ArmorComponent.GearHPs)
        {
            GameObject gear = Instantiate(gearPrefab, gearParent);
            gear.transform.SetSiblingIndex(0);
            gear.GetComponent<Image>().color = gearColors[k];

            gears.Add(gear);
            CreateHPs(hps, gear);
            k++;
        }
    }
    /// <summary>
    /// Generates HPs for each gear
    /// </summary>
    /// <param name="hps">Number of HPs</param>
    /// <param name="gear">Shift GameObject (parent)</param>
    private void CreateHPs(int hps, GameObject gear)
    {
        for (int i = 0; i < hps; i++)
        {
            HPs.Add(Instantiate(HPPrefab, gear.transform));
            HPs[i].transform.SetSiblingIndex(0);
        }
    }

    /// <summary>
    /// Updates the HPs after taken damage/healed
    /// </summary>
    private void OnHPChanged() {
        for (int i = 0; i < HPs.Count; i++) {
            if (player.ArmorComponent.HP <= i) {
                HPs[i].GetComponent<Image>().color = Color.black;
                HPs[i].transform.SetSiblingIndex(0);

            } else
            {
                HPs[i].GetComponent<Image>().color = Color.green;
            }
        }
    }
    /// <summary>
    /// Updates the current shift with a color
    /// </summary>
    private void OnGearChanged() {
        for (int i = 0; i < gears.Count; i++)
        {
            gears[i].GetComponent<Image>().color = gearColors[i];
        }
        gears[player.EngineComponent.CurrentGear].GetComponent<Image>().color = gearColors[player.EngineComponent.CurrentGear] * 2;
        speed.color = Color.white;
        StartCoroutine(Utility.ExecuteAfterTime(ChangeSpeedColor, 0.07f));
    }

    private void ChangeSpeedColor() {
        speed.color = gearColors[player.EngineComponent.CurrentGear] * 2;
    }

    void Update()
    {
        //Some debug text values
        playerSpeed.text = string.Format("Time: {0:f2}s / 60.00s", time);
        UpdateAcceleration();
        UpdateSpeed();
        
        playerOnMap.transform.localPosition = new Vector3(playerOnMap.transform.localPosition.x, Mathf.Lerp(mapStart, mapEnd, tValueMap));   
    }

    void UpdateSpeed() {
        speed.fillAmount = player.ShipComponent.SpeedPercentage;

    }

    void UpdateAcceleration() {
        accel.fillAmount = Mathf.Sqrt(player.EngineComponent.AccelerationPercentage);
        accel2.fillAmount = player.EngineComponent.AccelerationPercentage - 1;
    }

    public void ShowFinishGame(string text) {
        endText.text = text;
        endPanel.SetActive(true);
    }
 
}
