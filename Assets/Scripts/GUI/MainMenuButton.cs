﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour {
	public Button play;
	// Use this for initialization
	void Start () {
		play = GetComponent<Button>();
		play.onClick.AddListener(OpenLevelSelect);
	}
	
	void OpenLevelSelect() {
		SceneManager.LoadScene("LevelSelect");
	}
}
