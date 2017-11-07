using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float movSpeed;
	public float rotSpeed;
	public float maxAngle;
	CharacterController me;

	private void Update () 
	{
		// Movement
		var x = Input.GetAxis ("Horizontal");
		var z = Input.GetAxis ("Vertical");
		var dir = transform.TransformDirection (new Vector3 (x, 0, z) * movSpeed);
		me.SimpleMove (dir);

		// Rotation
		var h = Input.GetAxis ("Mouse X");
		transform.Rotate (Vector3.up, h * rotSpeed * Time.deltaTime);

		// Rotate vertically (limited)
		var cam = Camera.main.transform;
		var lastV = cam.localRotation;
		var v = -Input.GetAxis ("Mouse Y");
		cam.Rotate (Vector3.right, v * rotSpeed * Time.deltaTime);
		var angle = Quaternion.Angle (Quaternion.identity, cam.localRotation);
		if ( angle >= maxAngle || angle <= -maxAngle )
		{
			cam.transform.localRotation = lastV;
		}

/// Unreal jeje
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying && Input.GetKeyDown (KeyCode.Escape))
			UnityEditor.EditorApplication.isPlaying =  false;
#endif
	}

	private void Awake() 
	{
		me = GetComponent<CharacterController> ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
