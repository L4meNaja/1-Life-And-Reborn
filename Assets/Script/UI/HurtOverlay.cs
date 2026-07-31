using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HurtOverlay : MonoBehaviour
{
    public RawImage hurtOverlay;
    public Texture2D healthHurt;
    public Texture2D shieldHurt; 

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    void Start()
    {
        if (hurtOverlay != null)
        {
            Color c = hurtOverlay.color;
            c.a = 0;
            hurtOverlay.color = c;
        }
    }

    public void ShowHurtEffect(bool isShield)
    {
        if (hurtOverlay == null) return;

        hurtOverlay.texture = isShield ? shieldHurt : healthHurt;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        Color color = hurtOverlay.color;
        color.a = 1f;
        hurtOverlay.color = color;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            hurtOverlay.color = color;
            yield return null;
        }

        color.a = 0f;
        hurtOverlay.color = color;
    }
}