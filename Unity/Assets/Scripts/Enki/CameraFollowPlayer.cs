using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{

    public Transform player;

    GameStateManager gameStateManagerScript;
    Camera cam;

    Vector3 lookAtposition;
    Transform TargetPosition;

    void Awake()
    {
        gameStateManagerScript = FindObjectOfType<GameStateManager>();
        cam = GetComponent<Camera>();
        GameObject PosTarget = new GameObject("CameraTarget");
        TargetPosition = PosTarget.transform;
        TargetPosition.position = transform.position;
        TargetPosition.SetParent(player);
    }

    void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPosition.position, Time.deltaTime * 8);
        transform.LookAt(player);

        cam.fieldOfView = Mathf.Lerp(50, 100, gameStateManagerScript.playerCurrentSpeed);


    }
}
