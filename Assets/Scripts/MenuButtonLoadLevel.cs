using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButtonLoadLevel : MonoBehaviour {

	public void loadLevel(string leveltoLoad)
	{
		SceneManager.LoadScene (leveltoLoad);
	}
}
