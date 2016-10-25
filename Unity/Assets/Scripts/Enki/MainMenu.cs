using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{



    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);

        //switch (SceneManager.GetActiveScene().buildIndex)
        //{
        //    case 0:
        //        SceneManager.LoadScene(1);
        //        break;

        //    case 1:
        //        SceneManager.LoadScene(0);
        //        break;

        //    default:
        //        break;
        //}
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
