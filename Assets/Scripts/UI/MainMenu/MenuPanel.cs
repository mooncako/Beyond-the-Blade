using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Button _startGame;
    [SerializeField, BoxGroup("References")] private Button _loadGame;

    void OnValidate()
    {
        if (_startGame == null) _startGame = GetComponentsInChildren<Button>()[0];
        if (_loadGame == null) _loadGame = GetComponentsInChildren<Button>()[1];
    }

    void OnEnable()
    {
        _startGame.onClick.AddListener(() =>
        {
            MainMenuSaveLoadTriggerEvent.Trigger(SaveLoadEventType.Save);
        });
    }

    void OnDisable()
    {
        _startGame.onClick.RemoveListener(() =>
        {
            MainMenuSaveLoadTriggerEvent.Trigger(SaveLoadEventType.Save);
        });
    }
}
