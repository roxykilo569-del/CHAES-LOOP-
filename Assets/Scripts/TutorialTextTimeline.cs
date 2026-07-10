using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class TutorialTextItem
{
    [Header("Time")]
    public float startTime = 0f;
    public float duration = 3f;

    [Header("Text")]
    [TextArea(2, 6)]
    public string text;

    [Header("Position")]
    public bool overridePosition = true;
    public Vector2 anchoredPosition = new Vector2(0f, -250f);

    public bool overrideSize = false;
    public Vector2 sizeDelta = new Vector2(900f, 160f);

    [Header("Style")]
    public bool overrideStyle = true;
    public float fontSize = 36f;
    public Color textColor = Color.white;
    public TextAlignmentOptions alignment = TextAlignmentOptions.Center;
    public FontStyles fontStyle = FontStyles.Normal;
    public TMP_FontAsset fontAsset;
}

public class TutorialTextTimeline : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup textGroup;
    public TextMeshProUGUI textUI;
    public RectTransform textRect;

    [Header("Tutorial Texts")]
    public List<TutorialTextItem> tutorialTexts = new List<TutorialTextItem>();

    [Header("Fade")]
    public float fadeInTime = 0.15f;
    public float fadeOutTime = 0.15f;

    [Header("Typewriter")]
    public bool useTypewriter = true;
    public float typeSpeed = 0.025f;

    [Header("Actual Gameplay Time")]
    public float gameTimer = 0f;
    public bool onlyCountWhenPlaying = true;

    [Header("Dialogue")]
    public bool pauseWhenDialogueActive = true;
    public GameObject dialoguePanel;
    public CanvasGroup dialogueCanvasGroup;

    private int currentIndex = -999;
    private Coroutine textRoutine;
    private bool wasPausedByDialogue = false;

    private Vector2 defaultAnchoredPosition;
    private Vector2 defaultSizeDelta;
    private float defaultFontSize;
    private Color defaultColor;
    private TextAlignmentOptions defaultAlignment;
    private FontStyles defaultFontStyle;
    private TMP_FontAsset defaultFontAsset;

    private void Awake()
    {
        if (textUI != null)
        {
            textUI.richText = true;

            if (textRect == null)
            {
                textRect = textUI.GetComponent<RectTransform>();
            }

            defaultFontSize = textUI.fontSize;
            defaultColor = textUI.color;
            defaultAlignment = textUI.alignment;
            defaultFontStyle = textUI.fontStyle;
            defaultFontAsset = textUI.font;
        }

        if (textRect != null)
        {
            defaultAnchoredPosition = textRect.anchoredPosition;
            defaultSizeDelta = textRect.sizeDelta;
        }

        HideInstant();
    }

    private void Update()
    {
        if (!CanCountGameplayTime())
        {
            return;
        }

        if (IsDialogueOpen())
        {
            if (textRoutine != null)
            {
                StopCoroutine(textRoutine);
                textRoutine = null;
            }

            HideInstant();
            wasPausedByDialogue = true;
            return;
        }

        if (wasPausedByDialogue)
        {
            currentIndex = -999;
            wasPausedByDialogue = false;
        }

        gameTimer += Time.deltaTime;

        int nextIndex = GetCurrentTextIndex();

        if (nextIndex != currentIndex)
        {
            currentIndex = nextIndex;

            if (textRoutine != null)
            {
                StopCoroutine(textRoutine);
            }

            if (currentIndex >= 0)
            {
                textRoutine = StartCoroutine(
                    ShowTextRoutine(tutorialTexts[currentIndex])
                );
            }
            else
            {
                textRoutine = StartCoroutine(HideRoutine());
            }
        }
    }

    private bool CanCountGameplayTime()
    {
        if (!onlyCountWhenPlaying)
        {
            return true;
        }

        if (GameManager.Instance == null)
        {
            return false;
        }

        return GameManager.Instance.Phase == GamePhase.Playing;
    }

    private bool IsDialogueOpen()
    {
        if (!pauseWhenDialogueActive)
        {
            return false;
        }

        if (dialogueCanvasGroup != null)
        {
            return dialogueCanvasGroup.gameObject.activeInHierarchy &&
                   dialogueCanvasGroup.alpha > 0.01f;
        }

        if (dialoguePanel != null)
        {
            return dialoguePanel.activeInHierarchy;
        }

        return false;
    }

    private int GetCurrentTextIndex()
    {
        for (int i = 0; i < tutorialTexts.Count; i++)
        {
            TutorialTextItem item = tutorialTexts[i];

            if (gameTimer >= item.startTime &&
                gameTimer <= item.startTime + item.duration)
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator ShowTextRoutine(TutorialTextItem item)
    {
        ApplyItemVisual(item);

        if (textUI != null)
        {
            textUI.text = item.text;
            textUI.maxVisibleCharacters = 0;
        }

        yield return FadeTo(1f, fadeInTime);

        if (textUI == null) yield break;

        if (useTypewriter)
        {
            textUI.ForceMeshUpdate();

            int totalVisibleCharacters = textUI.textInfo.characterCount;
            int visibleCount = 0;

            while (visibleCount <= totalVisibleCharacters)
            {
                textUI.maxVisibleCharacters = visibleCount;
                visibleCount++;

                yield return new WaitForSeconds(typeSpeed);
            }
        }
        else
        {
            textUI.maxVisibleCharacters = int.MaxValue;
        }
    }

    private void ApplyItemVisual(TutorialTextItem item)
    {
        if (textRect != null)
        {
            if (item.overridePosition)
            {
                textRect.anchoredPosition = item.anchoredPosition;
            }
            else
            {
                textRect.anchoredPosition = defaultAnchoredPosition;
            }

            if (item.overrideSize)
            {
                textRect.sizeDelta = item.sizeDelta;
            }
            else
            {
                textRect.sizeDelta = defaultSizeDelta;
            }
        }

        if (textUI != null)
        {
            if (item.overrideStyle)
            {
                textUI.fontSize = item.fontSize;
                textUI.color = item.textColor;
                textUI.alignment = item.alignment;
                textUI.fontStyle = item.fontStyle;

                if (item.fontAsset != null)
                {
                    textUI.font = item.fontAsset;
                }
                else
                {
                    textUI.font = defaultFontAsset;
                }
            }
            else
            {
                textUI.fontSize = defaultFontSize;
                textUI.color = defaultColor;
                textUI.alignment = defaultAlignment;
                textUI.fontStyle = defaultFontStyle;
                textUI.font = defaultFontAsset;
            }
        }
    }

    private IEnumerator HideRoutine()
    {
        yield return FadeTo(0f, fadeOutTime);

        if (textUI != null)
        {
            textUI.text = "";
            textUI.maxVisibleCharacters = int.MaxValue;
        }
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (textGroup == null) yield break;

        float startAlpha = textGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            textGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        textGroup.alpha = targetAlpha;
    }

    private void HideInstant()
    {
        if (textGroup != null)
        {
            textGroup.alpha = 0f;
        }

        if (textUI != null)
        {
            textUI.text = "";
            textUI.maxVisibleCharacters = int.MaxValue;
        }
    }

    public void ResetTimeline()
    {
        gameTimer = 0f;
        currentIndex = -999;
        wasPausedByDialogue = false;

        if (textRoutine != null)
        {
            StopCoroutine(textRoutine);
        }

        HideInstant();
    }
}