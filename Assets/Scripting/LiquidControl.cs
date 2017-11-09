using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LiquidControl : MonoBehaviour
{
	[Range(0f,1f)]
	public float fillLevel;				// How full it is
	public float fullHeight;            // Height at which vessel is full
	public float emptyHeight;			// Height at which vessel is empty

	int _Level;
	Material mat;

	private void Update()
	{
		var lerp = Mathf.Lerp (emptyHeight, fullHeight, fillLevel);
		mat.SetFloat (_Level, lerp);
	}

	private void OnEnable () 
	{
		_Level = Shader.PropertyToID ("_Level");
		mat = GetComponent<Renderer> ().sharedMaterial;
	}
}