using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFocusOnAwake : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private bool _isPersistant;

    void OnEnable()
    {
        if (_isPersistant)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void OnDisable()
    {
        if (_isPersistant)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnDestroy()
    {
        if (_isPersistant)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Awake()
    {
        if (!_isPersistant)
            AssignCamTargetEvent.Trigger(transform);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        AssignCamTargetEvent.Trigger(transform);
    }
}
