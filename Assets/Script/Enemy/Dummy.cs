using UnityEngine;

public class Dummy : MonoBehaviour
{
   public float health;
   public GameObject FloatingTextPrefab;

void takeDamage(int damage)
    {
        health -= damage;
        ShowFloatingText();
    }

    void ShowFloatingText()
    {
        var go = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = health.ToString();
    }

}
