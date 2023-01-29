using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nebuloic
{
    /// <summary>
    /// A class to handle the GUI of the game, has references to various gameobjects and UI elements
    /// </summary>
    public class GUIHandler : MonoBehaviour
    {
        [SerializeField] ShipBehaviour _ship;
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
        [SerializeField] Image speed2;
        [SerializeField] Image speedParent;

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
        public void SetWarning(Color color, float durationInSeconds)
        {
            warningPanel.GetComponent<Image>().color = color;
            if (resetWarning != null) StopCoroutine(resetWarning);
            resetWarning = StartCoroutine(Utility.ExecuteAfterTime(ResetWarning, durationInSeconds));
        }

        void ResetWarning()
        {
            warningPanel.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        private void Awake()
        {
            SetGUI(this);
            CreateGears();
        }
        static void SetGUI( GUIHandler gui ) => GameManager.guiHandler = gui;

        /// <summary>
        /// Generates UI elements for each gear
        /// </summary>
        private void CreateGears()
        {
            int k = 0;
            foreach (int hps in _ship.ShipData.ArmorData.GearHPs)
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
        public void UpdateHPs(int HPs, int _)
        {
            foreach (GearUI gear in gears)
            {
                HPs -= gear.UpdateHPs(HPs);
            }
        }
        /// <summary>
        /// Updates the current shift with a color
        /// </summary>
        public void UpdateGear(int gear, int _)
        {
            for (int i = 0; i < gears.Count; i++)
            {
                gears[i].SelectGear(i == gear);
            }
            speed.color = Color.white;
            StartCoroutine(Utility.ExecuteAfterTime(ChangeSpeedColor, 0.07f));
            speed2.fillAmount = 0;
        }

        private void ChangeSpeedColor()
        {
            speed.color = GearColorsSelected[_ship.Logic.Engine.CurrentGear];
            speed2.color = Color.white;
            speedParent.color = _gearColors[_ship.Logic.Engine.CurrentGear];
        }


        void Update()
        {
            _levelTime.text = string.Format("Time: {0:f2}s / 60.00s", time);
            UpdateAcceleration();
            UpdateSpeed();

            playerOnMap.transform.localPosition = new Vector3(playerOnMap.transform.localPosition.x, Mathf.Lerp(mapStart, mapEnd, tValueMap));
        }

        void UpdateSpeed()
        {
            speed.fillAmount = _ship.Logic.Engine.SpeedPercentage;
            speed2.fillAmount = Mathf.Lerp(speed2.fillAmount, _ship.Logic.Engine.SpeedPercentage, 0.2f);
        }

        void UpdateAcceleration()
        {
            accel.fillAmount = Mathf.Sqrt(_ship.Logic.Engine.AccelerationPercentage);
            accel2.fillAmount = _ship.Logic.Engine.AccelerationPercentage - 1;
        }

        public void ShowFinishGame(string text)
        {
            endText.text = text;
            endPanel.SetActive(true);
        }

    }
}