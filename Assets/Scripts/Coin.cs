using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	public int coinValue = 1;
	public bool taken = false;
	public GameObject explosion;

	// if the player touches the coin, it has not already been taken, and the player can move (not dead or victory)
	// then take the coin
	void OnTriggerEnter2D (Collider2D collider)
	{
		Debug.Log("Coin, collider layer: " + LayerMask.LayerToName(collider.gameObject.layer));
		if (collider.gameObject.layer == LayerMask.NameToLayer("Player") && (!taken)) {
			CharacterController2D characterController = collider.gameObject.GetComponentInParent<CharacterController2D>();
			if (characterController.playerCanMove) {
				// mark as taken so doesn't get taken multiple times
				taken=true;

				// if explosion prefab is provide, then instantiate it
				if (explosion)
				{
					Instantiate(explosion,transform.position,transform.rotation);
				}

				// do the player collect coin thing
				characterController.CollectCoin(coinValue);

				// destroy the coin
				DestroyObject(this.gameObject);
			}
		}
	}

}
