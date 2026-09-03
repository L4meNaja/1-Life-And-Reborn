using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class EnemyBullet : MonoBehaviour
{
    public float life = 3;
 
    void Awake()
    {
        Destroy(gameObject, life);
    }
 
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStats>().TakeDamage(10);
        }
        Destroy(gameObject);
    }
}