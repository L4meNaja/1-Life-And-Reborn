using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class DamageArea : MonoBehaviour
{
    public float damageAmount = 10f;
    private bool isPlayerInside = false;

    void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerStats>() != null)
        {
            if (!isPlayerInside)
            {
                isPlayerInside = true;
                StartCoroutine(DamageRoutine(other.GetComponent<PlayerStats>()));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerStats>() != null)
        {
            isPlayerInside = false;
            StopAllCoroutines();
        }
    }

    IEnumerator DamageRoutine(PlayerStats player)
    {
        while (isPlayerInside && player != null)
        {
            player.TakeDamage(damageAmount);
            Debug.Log($"โดนดาเมจจากพื้นที่: {damageAmount}");

            yield return new WaitForSeconds(1f);
        }
    }
}