using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A class to handle the UI of the game, has references to various gameobjects and UI elements
/// </summary>
public class UIHandler : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    Text playerSpeed;

    [SerializeField]
    Transform shiftParent;

    [SerializeField]
    GameObject shiftPrefab;
    [SerializeField]
    GameObject HPPrefab;
    List<GameObject> shifts = new List<GameObject>();
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
    /// Sets the warningPanel to a shift color when non-newtonian cloud appears or whatever
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
        player.HPChangeEvent += OnHPChanged;
        player.ShiftChangeEvent += OnShiftChanged;
        CreateShifts();
    }
    /// <summary>
    /// Generates UI elements for each shift
    /// </summary>
    private void CreateShifts()
    {
        int k = 0;
        foreach (int hps in player.shiftHps)
        {
            GameObject shift = Instantiate(shiftPrefab, shiftParent);
            shift.transform.SetSiblingIndex(0);
            shift.GetComponent<Image>().color = player.shiftColors[k];

            shifts.Add(shift);
            CreateHPs(hps, shift);
            k++;
        }
    }
    /// <summary>
    /// Generates HPs for each shift
    /// </summary>
    /// <param name="hps">Number of HPs</param>
    /// <param name="shift">Shift GameObject (parent)</param>
    private void CreateHPs(int hps, GameObject shift)
    {
        for (int i = 0; i < hps; i++)
        {
            HPs.Add(Instantiate(HPPrefab, shift.transform));
            HPs[i].transform.SetSiblingIndex(0);
        }
    }

    /// <summary>
    /// Updates the HPs after taken damage/healed
    /// </summary>
    private void OnHPChanged() {
        for (int i = 0; i < HPs.Count; i++) {
            if (player.HP <= i) {
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
    private void OnShiftChanged() {
        for (int i = 0; i < shifts.Count; i++)
        {
            shifts[i].GetComponent<Image>().color = player.shiftColors[i];
        }
        shifts[player.CurrentShift].GetComponent<Image>().color = player.shiftColors[player.CurrentShift] * 2;
        speed.color = Color.white;
        StartCoroutine(Utility.ExecuteAfterTime(ChangeSpeedColor, 0.07f));
    }

    private void ChangeSpeedColor() {
        speed.color = player.shiftColors[player.CurrentShift] * 2;
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
        speed.fillAmount = player.speedTValue;

    }

    void UpdateAcceleration() {
        accel.fillAmount = Mathf.Sqrt(Mathf.InverseLerp(player.accelerationModifier / player.shiftNumber, player.accelerationModifier, player.CurrentAcceleration) + 0.1f);
        accel2.fillAmount = Mathf.InverseLerp(player.accelerationModifier, player.accelerationModifier * 2, player.CurrentAcceleration) / 2;
    }

    public void ShowFinishGame(string text) {
        endText.text = text;
        endPanel.SetActive(true);
    }
 
}
