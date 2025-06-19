using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class Meun : MonoBehaviour
{

    public void StartGameButtonEvent()
    {
        StartCoroutine(LoadGameMainScene());
    }
    public void ExitGameButtonEvent()
    {
        Application.Quit();
    }
    public void DevelopButtonEvent()
    {
        Debug.Log("Develop");
    }
    public void SettingsButtonEvent()
    {
        Debug.Log("Settings");
    }

    IEnumerator LoadGameMainScene()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("GameMainScene");
        while(ao.progress < 0.9f)
        {
            yield return null;
        }
    }
}