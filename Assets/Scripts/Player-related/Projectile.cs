using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] public float SpeedReduction = 1f;
    [SerializeField] public float destroyTimeInSeconds = 1f;

    Coroutine c;


    private void Start()
    {
        c = StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime() {
        yield return new WaitForSeconds(destroyTimeInSeconds);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ObjectObstacle obstacle)) {
            obstacle.ObstacleHP -= damage;
            StopCoroutine(c);
            Destroy(gameObject);
        }
    }
}
