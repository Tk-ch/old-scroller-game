using System.Collections;
using UnityEngine;

public class SimpleAnimator : MonoBehaviour
{
    [SerializeField] GameObject deceleration;
    [SerializeField] float decelerationShowTimeInSeconds;
    [SerializeField] Player player;
    [SerializeField] SpriteRenderer thruster;
    [SerializeField] Camera mainCamera;

    private void Start()
    {
        player.EngineComponent.OnGearChanged += UpdateThruster;
        player.ArmorComponent.OnHPChanged += ShakeCamera;
    }

    void UpdateThruster() {
        //thruster.material.SetColor("_Color", player.shiftColors[player.CurrentShift]);
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
        float accelerationInfluence = Mathf.Lerp(0, 2, Mathf.InverseLerp(0, 0.5f, player.EngineComponent.CurrentAcceleration));
        float speedInfluence = Mathf.Lerp(-4, -5, Mathf.Pow(Mathf.InverseLerp(3, 25, player.EngineComponent.CurrentSpeed), 0.3f));
        player.transform.position = new Vector2(player.transform.position.x, accelerationInfluence + speedInfluence);

        if (player.ShipComponent.IsRolling)
        {
            transform.Rotate(0, 720 * Time.deltaTime, 0);
        } else
        {
            transform.rotation = Quaternion.identity;
        }
        thruster.material.SetFloat("_tValue", Mathf.Pow(Mathf.Lerp(thruster.material.GetFloat("_tValue"), player.EngineComponent.SpeedPercentage, 0.5f), 2));
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(0,0,-10), 0.1f);
    }
}
