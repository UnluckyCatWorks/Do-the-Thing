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
		// Disable capsule mesh
		transform.GetChild (1).gameObject.SetActive (false);
	}

	#region HELPERS
	void Crouch () 
	{
		var crouching = Input.GetKey (KeyCode.LeftControl);
		var target = Vector3.up * (crouching ? 0.6f : 1.79f);
		var lerp = Vector3.Lerp (cam.localPosition, target, Time.deltaTime * 5f);
		cam.localPosition = lerp;

		// Modify speed
		movSpeed = charSpeed * (crouching ? 0.35f : 1f);
		if (Input.GetKey (KeyCode.LeftShift))
		{
			// Auto-drop when sprinting
			movSpeed = charSpeed * 1.35f;
			if (Grabbable.current!=null)
			{
				Grabbable.current.Drop ( me.velocity * 0.75f);
				Grabbable.current = null;
			}
		}
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

	[HideInInspector]
	public bool aimingGun;
	public GameObject gun;
	public ParticleSystem gunPs;
	void Raycasting () 
	{
		if (aimingGun)
		{
			if (Input.GetKeyDown (KeyCode.Mouse0))
			{
				//-> Disparar
				gunPs.Play ();
			}
			if (Input.GetKeyUp (KeyCode.Mouse0))
			{
				//-> Stop
				gunPs.Stop ();
			}
		}

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
			StopCoroutine ("FadeOut");
			StartCoroutine ("FadeIn");
			iconIsIn = true;
		}
	}
	void HideIcon () 
	{
		if (!iconIsIn) return;
		else
		{
			StopCoroutine ("FadeIn");
			StartCoroutine ("FadeOut");
			iconIsIn = false;
		}
	}
	IEnumerator FadeIn () 
	{
		var time = 0f;
		var start = icon.color.a;
		while (icon.color.a != 1)
		{
			var lerp = Mathf.Lerp (start, 1, time);
			icon.color = new Color (1, 1, 1, lerp);

			// Add time duration
			time += Time.deltaTime * 1.5f;
			yield return null;
		}
	}
	IEnumerator FadeOut ()
	{
		var time = 0f;
		var start = icon.color.a;
		while (icon.color.a != 0)
		{
			var lerp = Mathf.Lerp (start, 0, time);
			icon.color = new Color (1, 1, 1, lerp);

			// Add time duration
			time += Time.deltaTime * 1.5f;
			yield return null;
		}
	}
	#endregion
}
