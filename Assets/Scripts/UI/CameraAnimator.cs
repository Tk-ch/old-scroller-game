using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    public class CameraAnimator : MonoBehaviour
    {
        [SerializeField] ShipBehaviour ship;

        public void ShakeCamera(int _, int damage)
        {
            if (damage <= 0) return;
            transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        private void Update()
        {
            float accelerationInfluence = Mathf.Lerp(-2, 0, Mathf.InverseLerp(0, 2, ship.Logic.Engine.CurrentAcceleration));
            float speedInfluence = Mathf.Lerp(0, -1, Mathf.Pow(Mathf.InverseLerp(3, 25, ship.Logic.Engine.CurrentSpeed), 0.3f));
            Vector2 cameraCoord = new Vector2(0, -(accelerationInfluence + speedInfluence));
            transform.position = Vector3.Lerp(transform.position, new Vector3(cameraCoord.x, cameraCoord.y, -10), 0.1f);
        }
    }
}