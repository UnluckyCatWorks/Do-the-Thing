using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[HideInInspector]
	public bool active;
	NavMeshAgent ia;
	Transform player;

	public GameObject ps_Poff;
	public GameObject caca;
	public GameObject unicorn;
	Ingredients unicornRecipe =
		Ingredients.Anti_Matter |
		Ingredients.Destiled_Water |
		Ingredients.Quantic_Pears |
		Ingredients.Flower |
		Ingredients.Soap |
		Ingredients.Substance_X |
		Ingredients.Unicorn_Blood |
		Ingredients.Uranium_Juice;

	private void Update ()
	{
		if (!active) return;
		ia.SetDestination (player.position);
		if (Vector3.Distance ( player.position, transform.position ) < 2.1f)
		{
			// dying
			SceneManager.LoadScene ("Menu");
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void Awake()
	{
		ia = GetComponent<NavMeshAgent> ();
		player = FindObjectOfType<Player> ().transform;
	}

	float vida=100;
	private void OnParticleCollision( GameObject other ) 
	{
		if (other.name != "Shooting") return;
		if (vida < 0)
		{
			var r = Combinations.combination;
			if ( r == unicornRecipe )
			{
				Instantiate (unicorn, transform.position + Vector3.up * 1.5f, Quaternion.Euler(0,180,0));
				Destroy (gameObject);
			}
			else if ((r & Ingredients.Cactus) == Ingredients.Cactus)
			{
				Instantiate (caca, transform.position + Vector3.up * 1.5f, Quaternion.identity);
				Destroy (gameObject);
			}
		}
		else vida -= 95 * Time.deltaTime;
	}
}
