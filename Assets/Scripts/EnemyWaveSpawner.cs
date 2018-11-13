using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour {

    [Tooltip("Time in secounds between wave spawns")]
    [SerializeField] private float spawnRate = 3;

    [Tooltip("number of enemys spawned per waves")]
    [SerializeField] private int waveSize = 3;

    [Tooltip("enemy to spawn")]
    [SerializeField] private GameObject enemy;

    [HideInInspector] public bool spawnEnabled;

    // Use this for initialization
    void Awake () {
        spawnEnabled = true;
        StartCoroutine("SpawnWave");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SpawnWave()
    {
        while (true) {
            if (spawnEnabled) { 
                for (int i = -1 *waveSize/2; i < waveSize + (waveSize & 0x1); i++) {
                    Instantiate(enemy, transform.position + new Vector3(i * 10, 0, 0), transform.rotation);
                }
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }
}
