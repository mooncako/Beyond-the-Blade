using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    [HideInInspector]
    public string ObjectID;

    [SerializeField, BoxGroup("Settings")] private List<string> _destroySceneNames = new List<string>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckDestroy;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckDestroy;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= CheckDestroy;
    }

    private void Awake()
    {
        ObjectID = name + transform.position.ToString() + transform.eulerAngles.ToString();
        DontDestroy[] objects = FindObjectsByType<DontDestroy>(FindObjectsSortMode.None);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != this)
            {
                if (objects[i].ObjectID == ObjectID)
                {
                    Destroy(gameObject);
                }
            }
        }



        DontDestroyOnLoad(gameObject);
    }

    private void CheckDestroy(Scene scene, LoadSceneMode mode)
    {
        if (_destroySceneNames.Count > 0)
        {
            for (int i = 0; i < _destroySceneNames.Count; i++)
            {
                if (SceneManager.GetActiveScene().name == _destroySceneNames[i])
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

