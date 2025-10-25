using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFocusOnAwake : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private bool _isPersistent;

    void OnEnable()
    {
        if (_isPersistent)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void OnDisable()
    {
        if (_isPersistent)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnDestroy()
    {
        if (_isPersistent)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Awake()
    {
        if (!_isPersistent)
            AssignCamTargetEvent.Trigger(transform);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        AssignCamTargetEvent.Trigger(transform);
    }
}
