using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    //ENEMY TEMPLATE
    public Enemy enemyTemplate;

    //REFERENCES
    public Transform shotSpawn;

    //VARIABLES
    float health;
    GameObject projectile;
    float cadence;
    float range;
    GameObject drop;
    float dropAmount;
    float distanceOnTrack;

    void Start()
    {
        //INITIALIZE
        health = enemyTemplate.health;
        projectile = enemyTemplate.projectile;
        cadence = enemyTemplate.cadence;
        range = enemyTemplate.range;
        drop = enemyTemplate.drop;
        dropAmount = enemyTemplate.dropAmount;
    }

    void Update()
    {
        //SET DISTANCE ON PATH
        distanceOnTrack = TrackManager.Instance.GetClosestDistanceOnPath(transform.position);

        //DETECT PLAYER
        if (Mathf.Abs(distanceOnTrack - GameManager.Instance.player.distanceTravelled) < range)
        {
            cadence -= Time.deltaTime;
            if (cadence < 0)
                Shoot();          
        }

        //CHECK HEALTH
        if (health <= 0)
            Explode();
    }

    void Shoot()
    {
        GameObject s = Instantiate(enemyTemplate.projectile, shotSpawn.position, shotSpawn.rotation);
        s.tag = "Enemy";
        cadence = enemyTemplate.cadence;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public void Explode()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().AddExplosionForce(50, transform.position, 5);
        this.enabled = false;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }
}
