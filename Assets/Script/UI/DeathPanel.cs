using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour
{
    [Header("Canvas & Background")]
    public Canvas myCanvas;
    public Image panelBg;
    public float fadeDuration = 1.0f;

    [Header("Agent Data")]
    public int currentAgentNumber = 1;

    [Header("Text References")]
    public TMP_Text deathText;
    public TMP_Text welcomeText;
    public TMP_Text agentCodeText;

    [Header("TypeWriter Settings")]
    public float typeDelay = 0.05f;

    [Header("Positions & Offsets")]
    public Vector2 deathTextFloatOffset = new Vector2(0, 150f);
    public Vector2 welcomeMoveLeftOffset = new Vector2(-100f, 0);
    
    [Tooltip("ปรับตำแหน่ง Agent Text ในขั้นที่ 1 (อิงจากตำแหน่ง Welcome)")]
    public Vector2 agentCodeOffsetRight = new Vector2(250f, 0); 

    [Header("Agent Code Target Position")]
    public Vector2 agentCodeTargetPosition = new Vector2(710f, 428f);

    private int currentStep = 0;
    private bool isAnimating = false;
    private RectTransform deathTextRect;
    private RectTransform welcomeTextRect;
    private RectTransform agentCodeTextRect;

    private Vector2 deathTextOriginalPos;
    private Vector2 welcomeTextOriginalPos;

    private void Awake()
    {
        if (myCanvas == null) myCanvas = GetComponent<Canvas>();
        
        if (deathText != null) deathTextRect = deathText.GetComponent<RectTransform>();
        if (welcomeText != null) welcomeTextRect = welcomeText.GetComponent<RectTransform>();
        if (agentCodeText != null) agentCodeTextRect = agentCodeText.GetComponent<RectTransform>();

        if (deathTextRect != null) deathTextOriginalPos = deathTextRect.anchoredPosition;
        if (welcomeTextRect != null) welcomeTextOriginalPos = welcomeTextRect.anchoredPosition;

        ResetPanelUI();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAnimating && currentStep > 0)
        {
            AdvanceSequence();
        }
    }

    public void PlayDeathSequence()
    {
        Time.timeScale = 0f;

        // รีเซ็ต UI ทั้งหมดให้พร้อมสำหรับการตายรอบใหม่
        ResetForNextDeath();

        int oldAgent = currentAgentNumber;
        currentAgentNumber++;

        if (myCanvas != null) myCanvas.enabled = true;
        StartCoroutine(StartDeathSequenceRoutine(oldAgent));
    }

    private IEnumerator StartDeathSequenceRoutine(int oldAgentNumber)
    {
        isAnimating = true;

        if (agentCodeText != null)
        {
            StartCoroutine(FadeOutText(agentCodeText, fadeDuration));
        }

        yield return StartCoroutine(FadePanelBg(0f, 1f, fadeDuration));

        string deathMsg = $"You're dead Agent {oldAgentNumber.ToString("D3")}";
        yield return StartCoroutine(TypeTextRoutine(deathText, deathMsg));

        currentStep = 1;
        isAnimating = false;
    }

    private void AdvanceSequence()
    {
        if (currentStep == 1)
        {
            StartCoroutine(Step1_WelcomeSequence());
        }
        else if (currentStep == 2)
        {
            StartCoroutine(Step2_FinalStep());
        }
    }

    private IEnumerator Step1_WelcomeSequence()
    {
        isAnimating = true;

        Vector2 targetDeathPos = deathTextOriginalPos + deathTextFloatOffset;
        yield return StartCoroutine(MoveRectRoutine(deathTextRect, deathTextRect.anchoredPosition, targetDeathPos, 0.5f));

        yield return StartCoroutine(TypeTextRoutine(welcomeText, "Welcome"));

        Vector2 targetWelcomePos = welcomeTextOriginalPos + welcomeMoveLeftOffset;
        yield return StartCoroutine(MoveRectRoutine(welcomeTextRect, welcomeTextRect.anchoredPosition, targetWelcomePos, 0.4f));

        if (agentCodeTextRect != null)
        {
            Color c = agentCodeText.color;
            c.a = 1f;
            agentCodeText.color = c;
            
            agentCodeTextRect.anchoredPosition = targetWelcomePos + agentCodeOffsetRight;
        }

        string newAgentStr = currentAgentNumber.ToString("D3");
        string agentCodeMsg = $"Agent {newAgentStr}";
        yield return StartCoroutine(TypeTextRoutine(agentCodeText, agentCodeMsg));

        currentStep = 2;
        isAnimating = false;
    }

    private IEnumerator Step2_FinalStep()
    {
        isAnimating = true;

        Time.timeScale = 1f;

        if (PlayerStats.playerStats != null)
        {
            PlayerStats.playerStats.RandomStat();
        }

        StartCoroutine(MoveRectRoutine(agentCodeTextRect, agentCodeTextRect.anchoredPosition, agentCodeTargetPosition, fadeDuration));

        StartCoroutine(FadeOutText(deathText, fadeDuration));
        StartCoroutine(FadeOutText(welcomeText, fadeDuration));

        yield return StartCoroutine(FadePanelBg(1f, 0f, fadeDuration));

        if (deathText != null) deathText.gameObject.SetActive(false);
        if (welcomeText != null) welcomeText.gameObject.SetActive(false);

        currentStep = 3;
        isAnimating = false;
    }

    private IEnumerator TypeTextRoutine(TMP_Text textComponent, string message)
    {
        if (textComponent == null) yield break;

        textComponent.gameObject.SetActive(true);
        textComponent.text = "";

        foreach (char c in message)
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(typeDelay);
        }
    }

    private IEnumerator FadePanelBg(float startAlpha, float targetAlpha, float duration)
    {
        if (panelBg == null) yield break;

        float elapsed = 0f;
        Color c = panelBg.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            panelBg.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        panelBg.color = c;
    }

    private IEnumerator FadeOutText(TMP_Text textComponent, float duration)
    {
        if (textComponent == null) yield break;

        float elapsed = 0f;
        Color startColor = textComponent.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            textComponent.color = c;
            yield return null;
        }

        Color finalColor = startColor;
        finalColor.a = 0f;
        textComponent.color = finalColor;
    }

    private IEnumerator MoveRectRoutine(RectTransform rect, Vector2 start, Vector2 end, float duration)
    {
        if (rect == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            rect.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        rect.anchoredPosition = end;
    }

    private void ResetForNextDeath()
    {
        currentStep = 0;
        isAnimating = false;

        if (deathText != null)
        {
            deathText.text = "";
            deathText.gameObject.SetActive(true);
            Color c = deathText.color;
            c.a = 1f;
            deathText.color = c;
            deathTextRect.anchoredPosition = deathTextOriginalPos;
        }

        if (welcomeText != null)
        {
            welcomeText.text = "";
            welcomeText.gameObject.SetActive(false);
            Color c = welcomeText.color;
            c.a = 1f;
            welcomeText.color = c;
            welcomeTextRect.anchoredPosition = welcomeTextOriginalPos;
        }
    }

    private void ResetPanelUI()
    {
        ResetForNextDeath();

        if (panelBg != null)
        {
            Color c = panelBg.color;
            c.a = 0f;
            panelBg.color = c;
        }

        if (agentCodeText != null)
        {
            agentCodeText.gameObject.SetActive(true);
            agentCodeText.text = $"Agent {currentAgentNumber.ToString("D3")}";
            Color c = agentCodeText.color;
            c.a = 1f;
            agentCodeText.color = c;
            agentCodeTextRect.anchoredPosition = agentCodeTargetPosition;
        }
    }
}