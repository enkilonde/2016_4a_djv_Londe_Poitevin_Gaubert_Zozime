using UnityEngine;
using System.Collections;

/* Author : Thibaut Poittevin 
 * 
 * Version 1.03
 */

[RequireComponent (typeof (Collider))]
public class DrawCollider : MonoBehaviour 
{
	public bool solid = true, wires = false;
	public Color colorActive = new Color(0, 1, 0, 0.2f);
	public bool drawInactive = true;
	public Color colorInactive = new Color(1, 0, 0, 0.2f);

	void OnDrawGizmos()
	{
		var oldMatrix = Gizmos.matrix;

		Collider collider = GetComponent<Collider> ();

		if (collider.enabled)
			Gizmos.color = colorActive;
		else
			Gizmos.color = colorInactive;

		if (collider.enabled || drawInactive) 
		{
			if (collider is BoxCollider)
				DrawCube((BoxCollider)collider);
			else if (collider is SphereCollider)
				DrawSphere((SphereCollider)collider);
			else if (collider is CapsuleCollider)
				DrawCapsule((CapsuleCollider)collider);
		}

		Gizmos.matrix = oldMatrix;
	}

	void DrawCube(BoxCollider collider)
	{
		Gizmos.matrix = Matrix4x4.TRS (
			transform.TransformPoint(collider.center), 
			transform.rotation,
			transform.localScale);

		if (solid)
			Gizmos.DrawCube (Vector3.zero, collider.size);
		if (wires)
			Gizmos.DrawWireCube(Vector3.zero, collider.size);
	}

	void DrawSphere(SphereCollider collider)
	{
		Gizmos.matrix = Matrix4x4.TRS (
			transform.TransformPoint(collider.center), 
			transform.rotation,
			transform.localScale);

		if (solid)
			Gizmos.DrawSphere (Vector3.zero, collider.radius);
		if (wires) 
			Gizmos.DrawWireSphere (Vector3.zero, collider.radius);
	}
	void DrawCapsule(CapsuleCollider collider)
	{
		Gizmos.matrix = Matrix4x4.TRS (
			transform.TransformPoint(collider.center), 
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, 1.0f));

		Vector3 scale = transform.localScale;
		float radius = collider.radius * Mathf.Max(scale.x, scale.z);

		Vector3 up = Vector3.zero;
		switch (collider.direction) {
		case 0: // X axis
			up = (collider.height * scale.y / 2 - radius) * transform.right;
			break;
		case 1: // Y axis, default
			up = (collider.height * scale.y / 2 - radius) * transform.up;
			break;
		case 2 : // Z axis
			up = (collider.height * scale.y / 2 - radius) * transform.forward;
			break;
		}

		Vector3 right = radius * transform.right;
		Vector3 forward = radius * transform.forward;
	
		if (solid) {
			Gizmos.DrawSphere (up, radius);
			Gizmos.DrawSphere (-up, radius);
		}
		if (wires) {
			Gizmos.DrawWireSphere (up, radius);
			Gizmos.DrawWireSphere (-up, radius);
		}
		if (solid || wires) {
			Gizmos.DrawLine (up + right, -up + right);
			Gizmos.DrawLine (up - right, -up - right);
			Gizmos.DrawLine (up + forward, -up + forward);
			Gizmos.DrawLine (up - forward, -up - forward);
		}
	}




}
