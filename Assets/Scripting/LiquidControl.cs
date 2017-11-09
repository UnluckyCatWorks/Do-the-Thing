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
	public IEnumerator FadeColor ( Color target ) 
	{
		var time = 0f;
		var start = mat.color;
		while (mat.color != target)
		{
			var lerp = Color.Lerp (start, target, time);
			mat.color = lerp;

			// Add time duration
			time += Time.deltaTime * 2f;
			yield return null;
		}
	}

	private void OnEnable () 
	{
		_Level = Shader.PropertyToID ("_Level");
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying)
			mat = GetComponent<Renderer> ().material;
		else
			mat = GetComponent<Renderer> ().sharedMaterial;
#else
		mat = GetComponent<Renderer> ().material;
#endif
	}
}