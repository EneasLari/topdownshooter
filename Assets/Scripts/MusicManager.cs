using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneName;

    void Start()
    {
        //Gets replaced with onSceneManager.sceneloaded
        //OnLevelWasLoaded(0);
    }


    //Gets replaced with onSceneManager.sceneloaded
    //void OnLevelWasLoaded(int sceneIndex)
    //{
    //    string newSceneName = SceneManager.GetActiveScene().name;
    //    if (newSceneName != sceneName)
    //    {
    //        sceneName = newSceneName;
    //        Invoke("PlayMusic", .2f);
    //    }
    //}

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        //code of OnLevelWasLoaded
        string newSceneName = SceneManager.GetActiveScene().name;
        if (newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if (sceneName == "MainMenu")
        {
            clipToPlay = menuTheme;
        }
        else if (sceneName == "MainGame")
        {
            clipToPlay = mainTheme;
        }

        if (clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }

    }

}