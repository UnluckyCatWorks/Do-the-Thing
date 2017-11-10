using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBTN : MonoBehaviour, I_Interactable
{
	public Animator anim;
	public void Interact () 
	{
		// Close caldero
		anim.SetTrigger ("Close");
	}
	public bool CanInteract () 
	{
		return true;
	}
}
