using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetails : MonoBehaviour {
    [Tooltip("What camera should this canvas face")]
    [SerializeField] private UnityEngine.Camera mainCamera;

    [Tooltip("amount of time until the health bar is hidden")]
    [SerializeField] private float hideTime = 1;
    private float hideTimer;

    // The healthbar
    private RectTransform healthBar;
    private CanvasGroup healthBarGroup;
    private float startWidth;
    private bool hpActive;


    // Use this for initialization
    void Start () {
        // if we havn't set the camera grab it
        if (mainCamera == null) mainCamera = UnityEngine.Camera.main;
        healthBar = transform.GetChild(0).GetComponent<RectTransform>();
        healthBarGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        startWidth = healthBar.localScale[0];
        HideHealthBar();
    }
	
	// Update is called once per frame
	void Update () {

        // face canvas towards the camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back,
            mainCamera.transform.rotation * Vector3.up);

        // healthbar 
        hideTimer += Time.deltaTime;
        if (hideTimer > hideTime) HideHealthBar();
    }

    public void HideHealthBar()
    {
        
        healthBarGroup.alpha = 0f;
        healthBarGroup.interactable = false;
        healthBarGroup.blocksRaycasts = false;
        hpActive = false;
    }

    public void ShowHealthBar()
    {
        healthBarGroup.alpha = 1f;
        healthBarGroup.interactable = true;
        healthBarGroup.blocksRaycasts = true;
        hpActive = true;
    }

    public void UpdateHealthBar(float newHealth, float startHealth)
    {
        Vector3 oldScale = healthBar.localScale;
        healthBar.localScale = new Vector3(startWidth * (newHealth / startHealth), oldScale[1], oldScale[2]);
        if (hpActive)
        {
            hideTimer = 0;
        }
        else
        {
            ShowHealthBar();
        }
    }
}
