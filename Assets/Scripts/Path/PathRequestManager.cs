using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathRequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequests = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	Pathfinding pathfinding;
	bool isFinding;

	void Awake() {
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
		PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
		instance.pathRequests.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	void TryProcessNext() {
		if(!isFinding && pathRequests.Count > 0) {
			currentPathRequest = pathRequests.Dequeue();
			isFinding = true;
			//pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback(path, success);
		isFinding = false;
		TryProcessNext();
	}
	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 pStart, Vector3 pEnd, Action<Vector3[], bool> pCallback) {
			pathStart = pStart;
			pathEnd = pEnd;
			callback = pCallback;
		}
	}
}
