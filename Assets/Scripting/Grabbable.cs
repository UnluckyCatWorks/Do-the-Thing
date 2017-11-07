using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
	public Rigidbody body;

	public void Grab ()
	{
		// Disable collision to avoid breaking grab point
		foreach (var c in GetComponents<Collider> ()) c.enabled = false;
		// Make kinematic to avoid physic issues
		body.isKinematic = true;
		StartCoroutine ("GoToGrabPoint");
	}

	IEnumerator GoToGrabPoint () 
	{
		var p = FindObjectOfType<Player> ();
		float time=0f;
		while ( Vector3.Distance ( transform.position, p.grabPoint.position ) != 0 )
		{
			var lerp = Vector3.Lerp (transform.position, p.grabPoint.position, time );
			transform.position = lerp;
			// Add time duration
			time += Time.deltaTime / 0.18f;
			yield return null;
		}
		// When in place, set parenting
		transform.SetParent (p.grabPoint);
		grabbed = true;
	}

	bool grabbed;
	private void Update() 
	{
		if (!grabbed) return;

		var lerp = Quaternion.Lerp (transform.rotation, Quaternion.identity, Time.deltaTime * 10f );
		transform.rotation = lerp;
	}
}
