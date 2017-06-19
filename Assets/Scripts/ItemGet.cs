using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGet : MonoBehaviour {

	public string itemName = "default";
	public GameObject explosion;
	public AudioClip collectionSound;
	public bool itemAdded = false; // flag for if item has ben added
	public bool pauseGame = true; // pause the game when item is collected

	private AudioSource _audio;

	void Awake() {
		_audio = GetComponent<AudioSource> ();

		if (_audio == null) {
			_audio = gameObject.AddComponent<AudioSource>();
		}
	}


	void OnTriggerEnter2D(Collider2D collider) {
		if (itemAdded == false && collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
			Debug.Log("Player colliding with item");
			//PlaySound(collectionSound);
			AddItemToList();
			EraseItem();

			itemAdded = true;
		}
	}

	void AddItemToList() {
		if (ItemInventory.iv) {
			Debug.Log("Adding item to play ItemInventory");
			ItemInventory.iv.AddItem(itemName, pauseGame);
		} 

		else {
			Debug.Log("ItemInventory not set up");
		}
	}

	void PlaySound(AudioClip sound, float volume = 0.8f) {
		if (_audio != null) {
			Debug.Log("playing audio clip");
			_audio.PlayOneShot(sound, volume);
		}
	}

	void EraseItem() {
		// if explosion prefab is provide, then instantiate it
		if (explosion)
		{
			Instantiate(explosion,transform.position,transform.rotation);
		}

		Destroy(this.gameObject);
	}

}
