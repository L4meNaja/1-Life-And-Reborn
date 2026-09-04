using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 3f;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroyTime);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
