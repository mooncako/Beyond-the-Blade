using System.Collections;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIIcon : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Image _icon;
    [SerializeField, BoxGroup("References")] private Image _background;
    [SerializeField, BoxGroup("References")] private Image _selection;
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;

    private Tween _alphaTween;


    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        _canvasGroup.alpha = 0;
    }


    void OnDisable()
    {
        _alphaTween.Stop();
    }

    public void OnAbilityCooldownStarted(float cooldownTime)
    {
        StartCoroutine(StartIconCooldownEffect(cooldownTime));
    }

    private IEnumerator StartIconCooldownEffect(float cooldownTime)
    {

        float elapsedTime = 0f;

        _icon.fillAmount = 0f;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            _icon.fillAmount = elapsedTime / cooldownTime;
            yield return null;
        }

        _icon.fillAmount = 1f;
    }

    public void Select()
    {
        _selection.gameObject.SetActive(true);
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
    }

    public void Deselect()
    {
        _selection.gameObject.SetActive(false);
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, .5f, .5f);
    }

    public void AssignIcon(Sprite icon)
    {
        _icon.sprite = icon;
        _background.sprite = icon;
        if (_selection.gameObject.activeSelf)
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
        }
        else
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, .5f, .5f);
        }
        
    }
}
