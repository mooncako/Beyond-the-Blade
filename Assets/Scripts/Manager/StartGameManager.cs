using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameManager : MMSingleton<StartGameManager>
{
    public void StartGame(string saveName)
    {
        SaveEvent.Trigger(saveName);
        SceneManager.LoadScene("TestHub");
    }
}
