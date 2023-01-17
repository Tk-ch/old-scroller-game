using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearUI : MonoBehaviour
{
    [SerializeField] GameObject _HPPrefab;
    public Color gearColor { get; set; }
    public Color gearColorSelected { get; set; }

    List<GameObject> _HPs = new List<GameObject>();


    public void CreateHPs(int hps)
    {
        for (int i = 0; i < hps; i++)
        {
            _HPs.Add(Instantiate(_HPPrefab, transform));
            _HPs[i].transform.SetSiblingIndex(0);
        }
    }

    public int UpdateHPs(int hps) {
        for (int i = 0; i < _HPs.Count; i++)
        {
            if (hps < i) {
                _HPs[i].GetComponent<Image>().color = Color.black;
                _HPs[i].transform.SetSiblingIndex(0);
            }
            else
            {
                _HPs[i].GetComponent<Image>().color = Color.green;
            }
        }
        return _HPs.Count;
    }

    public void SelectGear(bool selected) {
         GetComponent<Image>().color = selected ? gearColorSelected : gearColor;
    }

}
