using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPicker : MonoBehaviour, I_Interactable
{
	public void Interact () 
	{
		var p = FindObjectOfType<Player> ();
		p.gun.SetActive (true);
		p.aimingGun = true;
	}
	public bool CanInteract () 
	{
		return true;
	}
}
