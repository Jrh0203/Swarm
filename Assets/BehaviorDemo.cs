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


    float timer = 0;
    float maxTime = 10.0f;
    // Use this for initialization
    void Start () {
        bt = GetComponent<BehaviorTree>();
        m = GetComponent<Renderer>().material;
        Debug.Log(bt);
        root = bt.root;
        root.AddChild(new Condition(RadiusCheck));
        root.AddChild(new BAction(RandomColor));
        m.color = Random.ColorHSV();
        nextC = Random.ColorHSV();

    }
    bool RadiusCheck()
    {
        // GameObject p = GameManager.Instance.PlayerObj.gameObject;
        float dist = Mathf.Abs((p.transform.position - transform.position).magnitude);
        // Debug.Log("dist " + dist);
        return dist > 10.0;
    }

	Status RandomColor()
    {
        timer += Time.deltaTime;
        m.color = Color.Lerp(m.color, nextC, timer / maxTime);
        if (timer < maxTime)
            return Status.RUNNING;
        Debug.Log("WE DID IT BOIS");
        timer = 0;
        nextC = Random.ColorHSV();
        return Status.SUCCESS;
    }
}
