using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour {
	public static ItemInventory iv;
	public bool doubleJump = false;
	public bool dash = false;
	public bool lavaBoots = false;
	public GameObject ItemMessage;


	public void Awake() {
		if (iv == null)
			iv = this.GetComponent<ItemInventory>();

		if (ItemMessage == null)
			Debug.LogError("Need to set Item Message on Game Manager's Item Inventory");
	}

	public void ActivateItemMessage(string itemName) {
		if (ItemMessage) {
			ItemMessage.GetComponent<TextDisplay>().SetText(itemName);
			ItemMessage.SetActive(true);
		}
	}

	public void AddItem(string itemName) {
		ChangeItem(itemName, true);
		ActivateItemMessage(itemName);
	}

	public void RemoveItem(string itemName) {
		ChangeItem(itemName, false);
	}

	private void ChangeItem(string itemName, bool changeValue) {
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
