using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Button _startGame;
    [SerializeField, BoxGroup("References")] private Button _loadGame;
    [SerializeField, BoxGroup("References")] private Button _exitGame;

    void OnValidate()
    {
        if (_startGame == null) _startGame = GetComponentsInChildren<Button>()[0];
        if (_loadGame == null) _loadGame = GetComponentsInChildren<Button>()[1];
        if (_exitGame == null) _exitGame = GetComponentsInChildren<Button>()[2];
    }

    void OnEnable()
    {
        _startGame.onClick.AddListener(() =>
        {
            MainMenuSaveLoadTriggerEvent.Trigger(SaveLoadEventType.Save);
        });
        
        _loadGame.onClick.AddListener(() =>
        {
            MainMenuSaveLoadTriggerEvent.Trigger(SaveLoadEventType.Load);
        });

        _exitGame.onClick.AddListener(() =>
        {
            Application.Quit(0);
        });
    }

    void OnDisable()
    {
        _startGame.onClick.RemoveListener(() =>
        {
            MainMenuSaveLoadTriggerEvent.Trigger(SaveLoadEventType.Save);
        });

        _loadGame.onClick.RemoveListener(() =>
        {
            MainMenuSaveLoadTriggerEvent.Trigger(SaveLoadEventType.Load);
        });

        _exitGame.onClick.RemoveListener(() =>
        {
            Application.Quit(0);
        });
    }
}
