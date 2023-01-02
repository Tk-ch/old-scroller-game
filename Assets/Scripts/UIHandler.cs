using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        player.HPChangeEvent += OnHPChanged;
        player.ShiftChangeEvent += OnShiftChanged;
        int k = 0;
        foreach (int el in player.shiftHps) {
            GameObject shift = Instantiate(shiftPrefab, shiftParent);
            shift.transform.SetSiblingIndex(0);
            shift.GetComponent<Image>().color = player.shiftColors[k];
            
            shifts.Add(shift);
            for (int i = 0; i < el; i++) {
                HPs.Add(Instantiate(HPPrefab, shift.transform));
                HPs[i].transform.SetSiblingIndex(0);
            }
            k++;
        }

    }

    private void OnHPChanged() {
        for (int i = 0; i < HPs.Count; i++) {
            if (player.HP <= i) {
                HPs[i].GetComponent<Image>().color = Color.black;
                HPs[i].transform.SetSiblingIndex(0);

            }
        }
    }

    private void OnShiftChanged() {
        for (int i = 0; i < shifts.Count; i++)
        {
            shifts[i].GetComponent<Image>().color = player.shiftColors[i];
        }
        shifts[player.CurrentShift].GetComponent<Image>().color = Color.white;
    }

    void Update()
    {
        playerSpeed.text = string.Format("Speed: {0}/{5}\nShift: {1}\nCorrect Shift:{2}\nHP: {3}\nAccel: {4}", player.CurrentSpeed, player.CurrentShift, player.CorrectShift, player.HP, player.Acceleration, player.MaxSpeed);    
    }

}