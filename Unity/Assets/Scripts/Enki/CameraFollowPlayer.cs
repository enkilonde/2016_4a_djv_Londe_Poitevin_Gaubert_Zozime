using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{

    public Transform player;

    GameStateManager gameStateManagerScript;
    Camera cam;

    Vector3 lookAtposition;
    Transform TargetPosition;

    ParticleSystem speedFx;

    void Awake()
    {
        gameStateManagerScript = FindObjectOfType<GameStateManager>();
        cam = GetComponent<Camera>();
        GameObject PosTarget = new GameObject("CameraTarget");
        TargetPosition = PosTarget.transform;
        TargetPosition.position = transform.position;
        TargetPosition.SetParent(player);
        speedFx = transform.GetChild(0).GetComponent<ParticleSystem>();

    }

    void FixedUpdate ()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPosition.position, Time.deltaTime * 8);
        transform.LookAt(player);

        cam.fieldOfView = Mathf.Lerp(50, 100, gameStateManagerScript.playerCurrentSpeed);

        ParticleSystem.EmissionModule emmMod = speedFx.emission;
        emmMod.rate = Mathf.Lerp(-50, 50, gameStateManagerScript.playerCurrentSpeed);


    }
}
