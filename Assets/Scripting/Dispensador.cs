using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispensador : MonoBehaviour
{
	public Ingredients type;
	public Color liquidColor;
	public Color particleColor;

	private void OnParticleCollision ( GameObject other )
	{
		if (other.tag != "Liquid_Vessel") return;
		var grab = other.GetComponent<Grabbable> ();
		var liquid = other.GetComponentInChildren<LiquidControl> ();

		var increment = Time.deltaTime * 0.5f;
		liquid.fillLevel += increment;
		if (liquid.fillLevel > 1) liquid.fillLevel = 1;

		if (grab.ingredientType != type)
		{
			// Change liquid and liquid particle colors
			liquid.StartCoroutine (liquid.FadeColor (liquidColor));
			var main = liquid.GetComponent<LiquidGravity> ().ps.main;
			main.startColor = liquidColor;
			// Change ingredient
			grab.ingredientType = type;
		}
	}
}
