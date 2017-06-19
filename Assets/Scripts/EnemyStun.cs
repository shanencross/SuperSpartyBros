using UnityEngine;
using System.Collections;

public class EnemyStun : MonoBehaviour {

	// if Player hits the stun point of the enemy, then call Stunned on the enemy
	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
			CharacterController2D player = collision.gameObject.GetComponentInParent<CharacterController2D>();
			// tell the enemy to be stunned
			this.GetComponentInParent<Enemy>().Stunned();

			// make the player bounce off the enemy
			player.Bounce();
		}
	}
}
