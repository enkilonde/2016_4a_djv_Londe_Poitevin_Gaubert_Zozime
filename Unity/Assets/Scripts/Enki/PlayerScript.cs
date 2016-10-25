using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.UI;


public class PlayerScript : MonoBehaviour
{

    public float rotationSpeed = 5;
    public float speed = 10;
    public float grassMultiplier = 0.5f;

    Collider[] AllGrass;


    void Start ()
	{
        var gameUpdate = Observable.EveryUpdate();

        GameObject[] allgrasstemp = GameObject.FindGameObjectsWithTag("Grass");
        AllGrass = new Collider[allgrasstemp.Length];
        for (int i = 0; i < allgrasstemp.Length; i++)
        {
            AllGrass[i] = allgrasstemp[i].GetComponent<Collider>();
        }

        gameUpdate.Where(_ => Input.GetAxisRaw("Horizontal") != 0).Subscribe(_ => transform.Rotate(0, rotationSpeed * Time.deltaTime * Input.GetAxisRaw("Horizontal"), 0));
        gameUpdate.Where(_ => Input.GetAxisRaw("Vertical") != 0).Subscribe(_ => goForward());
    }

    void goForward()
    {
        float mult = 1;
        if (IsInGrass()) mult = 0.5f;
        transform.position += transform.forward * speed * Time.deltaTime * mult;

    }

    bool IsInGrass()
    {
        for (int i = 0; i < AllGrass.Length; i++)
        {
            if (AllGrass[i].bounds.Contains(transform.position)) return true;
        }
        return false;
    }


}
