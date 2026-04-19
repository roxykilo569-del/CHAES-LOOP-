using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 双角色对话 UI：头像、名字、正文（打字机 + 音效）；点击跳过当前句或进入下一句。
/// 需在场景中有 EventSystem，且本对象带 Graphic（如 Image）以接收射线点击。
/// </summary>
[RequireComponent(typeof(Image))]
public class DialogueUIController : MonoBehaviour, IPointerClickHandler
{
    public static DialogueUIController Instance { get; private set; }

    [Header("UI 引用")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private Image portraitLeft;
    [SerializeField] private Image portraitRight;
    [SerializeField] private TMP_Text nameLeft;
    [SerializeField] private TMP_Text nameRight;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text hintText;

    [Header("说话者高亮（可选）")]
    [SerializeField] private float inactivePortraitAlpha = 0.45f;

    [Header("打字机")]
    [SerializeField] private float secondsPerChar = 0.04f;
    [Tooltip("空格、换行是否也发声（一般关）")]
    [SerializeField] private bool playSoundOnWhitespace;
    [Header("打字音效")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip defaultTypeSound;
    [Range(0f, 1f)]
    [SerializeField] private float typeSoundVolume = 0.7f;

    [Header("事件")]
    public UnityEvent onDialogueEnded;

    private DialogueData _data;
    private int _index;
    private bool _playing;
    private bool _lineComplete;
    private Coroutine _typewriterRoutine;
    /// <summary>上一段打字音效结束时间（unscaled），播完才允许下一段。</summary>
    private float _typeSoundEndTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnDisable()
    {
        StopTypewriter();
    }

    /// <summary>开始一段对话（可从其他脚本调用）</summary>
    public void StartDialogue(DialogueData data)
    {
        if (rootPanel == null || data == null || data.lines == null || data.lines.Length == 0)
        {
            Debug.LogWarning("DialogueUIController: 对话数据为空。");
            return;
        }

        StopTypewriter();

        _data = data;
        _index = 0;
        _playing = true;

        if (portraitLeft != null)
        {
            portraitLeft.sprite = data.portraitLeft;
            portraitLeft.enabled = data.portraitLeft != null;
        }
        if (portraitRight != null)
        {
            portraitRight.sprite = data.portraitRight;
            portraitRight.enabled = data.portraitRight != null;
        }
        if (nameLeft != null) nameLeft.text = data.nameLeft;
        if (nameRight != null) nameRight.text = data.nameRight;

        if (rootPanel != null)
            rootPanel.SetActive(true);

        DialogueRuntimeEvents.Raise(data.dialogueStartEventId, data, -1);

        ApplyLine();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ProcessDialogueLine();
    }

    private void ProcessDialogueLine()
    {
        if (!_playing || _data == null)
            return;

        if (!_lineComplete)
        {
            FinishCurrentLineInstant();
            return;
        }

        _index++;
        if (_index >= _data.lines.Length)
            EndDialogue();
        else
            ApplyLine();
    }

    private void ApplyLine()
    {
        DialogueLine line = _data.lines[_index];
        DialogueRuntimeEvents.Raise(line.lineBeginEventId, _data, _index);

        _lineComplete = false;
        _typeSoundEndTime = -999f;

        SetPortraitHighlight(line.speaker);

        UpdateHintForTyping();

        if (bodyText != null && !string.IsNullOrEmpty(line.text))
        {
            bodyText.text = string.Empty;
            StopTypewriter();
            _typewriterRoutine = StartCoroutine(TypewriterRoutine(line.text));
        }
        else
        {
            if (bodyText != null)
                bodyText.text = line.text ?? string.Empty;
            _lineComplete = true;
            UpdateHintForComplete();
            DialogueRuntimeEvents.Raise(line.lineCompleteEventId, _data, _index);
        }
    }

    private IEnumerator TypewriterRoutine(string full)
    {
        var wait = secondsPerChar > 0f
            ? new WaitForSeconds(secondsPerChar)
            : null;

        for (int i = 1; i <= full.Length; i++)
        {
            if (bodyText != null)
                bodyText.text = full.Substring(0, i);

            char c = full[i - 1];
            if (ShouldPlaySoundForChar(c))
                TryPlayTypeSound();

            if (wait != null)
                yield return wait;
            else
                yield return null;
        }

        _lineComplete = true;
        _typewriterRoutine = null;
        UpdateHintForComplete();
        if (_data != null && _index >= 0 && _index < _data.lines.Length)
            DialogueRuntimeEvents.Raise(_data.lines[_index].lineCompleteEventId, _data, _index);
    }

    private bool ShouldPlaySoundForChar(char c)
    {
        if (char.IsWhiteSpace(c) && !playSoundOnWhitespace)
            return false;
        return true;
    }

    private void TryPlayTypeSound()
    {
        if (audioSource == null)
            return;

        AudioClip clip = GetActiveTypeClip();
        if (clip == null)
            return;

        float t = Time.unscaledTime;
        if (t < _typeSoundEndTime)
            return;

        audioSource.PlayOneShot(clip, typeSoundVolume);

        float pitch = Mathf.Abs(audioSource.pitch);
        if (pitch < 0.0001f)
            pitch = 1f;
        _typeSoundEndTime = t + clip.length / pitch;
    }

    private AudioClip GetActiveTypeClip()
    {
        if (_data != null && _data.typingSoundOverride != null)
            return _data.typingSoundOverride;
        return defaultTypeSound;
    }

    private void FinishCurrentLineInstant()
    {
        if (_data == null || _index < 0 || _index >= _data.lines.Length)
            return;

        StopTypewriter();

        string full = _data.lines[_index].text ?? string.Empty;
        if (bodyText != null)
            bodyText.text = full;

        _lineComplete = true;
        UpdateHintForComplete();
        if (_data != null && _index >= 0 && _index < _data.lines.Length)
            DialogueRuntimeEvents.Raise(_data.lines[_index].lineCompleteEventId, _data, _index);
    }

    private void StopTypewriter()
    {
        if (_typewriterRoutine != null)
        {
            StopCoroutine(_typewriterRoutine);
            _typewriterRoutine = null;
        }
    }

    private void UpdateHintForTyping()
    {
        if (hintText == null)
            return;
        hintText.text = "点击跳过";
    }

    private void UpdateHintForComplete()
    {
        if (hintText == null)
            return;
        hintText.text = _index < _data.lines.Length - 1 ? "点击继续" : "点击结束";
    }

    private void SetPortraitHighlight(DialogueSpeakerSide side)
    {
        SetImageAlpha(portraitLeft, side == DialogueSpeakerSide.Left ? 1f : inactivePortraitAlpha);
        SetImageAlpha(portraitRight, side == DialogueSpeakerSide.Right ? 1f : inactivePortraitAlpha);
    }

    private static void SetImageAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private void EndDialogue()
    {
        StopTypewriter();
        _playing = false;
        DialogueData ended = _data;
        _data = null;
        if (rootPanel != null)
            rootPanel.SetActive(false);
        if (ended != null)
            DialogueRuntimeEvents.Raise(ended.dialogueEndEventId, ended, -1);
        onDialogueEnded?.Invoke();
    }

    /// <summary>外部可查询是否正在对话</summary>
    public bool IsPlaying => _playing;
}
