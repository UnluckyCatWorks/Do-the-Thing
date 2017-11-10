using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combinations : MonoBehaviour
{
	public static Ingredients combination;

	private void OnTriggerEnter( Collider other )
	{
		var grab = other.GetComponent<Grabbable> ();
		if (grab != null)
		{
			combination |= grab.ingredientType;
			Destroy (other, 0.2f);
		}
	}
}
