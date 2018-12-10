using BehaviorTreeSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BehaviorTree : MonoBehaviour {

    [HideInInspector] public Selector root;

    // Use this for initialization
	void Awake () {
        //build tree
        BuildTree();
	}
	
	// Update is called once per frame
	void Update () {
        RunTree();
	}

    LinkedList<MonoBehaviour> GetAvalableBehaviors()
    {
        LinkedList<MonoBehaviour> result = new LinkedList<MonoBehaviour>();
        result.AddLast(GetComponent<FlockMember>());
        return result;

    }

    void RunTree()
    {
        root.Update();
        // Debug.Log("ROOT STATUS: " + root.Update());
    }

    void BuildTree()
    {
        root = new Selector();
        // root.AddChild(new Condition(() => { return false; } ));
        // root.AddChild(new BAction(() => { Debug.Log("HERE!"); return Status.SUCCESS; }));
    }
    /*
    void ChooseBehavior()
    {
        if (c1 || c2)
        {
            if (c1)
            {
                b1;
            }
            else
            {
                b2
            }
        }
        else
        {
            if (c3 || c4)
            {
                if (c3)
                {
                    b3
                } 
                else
                {
                    b4
                }
            }
            else
            {
                b5
            }
        }
    }
    */
}
