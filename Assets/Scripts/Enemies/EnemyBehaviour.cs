using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMode { onPool, appearance, disappearance, onTrack, eliminated }

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
    float timeOnTrack;
    GameObject projectile;
    float cadence;
    float range;
    GameObject drop;
    float dropAmount;

    //INTERNAL
    [HideInInspector] public Vector3 targetPosition = Vector3.zero;
    Vector3 poolPosition;
    [HideInInspector] public float xOffset = 0;
    [HideInInspector] public float yOffset = 0;
    float distanceOnPath;
    float eliminatedTime = 4;

    void Start()
    {
        //REFERENCES
        rb = GetComponent<Rigidbody>();

        //INITIALIZE
        enemyMode = EnemyMode.onPool;
        health = enemyTemplate.health;
        maxSpeed = enemyTemplate.maxSpeed;
        distanceToPlayer = enemyTemplate.distanceToPlayer;
        timeOnTrack = enemyTemplate.timeOnTrack;
        projectile = enemyTemplate.projectile;
        cadence = enemyTemplate.cadence;
        range = enemyTemplate.range;
        drop = enemyTemplate.drop;
        dropAmount = enemyTemplate.dropAmount;

        //GET POOL POSITION
        poolPosition = TrackManager.Instance.objectPoolPosition;
    }

    void Update()
    {
        switch (enemyMode)
        {
            case EnemyMode.onPool:
                //OBJECT REMAINS ON POOL
                rb.useGravity = false;
                GetComponent<CapsuleCollider>().enabled = true;
                health = enemyTemplate.health;
                timeOnTrack = enemyTemplate.timeOnTrack;
                transform.position = TrackManager.Instance.objectPoolPosition;
                break;

            case EnemyMode.appearance:
                //RESET RB INERTIA
                rb.velocity = Vector3.zero;
                
                //ROTATE
                transform.rotation = GameManager.Instance.player.transform.rotation;

                //CALCULATE TARGET POSITION
                targetPosition = TrackManager.Instance.GetPositionAtDistance(GameManager.Instance.player.distanceTravelled + distanceToPlayer);
                targetPosition += transform.right * xOffset + transform.up * yOffset;

                //MOVE TO TARGET POSITION
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxSpeed * Time.deltaTime);

                Debug.DrawLine(transform.position, targetPosition);

                if((transform.position - targetPosition).magnitude < 1)
                {         
                    enemyMode = EnemyMode.onTrack;
                    distanceOnPath = GameManager.Instance.player.distanceTravelled + enemyTemplate.distanceToPlayer;
                }

                //CHECK HEALTH
                if (health <= 0)
                    Explode();
                break;

            case EnemyMode.onTrack:
                //SET SPEED
                speed = Mathf.Clamp(GameManager.Instance.player.currentSpeed, 0, maxSpeed);

                //GET FORWARD VECTOR
                transform.forward = TrackManager.Instance.GetDirectionAtDistance(distanceOnPath);

                //INCREASE DISTANCE ON PATH
                distanceOnPath += speed * Time.deltaTime;
                
                //MOVE ENEMY
                transform.position += transform.forward * speed * Time.deltaTime;

                //SHOOT   
                cadence -= Time.deltaTime;
                if (cadence < 0)
                {
                    if (Mathf.Abs(distanceOnPath - GameManager.Instance.player.distanceTravelled) < range)
                    {
                        Shoot();
                    }
                }

                //CHECK HEALTH
                if (health <= 0)
                    Explode();

                //DISAPPEAR OVER TIME
                timeOnTrack -= Time.deltaTime;
                if (timeOnTrack < 0)
                    enemyMode = EnemyMode.disappearance;
                break;

            case EnemyMode.disappearance:
                //MOVE TO TARGET POSITION
                transform.position = Vector3.MoveTowards(transform.position, poolPosition, maxSpeed * Time.deltaTime);
                if ((transform.position == poolPosition))
                    enemyMode = EnemyMode.onPool;               
                break;

            case EnemyMode.eliminated:
                eliminatedTime -= Time.deltaTime;
                if (eliminatedTime < 0)
                    enemyMode = EnemyMode.onPool;
                break;
        }
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
        rb.AddExplosionForce(50, transform.position, 5);
        rb.useGravity = true;
        GetComponent<CapsuleCollider>().enabled = false;
        enemyMode = EnemyMode.eliminated;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }
}
