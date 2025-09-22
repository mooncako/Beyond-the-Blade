using System.IO;
using System.Linq;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveLoadPanel : MonoBehaviour,
    MMEventListener<MainMenuSaveLoadTriggerEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private SaveSlot[] _saveSlots;
    [SerializeField, BoxGroup("References")] private SavePrompt _savePrompt;
    [SerializeField, BoxGroup("References")] private Button _cancelButton;
    [SerializeField, BoxGroup("Debug")] private SaveSlot _currentSelectedSave;

    private Tween _alphaTween;
    private int _saveCount = 0;

    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (_savePrompt == null) _savePrompt = GetComponentInChildren<SavePrompt>();
        if (_cancelButton == null) _cancelButton = GetComponentsInChildren<Button>()[2];
        if (_saveSlots.Length == 0) _saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<MainMenuSaveLoadTriggerEvent>();
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].OnSaveSelected.AddListener(OnSaveSelected);
            _saveSlots[i].OnLoadSelected.AddListener(OnLoadSelected);
        }

        _cancelButton.onClick.AddListener(ClosePanel);
    }

    void OnDisable()
    {
        this.MMEventStopListening<MainMenuSaveLoadTriggerEvent>();
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].OnSaveSelected.RemoveListener(OnSaveSelected);
            _saveSlots[i].OnLoadSelected.RemoveListener(OnLoadSelected);
        }

        _cancelButton.onClick.RemoveListener(ClosePanel);

        _alphaTween.Stop();
    }

    public void OnMMEvent(MainMenuSaveLoadTriggerEvent e)
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;


        if (!Directory.Exists(DIRECTORY.SavePath))
        {
            Directory.CreateDirectory(DIRECTORY.SavePath);
        }

        string[] saveFiles = Directory.GetFiles(DIRECTORY.SavePath, "*.save");
        _saveCount = saveFiles.Length;
        for (int i = 0; i < saveFiles.Length; i++)
        {
            saveFiles[i] = saveFiles[i].Split(DIRECTORY.SavePath)[1];
            saveFiles[i] = saveFiles[i].Split('.')[0];
        }

        for (int i = 0; i < 3; i++)
        {
            if (i < saveFiles.Length)
            {
                _saveSlots[i].ToggleSave(true, saveFiles[i]);
            }
            else
            {
                _saveSlots[i].ToggleSave(false, "");
            }
        }

        if (e.EventType == SaveLoadEventType.Save)
        {
            for (int i = 0; i < _saveSlots.Length; i++)
            {
                _saveSlots[i].CurrentType = SaveLoadEventType.Save;
            }
        }
        else
        {
            for (int i = 0; i < _saveSlots.Length; i++)
            {
                _saveSlots[i].CurrentType = SaveLoadEventType.Load;
            }
        }
    }

    private void OnSaveSelected(SaveSlot slot)
    {
        _currentSelectedSave = slot;
        if (!_currentSelectedSave.IsSaved)
        {
            _saveCount++;
            _currentSelectedSave.ToggleSave(true, $"Save0{_saveCount}");
            StartGameManager.Instance.StartGame($"Save0{_saveCount}");
        }
        else
        {
            _savePrompt.EnablePrompt(_currentSelectedSave.GetSaveName());
        }
    }

    private void OnLoadSelected(SaveSlot slot)
    {
        _currentSelectedSave = slot;
        StartGameManager.Instance.StartGame(_currentSelectedSave.GetSaveName());
    }

    private void ClosePanel()
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

}
