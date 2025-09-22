using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SavePrompt : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private Button _confirmButton;
    [SerializeField, BoxGroup("References")] private Button _cancelButton;
    private Tween _alphaTween;

    private string _saveName;

    [HideInInspector] public UnityEvent OnPromptDisabled;

    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (_confirmButton == null) _confirmButton = GetComponentsInChildren<Button>()[0];
        if (_cancelButton == null) _cancelButton = GetComponentsInChildren<Button>()[1];
    }

    void OnEnable()
    {
        _confirmButton.onClick.AddListener(StartGame);
        _cancelButton.onClick.AddListener(DisablePrompt);
    }

    void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(StartGame);
        _cancelButton.onClick.RemoveListener(DisablePrompt);
        _alphaTween.Stop();
    }

    public void EnablePrompt(string saveName)
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        _saveName = saveName;
    }

    public void DisablePrompt()
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        OnPromptDisabled.Invoke();
    }

    private void StartGame()
    {
        StartGameManager.Instance.StartGame(_saveName);
    }
}
