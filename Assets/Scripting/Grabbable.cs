using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour, I_Interactable
{
	public static Grabbable current;

	public Ingredients ingredientType;
	Rigidbody body;
	bool colliding;
	bool grabbed;

	public bool CanInteract () 
	{
		return current!=this;
	}
	public void Interact () 
	{
		if (name == "Caca")
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Menu");
		// -> Grab object
		current = this;
		// Make kinematic to avoid physic issues
		foreach (var c in GetComponents<Collider> ()) c.isTrigger = true;
		body.isKinematic = true;
		StartCoroutine ("CarryObject");
		// Discard any collision
		colliding = false;
	}
	public void Drop ( Vector3 speed ) 
	{
		// Stop carrying object
		grabbed = false;
		transform.SetParent (null);
		StopCoroutine ("CarryObject");
		// Start applying physix again
		foreach (var c in GetComponents<Collider> ()) c.isTrigger = false;
		body.isKinematic = false;
		body.velocity = speed;
	}
	IEnumerator CarryObject () 
	{
		// Smooth translate object to player
		float time=0f;
		var p = Player.grabPoint;
		while ( Vector3.Distance ( transform.position, p.position ) > 0 )
		{
			if (!grabbed && time>0.2f) grabbed = true; 

			var lerp = Vector3.Lerp (transform.position, p.position, time );
			transform.position = lerp;

			// Add time duration
			time += Time.deltaTime * 4;
			yield return null;
		}
		transform.SetParent (Player.grabPoint);
	}

	private void Update () 
	{
		// Keep object un-rotated
		if (current!=this) return;
		var lerp = Quaternion.Lerp (transform.rotation, Quaternion.identity, Time.deltaTime * 0.95f );
		lerp.eulerAngles += new Vector3 (-3f * Time.deltaTime, 0, -10 * Time.deltaTime);
		transform.rotation = lerp;

		// If collision, drop object
		if (!grabbed) return;
		if (colliding)
		{
			Drop ( -FindObjectOfType<Player> ().transform.forward );
			current = null;
			return;
		}
	}
	private void OnTriggerEnter (Collider col) 
	{
		if (col.tag == "Player") return;
		if (!grabbed) return;
		colliding = true;
	}
	private void Awake () 
	{
		body = GetComponent<Rigidbody> ();
	}
}
