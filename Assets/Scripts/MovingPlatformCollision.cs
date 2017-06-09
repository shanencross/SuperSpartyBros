using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCollision : MonoBehaviour {

	private Transform _transform;
	private Transform _player;

	void Awake() {
		_transform = GetComponent<Transform>(); 
		_player = _transform.parent;

		if (_player == null) {
			Debug.LogWarning("This object has no parent. Parent should be Player!");
		}

		else if (_player.CompareTag("Player") == false) {
			Debug.LogWarning("This object's parent is not tagged \"Player\". Parent should be Player!");
		}

	}


	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag=="MovingPlatform") {
			_player.parent = collider.transform;
		}
	}

	// if the player exits a collision with a moving platform, then unchild it
	void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.tag=="MovingPlatform") {
			this.transform.parent = null;
		}
	}
}
