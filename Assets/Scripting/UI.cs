using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour 
{
	public void Play () 
	{
		SceneManager.LoadScene ("Main");
	}

	public void Exit () 
	{
		Application.Quit ();
	}

//	public void Controls
//	public void Credits
}
