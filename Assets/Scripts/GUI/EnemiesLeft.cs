using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesLeft : MonoBehaviour {
    private float startWidth;
    private float smoothSpeed = .075f;
    Text words;
    void Start () {
        startWidth = transform.localScale[0];
        words = GetComponent<Text>();
    }

	void Update() {
        int remain = GameManager.Instance.EnemiesLeft;
        words.text = "Enemies Left : "+remain;
    }
}
