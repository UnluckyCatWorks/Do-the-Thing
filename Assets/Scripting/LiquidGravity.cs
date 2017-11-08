using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidGravity : MonoBehaviour
{
	public AnimationCurve pourCurve;
	public ParticleSystem ps;
	public float minAngle;
	LiquidControl liquid;

	void Update () 
	{
		var intensity = AngleCheck ();
		if (intensity > 0)
		{
			var decrement = Time.deltaTime * intensity;
			liquid.fillLevel -= decrement;

			// Start emiitng poured liquid
			ps.Emit ((int) (150f * Time.deltaTime));
		}
	}

	float AngleCheck ()
	{
		// Find inclination angle
		var wEuler = transform.eulerAngles;
		// Discard top-down rotation
		wEuler.y = 0;
		var q = Quaternion.Euler (wEuler);
		var angle = Quaternion.Angle (q, Quaternion.identity);

		// If angle is enough to start pouring,
		// returns how intense is the pouring
		if (angle >= minAngle)
		{
			// How pronunced is the angle
			var factor = pourCurve.Evaluate (angle);
			var threshold = (1-liquid.fillLevel);
			if (factor > threshold) return factor;
			else					return 0;
		}
		else return 0;
	}

	private void Awake () 
	{
		liquid = GetComponentInChildren<LiquidControl> ();
	}
}
