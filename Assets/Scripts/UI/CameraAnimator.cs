using System.Collections;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [SerializeField] Player player;

    private void Start()
    {
        player.ArmorComponent.OnHPChanged += ShakeCamera;
    }

    public void ShakeCamera() {
        transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    private void Update()
    {
        float accelerationInfluence = Mathf.Lerp(0, 2, Mathf.InverseLerp(0, 0.5f, player.EngineComponent.CurrentAcceleration));
        float speedInfluence = Mathf.Lerp(-2, -4, Mathf.Pow(Mathf.InverseLerp(3, 25, player.EngineComponent.CurrentSpeed), 0.3f));
        Vector2 cameraCoord = new Vector2(0, -(accelerationInfluence + speedInfluence));
        transform.position = Vector3.Lerp(transform.position, new Vector3(cameraCoord.x, cameraCoord.y, -10), 0.1f);
    }
}
