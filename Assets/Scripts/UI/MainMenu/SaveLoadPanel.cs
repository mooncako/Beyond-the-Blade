using System.Linq;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SaveLoadPanel : MonoBehaviour,
    MMEventListener<MainMenuSaveLoadTriggerEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private SaveSlot[] _saveSlots;
    [SerializeField, BoxGroup("Debug")] private SaveSlot _currentSelectedSave;

    private Tween _alphaTween;

    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (_saveSlots.Length == 0) _saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<MainMenuSaveLoadTriggerEvent>();
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].OnSaveSlotSelected.AddListener(OnSaveSlotSelected);
        }
    }

    void OnDisable()
    {
        this.MMEventStopListening<MainMenuSaveLoadTriggerEvent>();
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].OnSaveSlotSelected.RemoveListener(OnSaveSlotSelected);
        }
        _alphaTween.Stop();
    }

    public void OnMMEvent(MainMenuSaveLoadTriggerEvent e)
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

    }

    private void OnSaveSlotSelected(SaveSlot slot)
    {
        _currentSelectedSave = slot;
    }
}
