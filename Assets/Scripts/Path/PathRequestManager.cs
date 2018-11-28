using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class PathRequestManager: MonoBehaviour {
    Queue<PathResult> results = new Queue<PathResult>();

    static PathRequestManager instance;
    
    void Awake() {
        instance = this;
    }

    void Update() {
        if(results.Count > 0) {
            print("results:" + results.Count);
            int count = results.Count;
            lock(results) {
                for(int i = 0; i < count; i++) {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        } 
    }

    public static void RequestPath(PathRequest request) {
        ThreadStart threadStart = delegate {
            GameManager.Instance.GridObj.FindPath(request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult result) {
        //only one thread enqueues at a time
        lock(results) {
            results.Enqueue(result);
        }
    }
}

public struct PathResult {
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback) {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

//container for data about a path request
public struct PathRequest {
    public Vector3 startPos;
    public Vector3 endPos;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 startPos, Vector3 endPos, Action<Vector3[], bool> callback) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.callback = callback;
    }
}