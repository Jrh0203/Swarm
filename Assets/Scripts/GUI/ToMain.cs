using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToMain : MonoBehaviour {
	public Button go;
	// Use this for initialization
	void Start () {
		go = GetComponent<Button>();
		go.onClick.AddListener(OpenMain);
	}
	
	void OpenMain() {
		SceneManager.LoadScene("MainMenu");
	}
}
