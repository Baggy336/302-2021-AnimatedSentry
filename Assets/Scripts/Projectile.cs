using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform target;

    public Vector3 vToPlayer;

    float speed = 7;

    float destroyCountdown = 0;

    private void Start()
    {
        destroyCountdown = 4;
        vToPlayer = (target.position - transform.position).normalized;
    }

    private void Update()
    {
        transform.position += vToPlayer * speed * Time.deltaTime;
        DestroyAfterTime();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        TargetableThing target = other.GetComponent<TargetableThing>();

        if (player) // Overlapping a player object
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth)
            {
                playerHealth.TakeDamage(10); // Give damage to the player
            }
            Destroy(gameObject); // Remove projectile
        }
        else if (target) // Overlapping a targetablething
        {
            HealthSystem targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth)
            {
                targetHealth.TakeDamage(5);
            }
            Destroy(gameObject);
        }
    }

    void DestroyAfterTime()
    {
        destroyCountdown -= Time.deltaTime;
        if (destroyCountdown <= 0) Destroy(gameObject);
    }
}
