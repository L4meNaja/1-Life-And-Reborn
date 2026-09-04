using TMPro;
using UnityEngine;
using System.Collections;

public class TypeWriter : MonoBehaviour
{
    public TMP_Text textUI;

    [TextArea]
    public string message;

    public float delay = 0.05f;

    void OnEnable()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textUI.text = "";

        foreach (char c in message)
        {
            textUI.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}