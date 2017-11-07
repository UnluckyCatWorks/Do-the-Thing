using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public LayerMask ignoredByGrabbed;
	public float charSpeed;
	public float rotSpeed;
	public float maxAngle;

	Grabbable grabbedObject;
	float movSpeed;

	[HideInInspector]
	public Transform grabPoint;
	CharacterController me;
	Transform cam;

	private void Update () 
	{
		Crouch ();
		Movement ();
		Rotation ();
		CollisionCheck ();
		Raycasting ();

		#region UNREAL JEJE
		#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying && Input.GetKeyDown (KeyCode.Escape))
			UnityEditor.EditorApplication.isPlaying = false;
		#endif 
		#endregion
	}
	private void Awake() 
	{
		cam = Camera.main.transform;
		me = GetComponent<CharacterController> ();
		grabPoint = Camera.main.transform.GetChild (0);
		// Don't show cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	#region HELPERS
	void Crouch () 
	{
		var crouching = Input.GetKey (KeyCode.LeftShift);
		var target = Vector3.up * (crouching ? 1.9f : 1f);
		var lerp = Vector3.Lerp (me.center, target, Time.deltaTime * 10f);
		var diff = lerp - me.center;
		me.center = lerp;

		// Correct get up
		transform.Translate (-diff);
		// Modify speed
		movSpeed = charSpeed * (crouching ? 0.35f : 1f);
	}

	void Movement () 
	{
		var x = Input.GetAxis ("Horizontal");
		var z = Input.GetAxis ("Vertical");
		var dir = transform.TransformDirection (new Vector3 (x, 0, z) * movSpeed);
		me.SimpleMove (dir);
	}

	void Rotation () 
	{
		// Rotate horizontally
		var h = Input.GetAxis ("Mouse X");
		transform.Rotate (Vector3.up, h * rotSpeed * Time.deltaTime);

		// Rotate vertically (clamped)
		var lastV = cam.localRotation;
		var v = -Input.GetAxis ("Mouse Y");
		cam.Rotate (Vector3.right, v * rotSpeed * Time.deltaTime);

		// Clamp rotation
		var angle = Quaternion.Angle (Quaternion.identity, cam.localRotation);
		if (angle >= maxAngle || angle <= -maxAngle)
			cam.transform.localRotation = lastV;
	}

	void CollisionCheck () 
	{
		// Correct grab position
		while (Physics.CheckSphere (grabPoint.position, 0.18f, ~ignoredByGrabbed))
		{
			grabPoint.Translate (0, 0, -Time.deltaTime);
			if (grabPoint.localPosition.z <= 0.451f)
			{
				var loc = grabPoint.localPosition;
				loc.z = 0.45f;
				grabPoint.localPosition = loc;
				return;
			}
		}

		// Return to position if haven't collided
		var lerp = Vector3.Lerp (grabPoint.localPosition, new Vector3 (0, -0.42f, 1.368f), Time.deltaTime * 2.5f);
		var cachePos = grabPoint.position;
		grabPoint.localPosition = lerp;
		// Check returning won't make collide again
		if (Physics.CheckSphere (grabPoint.position, 0.25f, ~ignoredByGrabbed))
			grabPoint.position = cachePos;
	}

	void Raycasting () 
	{
		if (grabbedObject == null)
		{
			RaycastHit hit;
			var ray = new Ray (cam.position, cam.forward);
			if (Physics.Raycast (ray, out hit, 2f, ~ignoredByGrabbed))
			{
				var grab = hit.collider.GetComponent<Grabbable> ();
				if (grab == null) return;
				if (Input.GetKey (KeyCode.Mouse0))
				{
					grabbedObject = grab;
					grab.Grab ();
				}
			}
		}
	}
	#endregion
}
