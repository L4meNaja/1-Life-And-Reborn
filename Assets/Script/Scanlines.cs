using UnityEngine;
using UnityEngine.UI;

public class ScanlineEffect : MonoBehaviour
{
    public float speed = 50f;

    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        Rect uv = rawImage.uvRect;

        uv.y -= speed * Time.deltaTime / 1000f;

        rawImage.uvRect = uv;
        Color c = rawImage.color;


    }
}