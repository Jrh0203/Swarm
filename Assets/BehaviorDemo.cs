using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTreeSpace;

public class BehaviorDemo : MonoBehaviour {

    BehaviorTree bt;
    Selector root;
    Material m;
    [SerializeField] GameObject p;
    Color nextC;
    Color startC;


    float timer = 0;
    float maxTime = 1.0f;
    // Use this for initialization
    void Start()
    {
        bt = GetComponent<BehaviorTree>();
        m = GetComponent<Renderer>().material;
        Debug.Log(bt);
        root = bt.root;
        root.AddChild(new Condition(RadiusCheck));
        root.AddChild(new BAction(() =>
        {
            Status s = RandomColor();
            Debug.Log("CURRENT CUBE STATUS: " + s);
            return s;
        }));
        m.color = Random.ColorHSV();
        startC = m.color;
        nextC = Random.ColorHSV();
    }
        
    bool RadiusCheck()
    {
        // GameObject p = GameManager.Instance.PlayerObj.gameObject;
        float dist = Vector3.Distance(p.transform.position, transform.position);
        // Debug.Log("dist " + dist);
        bool result =  dist > 7.0;
        Debug.Log(result);
        return result;
    }

	Status RandomColor()
    {
        timer += Time.deltaTime;
        m.color = Color.Lerp(startC, nextC, timer / maxTime);
        if (timer < maxTime)
            return Status.RUNNING;
        timer = 0;
        startC = m.color;
        nextC = Random.ColorHSV();
        return Status.SUCCESS;
    }
}
