using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public Enemy enemyToSpawn;
    public int numberOfEnemiesToSpawn = 1;

    Vector2 [] targetOffsets;

    void Start()
    {
        targetOffsets = new Vector2[numberOfEnemiesToSpawn];

        for (int i=0; i < targetOffsets.Length; i++)
        {
            targetOffsets[i].x = Random.Range(-4, 4);
            targetOffsets[i].y = Random.Range(-4, 4);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject[] enemies = TrackManager.Instance.enemies;

            foreach (GameObject e in enemies)
            {
                if(numberOfEnemiesToSpawn > 0 && e.GetComponent<EnemyBehaviour>().enemyMode == EnemyMode.onPool)
                {
                    //SET INITIAL POSITION
                    int side = (side = Random.Range(-1, 1) == 0 ? side = 1 : -1);
                    e.transform.position = transform.position + Vector3.left * 40 * side + Vector3.up * 20;

                    //SET OFFSETS
                    e.GetComponent<EnemyBehaviour>().xOffset = targetOffsets[numberOfEnemiesToSpawn-1].x;
                    e.GetComponent<EnemyBehaviour>().yOffset = targetOffsets[numberOfEnemiesToSpawn-1].y;

                    //CHANGE ENEMY MOIDE
                    e.GetComponent<EnemyBehaviour>().enemyMode = EnemyMode.appearance;

                    numberOfEnemiesToSpawn--;
                }
            }

            this.enabled = false;
        }
    }
}
