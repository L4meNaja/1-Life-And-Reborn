using UnityEngine;

public class UiFloating : MonoBehaviour
{
    public float amplitude = 5f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPos +
            new Vector3(
                0,
                Mathf.Sin(Time.time * speed) * amplitude,
                0
            );
    }
}