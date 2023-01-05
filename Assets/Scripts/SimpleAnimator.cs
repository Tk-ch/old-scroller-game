using System.Collections;
using UnityEngine;

public class SimpleAnimator : MonoBehaviour
{
    [SerializeField] GameObject deceleration;
    [SerializeField] float decelerationShowTimeInSeconds;
    [SerializeField] Player player;
    [SerializeField] SpriteRenderer thruster;
    [SerializeField] Camera mainCamera;
    public float tValue;

    private void Start()
    {
        player.ShiftChangeEvent += UpdateThruster;
    }

    void UpdateThruster() {
        thruster.material.SetColor("_Color", player.shiftColors[player.CurrentShift]);
    }

    public void ShowDeceleration()
    {
        deceleration.SetActive(true);
        StartCoroutine(HideDeceleration());
    }

    public void ShakeCamera() {
        mainCamera.transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    IEnumerator HideDeceleration()
    {
        yield return new WaitForSeconds(decelerationShowTimeInSeconds);
        deceleration.SetActive(false); 
    }

    private void Update()
    {
        
        if (player.isRolling)
        {
            transform.Rotate(0, 360 * Time.deltaTime, 0);
        }
        thruster.material.SetFloat("_tValue", Mathf.Pow(Mathf.Lerp(thruster.material.GetFloat("_tValue"), tValue, 0.5f), 2));
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(0,0,-10), 0.1f);
    }
}
