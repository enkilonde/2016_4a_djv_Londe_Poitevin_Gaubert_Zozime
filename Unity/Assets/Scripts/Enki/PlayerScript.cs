using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.UI;


public class PlayerScript : Vehicle
{



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

        gameUpdate.Where(_ => Input.GetAxisRaw("Horizontal") != 0).Subscribe(_ => rotateEntity(Input.GetAxisRaw("Horizontal")));
        gameUpdate.Where(_ => Input.GetAxisRaw("Vertical") != 0).Subscribe(_ => goForward(Input.GetAxisRaw("Vertical")));
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
