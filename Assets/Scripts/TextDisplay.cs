using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour {

	public GameObject textBackground;
	public GameObject alertTextObject;
	public GameObject descriptionTextObject;

	[System.Serializable]
	public struct BoxText {
		[TextArea(3,10)]
		public string alert;
		[TextArea(3,10)]
		public string description;

		public BoxText(string alertText, string descriptionText) {
			this.alert = alertText;
			this.description = descriptionText;
		}
	}

	public BoxText doubleJumpText;
	public BoxText dashText;
	public BoxText lavaBootsText;

	void Awake() {
		if (textBackground == null)
			Debug.LogError("Need to set text background (child)");
		if (alertTextObject == null)
			Debug.LogError("Need to set alert text (child)");
		if (descriptionTextObject == null)
			Debug.LogError("Need to set description text (child)");
	}

	public void SetText(string itemName) {
		Text alert = alertTextObject.GetComponent<Text>();
		Text description = descriptionTextObject.GetComponent<Text>();

		BoxText boxText;
		switch (itemName) {
		case ("doubleJump"):
			boxText = doubleJumpText;
			break;
		case ("dash"):
			boxText = dashText;
			break;
		case ("lavaBoots"):
			boxText = lavaBootsText;
			break;
		default:
			Debug.LogWarning("TextDisplay doesn't recognize " + itemName + " as a valid object. Cannot set text");
			return;
			break;
		}


		alert.text = boxText.alert;
		description.text = boxText.description;

	}
}
