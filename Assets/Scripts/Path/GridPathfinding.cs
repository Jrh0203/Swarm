using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GridPathfinding : MonoBehaviour {
	DisplayedGrid grid;

	void Awake() {
		grid = GetComponent<DisplayedGrid>();
	}

    void Update() {
		if(Input.GetButtonDown("Jump")) {
			FindPath(GameObject.FindWithTag("Player").transform.position, GameObject.FindWithTag("Enemy").transform.position);
		}
    }

	public void StartFindPath(Vector3 startPos, Vector3 goalPos) {

	}
	void FindPath(Vector3 startPos, Vector3 goalPos) {
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		Node startNode = grid.NodeFromWorldPos(startPos);
		Node goalNode = grid.NodeFromWorldPos(goalPos);
		//Debug.Log(goalNode.gridX + " " +  goalNode.gridY);
		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while(openSet.Count > 0) {
			Node min = openSet.RemoveFirst();
			closedSet.Add(min);

			if(min == goalNode) {
				grid.path = RetracePath(startNode, goalNode);
				sw.Stop();
				print("Path found: " + sw.ElapsedMilliseconds + " ms");
				return;
			}

			List<Node> neighbors = grid.GetNeighbors(min);
			foreach(Node neighbor in neighbors) {
				if(closedSet.Contains(neighbor) || !neighbor.walkable) {
					continue;
				}
				int newDistance = min.gCost + GetDistance(min, neighbor);
				if(neighbor.gCost > newDistance || !openSet.Contains(neighbor)) {
					neighbor.gCost = newDistance;
					neighbor.hCost = GetDistance(neighbor, goalNode);
					neighbor.parent = min;
					
					if(!openSet.Contains(neighbor)) {
						openSet.Add(neighbor);
					} else {
						openSet.UpdateItem(neighbor);
					}
				}
			}	
		}
	}

	List<Node> RetracePath(Node start, Node end) {
		List<Node> path = new List<Node>();

		Node current = end;
		while(current != start) {
			path.Add(current);
			current = current.parent;	
		}
		path.Add(start);
		path.Reverse();
		return path;
	}


	int GetDistance(Node a, Node b) {
		int distX = Mathf.Abs(a.gridX - b.gridX);
		int distY = Mathf.Abs(a.gridY - b.gridY);

		if(distX > distY) {
			return 14 * distY + 10 * (distX - distY);
		} else {
			return 14 * distX + 10 * (distY - distX);
		}
	}
}
