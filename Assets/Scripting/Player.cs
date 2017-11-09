using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region INTERNAL DATA
	public static Material outlineMat;
	public static Transform grabPoint;

	CharacterController me;
	Transform cam;

	public float charSpeed;
	float movSpeed;

	public float rotSpeed;
	public float maxAngle;
	#endregion

	private void Update () 
	{
		Crouch ();
		Movement ();
		Rotation ();
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
		// Don't show cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		// Setup refernces
		grabPoint = Camera.main.transform.GetChild (0);
		me = GetComponent<CharacterController> ();
		cam = Camera.main.transform;
		outlineMat = Resources.Load<Material> ("Materials/Outline");
	}

	#region HELPERS
	void Crouch () 
	{
		var crouching = Input.GetKey (KeyCode.LeftShift);
		var target = Vector3.up * (crouching ? 0.6f : 1.79f);
		var lerp = Vector3.Lerp (cam.localPosition, target, Time.deltaTime * 5f);
		cam.localPosition = lerp;

		// Modify speed
		movSpeed = charSpeed * (crouching ? 0.35f : 1f);
	}
	void Movement () 
	{
		var x = Input.GetAxis ("Horizontal");
		var z = Input.GetAxis ("Vertical");
		var dir = transform.TransformDirection (new Vector3 (x, 0, z)) * movSpeed;
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
	void Raycasting () 
	{
		if (Grabbable.current == null)
		{
			// Find object to grab / interact
			RaycastHit hit;
			var ray = new Ray (cam.position, cam.forward);
			if (Physics.Raycast (ray, out hit, 2f))
			{
				// Is a interactable object?
				var obj = hit.collider.GetComponent<I_Interactable> ();
				if (obj != null)
				{
					if ( obj.CanInteract () )
					{
						ShowIcon ();
						if (Input.GetKeyDown (KeyCode.Mouse0))
						{
							obj.Interact ();
							HideIcon ();
						}
						return;
					}
				}
			}
			// In case anything fails
			HideIcon ();
		}
		else
		{
			// Can't interact while holding an object
			if (!Input.GetKey (KeyCode.Mouse0))
			{
				Grabbable.current.Drop ( me.velocity );
				Grabbable.current = null;
			}
		}
	}
	#endregion

	#region INTERACTING ICON
	public SpriteRenderer icon;
	bool iconIsIn;

	void ShowIcon () 
	{
		if (iconIsIn) return;
		else
		{
			StopCoroutine ("FadeIcon");
			StartCoroutine (FadeIcon (0.6f));
			iconIsIn = true;
		}
	}
	void HideIcon () 
	{
		if (!iconIsIn) return;
		else
		{
			StopCoroutine ("FadeIcon");
			StartCoroutine (FadeIcon (0));
			iconIsIn = false;
		}
	}
	IEnumerator FadeIcon ( float target ) 
	{
		var time = 0f;
		var start = icon.color.a;
		while (icon.color.a < target)
		{
			var lerp = Mathf.Lerp (start, target, time);
			icon.color = new Color (1, 1, 1, lerp);

			// Add time duration
			time += Time.deltaTime * 0.5f;
			yield return null;
		}
	}
	#endregion
}
