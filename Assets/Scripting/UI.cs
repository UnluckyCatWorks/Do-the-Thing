using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

	public void ResetBTN ( GameObject btn ) 
	{
		var s = btn.GetComponent<Image> ();
		s.color = new Color (1, 1, 1, 0);
		s.sprite = null;
	}

//	public void Controls
//	public void Credits
}
