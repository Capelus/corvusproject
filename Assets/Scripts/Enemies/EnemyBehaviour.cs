using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMode { appearance, onTrack }

public class EnemyBehaviour : MonoBehaviour
{
    //ENEMY TEMPLATE
    public Enemy enemyTemplate;

    //MODE
    public EnemyMode enemyMode;

    //REFERENCES
    public Transform shotSpawn;
    Rigidbody rb;

    //VARIABLES
    float health;
    float speed;
    float maxSpeed;
    float distanceToPlayer;
    GameObject projectile;
    float cadence;
    float range;
    GameObject drop;
    float dropAmount;

    //INTERNAL
    Vector3 targetPosition;
    float distanceOnPath;

    void Start()
    {
        //REFERENCES
        rb = GetComponent<Rigidbody>();

        //INITIALIZE
        enemyMode = EnemyMode.appearance;
        health = enemyTemplate.health;
        maxSpeed = enemyTemplate.maxSpeed;
        distanceToPlayer = enemyTemplate.distanceToPlayer;
        projectile = enemyTemplate.projectile;
        cadence = enemyTemplate.cadence;
        range = enemyTemplate.range;
        drop = enemyTemplate.drop;
        dropAmount = enemyTemplate.dropAmount;

        //GET TARGET POSITION
        targetPosition = TrackManager.Instance.GetPositionAtDistance(GameManager.Instance.player.distanceTravelled + distanceToPlayer);

    }

    void Update()
    {
        switch (enemyMode)
        {
            case EnemyMode.appearance:
                //MOVE TO TARGET POSITION ROTATING WITH THE PATH
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxSpeed * Time.deltaTime);

                if((transform.position - targetPosition).magnitude < 1)
                {
                    distanceOnPath = TrackManager.Instance.GetClosestDistanceOnPath(transform.position);
                    enemyMode = EnemyMode.onTrack;
                }
                break;

            case EnemyMode.onTrack:
                //SET SPEED
                speed = Mathf.Clamp(GameManager.Instance.player.currentSpeed, 0, maxSpeed);

                //INCREASE DISTANCE ON PATH
                distanceOnPath += speed * Time.deltaTime;

                //GET FORWARD VECTOR
                Vector3 direction = TrackManager.Instance.GetDirectionAtDistance(distanceOnPath);
                transform.forward = direction.normalized;

                //MOVE ENEMY
                transform.position += direction * speed * Time.deltaTime;
                break;
        }

        //DETECT PLAYER
        if (Mathf.Abs(distanceOnPath - GameManager.Instance.player.distanceTravelled) < range)
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
        rb.constraints = RigidbodyConstraints.None;
        rb.AddExplosionForce(50, transform.position, 5);
        rb.useGravity = true;
        GetComponent<CapsuleCollider>().enabled = false;
        this.enabled = false;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }
}
