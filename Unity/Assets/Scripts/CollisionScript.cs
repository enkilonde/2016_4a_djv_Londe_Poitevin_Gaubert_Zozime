using UnityEngine;
using System.Collections;

public class CollisionScript
{
    // walls = tableau de murs à tester  |  PlayerPos = Position à tester pour collision  |  radius = rayon de collision de l'unité  |   vel = vitesse + direction de l'unité
    public static Vector3 CollisionManage(Transform[] walls, Vector3 PlayerPos, float radius, Vector3 vel)
    {
        int nbcoll = 0;
        Vector3 prevColl = new Vector3(0, 0, 0);
        Vector3 result = new Vector3(0, 0, 0);
        for (int i = 0; i < walls.Length; i++)
        {
            Vector3 n_Pos = ReplaceWallCollision(walls[i], PlayerPos, radius, vel);
            if (nbcoll > 0 && prevColl != Vector3.zero)
            {
                if (n_Pos != Vector3.zero)
                {
                    result = new Vector3((n_Pos.x + prevColl.x) * 0.5f, PlayerPos.y, (n_Pos.z + prevColl.z) * 0.5f);
                }

            }
            else
            {
                if (n_Pos != Vector3.zero)
                {
                    //Debug.Log("X : " + n_Pos.x + " | Y : " + n_Pos.y + " | Z : " + n_Pos.z);
                    nbcoll++;
                    prevColl = n_Pos;
                    result = n_Pos;
                }
            }
        }
        return result;
    }

    static Vector3 ReplaceWallCollision(Transform wall, Vector3 PlayerPos, float PlayerRadius, Vector3 vel)
    {
        bool XisLenght;
        float lenght;
        float radius;
        Vector3 WallOrigin;
        Vector3 WallEnd;

        if (wall.localScale.x >= wall.localScale.z)
        {   // X est la longueur du mur donc Right est la direction de la droite
            lenght = wall.localScale.x;
            radius = wall.localScale.z * 0.5f + PlayerRadius;
            WallOrigin = wall.transform.position - wall.right * 0.5f * lenght;
            WallEnd = wall.transform.position + wall.right * 0.5f * lenght;
            XisLenght = true;
        }
        else
        {   // Z est la longueur du mur donc Forward est la direction de la droite
            lenght = wall.localScale.z;
            radius = wall.localScale.x * 0.5f + PlayerRadius;
            WallOrigin = wall.transform.position - wall.forward * 0.5f * lenght;
            WallEnd = wall.transform.position + wall.forward * 0.5f * lenght;
            XisLenght = false;
        }

        Vector2 WallOrigin2D = new Vector2(WallOrigin.x, WallOrigin.z);
        Vector2 WallEnd2D = new Vector2(WallEnd.x, WallEnd.z);

        if (CollisionSegment(WallOrigin2D, WallEnd2D, PlayerPos, radius))
        {
            Vector3 posFromWall;
            posFromWall = wall.InverseTransformPoint(PlayerPos);
            Vector3 rr; // Position retourné après collision;
            //Debug.Log("Position par rapport au mur => X : " + posFromWall.x + " | Y : " + posFromWall.y + " | Z : " + posFromWall.z);
            if (XisLenght)
            {
                if (posFromWall.z > 0)
                {
                    posFromWall = new Vector3(posFromWall.x, posFromWall.y, radius / wall.localScale.z);
                }
                else
                {
                    posFromWall = new Vector3(posFromWall.x, posFromWall.y, -radius / wall.localScale.z);
                }
            }
            else
            {
                if (posFromWall.x > 0)
                {
                    posFromWall = new Vector3(radius / wall.localScale.x, posFromWall.y, posFromWall.z);
                }
                else
                {
                    posFromWall = new Vector3(-radius / wall.localScale.x, posFromWall.y, posFromWall.z);
                }
            }

            rr = wall.transform.TransformPoint(posFromWall);
            //Debug.Log("Position replacé => X : " + rr.x + " | Y : " + rr.y + " | Z : " + rr.z);
            return rr;
        }
        else
        {
            //return PlayerPos;
            return Vector3.zero;
        }

    }

    static Vector3 CheckWallCollision(Transform wall, Vector3 PlayerPos, float PlayerRadius, Vector3 vel)
    {
        bool XisDirection;
        float lenght;
        float radius;
        Vector3 WallOrigin;
        Vector3 WallEnd;

        if (wall.localScale.x >= wall.localScale.z)
        {   // X est la longueur du mur donc Right est la direction de la droite
            lenght = wall.localScale.x;
            radius = wall.localScale.z * 0.5f + PlayerRadius;
            WallOrigin = wall.transform.position - wall.right * 0.5f * lenght;
            WallEnd = wall.transform.position + wall.right * 0.5f * lenght;
            XisDirection = true;
        }
        else
        {   // Z est la longueur du mur donc Forward est la direction de la droite
            lenght = wall.localScale.z;
            radius = wall.localScale.x * 0.5f + PlayerRadius;
            WallOrigin = wall.transform.position - wall.forward * 0.5f * lenght;
            WallEnd = wall.transform.position + wall.forward * 0.5f * lenght;
            XisDirection = false;
        }

        Vector2 WallOrigin2D = new Vector2(WallOrigin.x, WallOrigin.z);
        Vector2 WallEnd2D = new Vector2(WallEnd.x, WallEnd.z);

        if (CollisionDroite(WallOrigin2D, WallEnd2D, PlayerPos, radius))
        {
            if (XisDirection)
            {
                return Vector3.Project(vel, wall.right);
            }
            else
            {
                return Vector3.Project(vel, wall.forward);
            }
        }
        else
        {
            return new Vector3(0, 0, 0);
        }

    }

    //A = Wall Origin, B = Wall End
    static bool CollisionDroite(Vector2 A, Vector2 B, Vector3 C, float radius)
    {
        Vector2 u;
        u.x = B.x - A.x;
        u.y = B.y - A.y;
        Vector2 AC;
        AC.x = C.x - A.x;
        AC.y = C.z - A.y;
        float numerateur = u.x * AC.y - u.y * AC.x;   // norme du vecteur v
        if (numerateur < 0)
            numerateur = -numerateur;   // valeur absolue ; si c'est négatif, on prend l'opposé.
        float denominateur = Mathf.Sqrt(u.x * u.x + u.y * u.y);  // norme de u
        float CI = numerateur / denominateur;
        //Debug.Log(CI);
        if (CI < radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static bool CollisionSegment(Vector2 A, Vector2 B, Vector3 C, float radius)
    {
        if (CollisionDroite(A, B, C, radius) == false)
            return false;  // si on ne touche pas la droite, on ne touchera jamais le segment
        Vector2 AB, AC, BC;
        AB.x = B.x - A.x;
        AB.y = B.y - A.y;
        AC.x = C.x - A.x;
        AC.y = C.z - A.y;
        BC.x = C.x - B.x;
        BC.y = C.z - B.y;
        float pscal1 = AB.x * AC.x + AB.y * AC.y;  // produit scalaire
        float pscal2 = (-AB.x) * BC.x + (-AB.y) * BC.y;  // produit scalaire
        if (pscal1 >= 0 && pscal2 >= 0)
            return true;   // I entre A et B, ok.
                           // dernière possibilité, A ou B dans le cercle
                           /*
                           if (CollisionPointCercle(A, C))
                               return true;
                           if (CollisionPointCercle(B, C))
                               return true;
                           */
        return false;
    }


}

