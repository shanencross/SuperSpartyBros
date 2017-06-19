using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ItemInventory : MonoBehaviour {
	public static ItemInventory iv;
	public bool doubleJump = false;
	public bool dash = false;
	public bool lavaBoots = false;
	public GameObject ItemMessage;
	public bool itemMessageDisplayed = false;
	public bool gamePaused = false; // if the game is paused for item collection
	public string advanceMessageButton = "Start";


	public void Awake() {
		if (iv == null)
			iv = this.GetComponent<ItemInventory>();

		if (ItemMessage == null)
			Debug.LogError("Need to set Item Message on Game Manager's Item Inventory");
	}

	public void Update() {
		if (itemMessageDisplayed && CrossPlatformInputManager.GetButtonDown(advanceMessageButton)) {
			DeactivateItemMessage();

			if (gamePaused)
				UnPause();
		}
	}

	void UnPause() {
		gamePaused = false;
		Time.timeScale = 1f;
	}

	public void ActivateItemMessage(string itemName, bool pauseGame=false) {
		if (ItemMessage) {
			ItemMessage.GetComponent<TextDisplay>().SetText(itemName);
			ItemMessage.SetActive(true);
			itemMessageDisplayed = true;

			if (pauseGame) {
				gamePaused = true;
				Time.timeScale = 0f;
			}
		}
	}

	public void DeactivateItemMessage() {
		if (ItemMessage) {
			itemMessageDisplayed = false;
			ItemMessage.SetActive(false);
		}
	}

	public void AddItem(string itemName, bool pauseGame=false) {
		ChangeItem(itemName, true);
		ActivateItemMessage(itemName, pauseGame);
	}

	public void RemoveItem(string itemName) {
		ChangeItem(itemName, false);
	}

	public void ChangeItem(string itemName, bool changeValue) {
		// changeValue == true means adding item
		// changeValue == false means removing item
		if (changeValue == true) {
			Debug.Log("Adding " + itemName + " to item inventory");
		} else {
			Debug.Log("Removing " + itemName + " from item inventory");
		}
		// activate the appropriate item flag
		switch (itemName) {
			case "doubleJump":
				doubleJump = changeValue;
				break;
			case "dash":
				dash = changeValue;
				break;
			case "lavaBoots":
				lavaBoots = changeValue;
				break;
			default:
				Debug.Log("itemName " + itemName + " is not a valid item name.");
				return;
				break;
		}
	}
}
