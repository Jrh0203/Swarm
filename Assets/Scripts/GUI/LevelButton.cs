using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour {
	public int levelNumber = 0;
	public Button level;
	// Use this for initialization
	void Start () {
		level = GetComponent<Button>();
		level.onClick.AddListener(OpenLevel);
	}
	
	void OpenLevel() {
		string levelString = "Level" + levelNumber;
		SceneManager.LoadScene(levelString);
	}
}
