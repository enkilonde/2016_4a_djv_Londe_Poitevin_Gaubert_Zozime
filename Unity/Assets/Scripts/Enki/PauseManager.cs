using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public Canvas pauseCanvas;

    public bool paused = true;

    public Transform canvasReadyGo;
    public float delayBetweenNumbers = 1.25f;
    public Transform endTrackCanvas;

    void Start ()
    {
        StartCoroutine(ReadyGo(delayBetweenNumbers));
	}

    IEnumerator ReadyGo (float delayBetweenNumbers)
    {
        canvasReadyGo.gameObject.SetActive(true);

        yield return new WaitForSeconds(delayBetweenNumbers);

        canvasReadyGo.GetChild(0).gameObject.SetActive(true); // 3
        yield return new WaitForSeconds(delayBetweenNumbers);

        canvasReadyGo.GetChild(0).gameObject.SetActive(false);
        canvasReadyGo.GetChild(1).gameObject.SetActive(true); // 2
        yield return new WaitForSeconds(delayBetweenNumbers);

        canvasReadyGo.GetChild(1).gameObject.SetActive(false);
        canvasReadyGo.GetChild(2).gameObject.SetActive(true); // 1
        yield return new WaitForSeconds(delayBetweenNumbers);

        paused = false;
        canvasReadyGo.GetChild(2).gameObject.SetActive(false);
        canvasReadyGo.GetChild(3).gameObject.SetActive(true); // GO
        yield return new WaitForSeconds(delayBetweenNumbers);

        canvasReadyGo.GetChild(3).gameObject.SetActive(false);
        canvasReadyGo.gameObject.SetActive(false);
    }

    void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TriggerPause();
        }

	}

    public void TriggerPause()
    {
        pauseCanvas.enabled = !pauseCanvas.enabled;
        paused = pauseCanvas.enabled;
    }

    public void EndTrack(bool isPlayerWinner)
    {
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
