using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour,
    MMEventListener<LoadSceneEvent>
{

    void OnEnable()
    {
        this.MMEventStartListening<LoadSceneEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<LoadSceneEvent>();
    }

    public void OnMMEvent(LoadSceneEvent e)
    {
        SceneManager.LoadScene(e.SceneName);
    }
}
