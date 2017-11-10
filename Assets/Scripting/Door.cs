using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour, I_Interactable
{
	public Text timer;
	public Animator anim;
	public void Interact () 
	{
		min = 0;
		sec = 10;
		anim.SetTrigger ("Pressed");
	}
	public bool CanInteract () 
	{
		return true;
	}
	private void Awake () 
	{
		min = 0;
		sec = 10;
		StartCoroutine ("CountDown");
	}

	int min, sec;
	IEnumerator CountDown () 
	{
		while ( !(min==0 && sec==0) )
		{
			yield return new WaitForSeconds (1f);
			sec--;
			if (sec==-1)
			{
				min--;
				sec = 59;
			}
			timer.text = BuildTime (min, sec);
		}
		//-> Cuando se acabe el tiempo
		anim.SetTrigger ("DoorUp");
	}
	string BuildTime (int min, int sec) 
	{
		string text;
		text = "0" + min;
		text += ":";
		if (sec<10) text += "0" + sec;
		else		text += sec;
		return text;
	}
}
