using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{



    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("La scene d'index " + levelIndex + " n'existe pas !", this.gameObject);
            return;
        }

        SceneManager.LoadScene(levelIndex);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                 Application.OpenURL(webplayerQuitURL);
        #else
                 Application.Quit();
        #endif
    }

}
