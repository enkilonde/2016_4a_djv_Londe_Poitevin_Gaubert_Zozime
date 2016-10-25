using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {

    public float rotationSpeed = 5;
    public float speed = 10;
    public float grassMultiplier = 0.5f;

    LevelDisposition levelDispoScript;

    // Use this for initialization
    void Awake ()
    {
        levelDispoScript = FindObjectOfType<LevelDisposition>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    protected void rotateEntity(float value)
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime * value, 0);
    }

    protected void goForward()
    {
        float mult = 1;
        if (isEntityInGrass(transform.position)) mult = 0.5f;
        transform.position += transform.forward * speed * Time.deltaTime * mult;
    }


    Transform GetEntityTile(Vector3 entityPos)
    {
        //Debug.Log("Player pos : (" + (int)entityPos.x / 10 + ", " + (int)entityPos.z / 10 + ")");
        return levelDispoScript.tiles[(int)entityPos.x / 10, (int)entityPos.z / 10].transform;
    }

    Vector2 GetPosInTile(Vector3 pos, Vector3 Tilepos)
    {
        return new Vector2(pos.x - Tilepos.x, pos.z - Tilepos.z);
    }

    bool isEntityInGrass(Vector3 entityPos)
    {

        Transform tile = GetEntityTile(entityPos);

        Vector2 relativePos = GetPosInTile(entityPos, tile.position);

        switch (tile.name)
        {
            case "roadtile_checkpoint":
                return GetStraightTileGrass(tile, relativePos);

            case "roadtile_curve":
                GetCurvedGrass(tile, relativePos);
                break;

            case "roadtile_straight":
                return GetStraightTileGrass(tile, relativePos);

            default:
                break;
        }


        return false;
    }

    bool GetStraightTileGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 2.5f || relativpos.y < -2.5f) return true;
        }
        else
        {
            if (relativpos.x > 2.5f || relativpos.x < -2.5f) return true;
        }

        return false;
    }

    bool GetCurvedGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 2.5f || relativpos.y < -2.5f) return true;
        }
        else
        {
            if (relativpos.x > 2.5f || relativpos.x < -2.5f) return true;
        }

        Vector3 pivot = tile.GetChild(0).position;
        pivot.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, pivot); //OPTIMISABLE!!!
        

        return false;
    }




}
