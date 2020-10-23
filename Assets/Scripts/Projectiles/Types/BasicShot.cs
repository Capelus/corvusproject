using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileParameters))]
public class BasicShot : MonoBehaviour
{
    //BASE PROJECTILE PARAMETERS
    ProjectileParameters projectileParameters;

    //SHOT PARAMETERS
    float distanceOnPath;
    public float speed = 80;
    Vector3 direction;
    public float damage = 20;
    public float lifeTime = 2;

    private void Start()
    {
        projectileParameters = GetComponent<ProjectileParameters>();
        distanceOnPath = projectileParameters.initialDistanceOnPath;
        speed += projectileParameters.initialSpeed;
        if (projectileParameters.direction == -1)
            speed = -speed;
    }

    void Update()
    {
        //INCREASE DISTANCE ON PATH
        distanceOnPath += speed * Time.deltaTime;
        
        //GET FORWARD VECTOR
        direction = TrackManager.Instance.GetDirectionAtDistance(distanceOnPath);
        transform.forward = direction.normalized;

        //MOVE SHOT
        transform.position += direction * speed * Time.deltaTime;

        //LIFETIME COUNTDOWN
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            DestroyShot(false);
    }

    //EFFECT
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                if (!transform.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyBehaviour>().TakeDamage(damage);
                    DestroyShot(true);
                }
                break;

            case "Player":
                if (!transform.CompareTag("Player"))
                {
                    other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
                    DestroyShot(true);
                }
                break;

            case "Obstacle":
                if (other.transform.parent.GetComponent<BreakableBehaviour>())
                    other.transform.parent.GetComponent<BreakableBehaviour>().Destroy();
                break;

            default: 
                DestroyShot(true);
                break;
        }
    }

    public void DestroyShot(bool explode)
    {
        if (explode)
            EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);

        Destroy(this.gameObject);
    }
}
