using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public Canvas pauseCanvas;

    bool isGameStopped = true;
    bool isInMenu = false;

    public bool paused
    {
        get
        {
            if (isInMenu || isGameStopped)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Transform canvasReadyGo;
    public float delayBetweenNumbers = 1.25f;
    public Transform endTrackCanvas;

    public Text chronometerText;
    float elapsedTime = 0f;

    void Start ()
    {
        StartCoroutine(ReadyGo(delayBetweenNumbers));
        Debug.Log("Paused = " + isGameStopped + " + "  + isInMenu + " + " + paused);
	}

    IEnumerator ReadyGo (float delayBetweenNumbers)
    {
        canvasReadyGo.gameObject.SetActive(true);
        yield return new WaitForSeconds(delayBetweenNumbers);

        while (isInMenu) {
            yield return new WaitForEndOfFrame();
        }

        canvasReadyGo.GetChild(0).gameObject.SetActive(true); // 3
        yield return new WaitForSeconds(delayBetweenNumbers);

        while (isInMenu)
        {
            yield return new WaitForEndOfFrame();
        }
        canvasReadyGo.GetChild(0).gameObject.SetActive(false);
        canvasReadyGo.GetChild(1).gameObject.SetActive(true); // 2
        yield return new WaitForSeconds(delayBetweenNumbers);

        while (isInMenu)
        {
            yield return new WaitForEndOfFrame();
        }
        canvasReadyGo.GetChild(1).gameObject.SetActive(false);
        canvasReadyGo.GetChild(2).gameObject.SetActive(true); // 1
        yield return new WaitForSeconds(delayBetweenNumbers);

        while (isInMenu)
        {
            yield return new WaitForEndOfFrame();
        }
        if (canvasReadyGo.gameObject.activeSelf) isGameStopped = false;  
        canvasReadyGo.GetChild(2).gameObject.SetActive(false);
        canvasReadyGo.GetChild(3).gameObject.SetActive(true); // GO
        yield return new WaitForSeconds(delayBetweenNumbers);


        canvasReadyGo.GetChild(3).gameObject.SetActive(false);
        canvasReadyGo.gameObject.SetActive(false);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Skip ReadyGo
        {
            StopCoroutine(ReadyGo(delayBetweenNumbers));
            isGameStopped = false;
            canvasReadyGo.gameObject.SetActive(false);
        }


        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TriggerPause();
        }

        if (!paused)
        {
            UpdateChronometer();
        }

	}

    public void UpdateChronometer()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime) / 60;
        int seconds = Mathf.FloorToInt(elapsedTime) % 60; 
        int decimals = Mathf.FloorToInt(elapsedTime * 100f) % 100;
        string theMinutes;
        string theSeconds;
        string theDecimals;
        if (minutes < 10) { theMinutes = "0" + minutes; } else { theMinutes = "" + minutes; }
        if (seconds < 10) { theSeconds = "0" + seconds; } else { theSeconds = "" + seconds; }
        if (decimals < 10) { theDecimals = "0" + decimals; } else { theDecimals = "" + decimals; }
        chronometerText.text = "Time = " + theMinutes + " : " + theSeconds + " \" " + theDecimals;
    }

    public void TriggerPause()
    {
        pauseCanvas.enabled = !pauseCanvas.enabled;
        isInMenu = pauseCanvas.enabled;
    }

    public void EndTrack(bool isPlayerWinner)
    {
        isGameStopped = true;

        endTrackCanvas.gameObject.SetActive(true);

        if (isPlayerWinner)
        {
            endTrackCanvas.Find("LoserText").gameObject.SetActive(false);
        }
        else
        {
            endTrackCanvas.Find("WinnerText").gameObject.SetActive(false);
        }
    }
}
