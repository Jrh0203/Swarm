using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
	Grid grid;

	void Awake() {
		grid = GetComponent<Grid>();
	}

	public Vector3[] FindPath(Vector3 startPos, Vector3 goalPos) {
		bool foundPath = false;
		
		Node startNode = grid.NodeFromWorldPos(startPos);
		Node goalNode = grid.NodeFromWorldPos(goalPos);
		
		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while(openSet.Count > 0) {
			Node min = openSet.RemoveFirst();
			closedSet.Add(min);

			if(min == goalNode) {
				foundPath = true;
				break;
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
		if(foundPath) {
			return RetracePath(startNode, goalNode);;
		}
		return null;
	}

	
	Vector3[] RetracePath(Node start, Node end) {
		List<Vector3> path = new List<Vector3>();

		Node current = end;
		while(current != start) {
			path.Add(current.worldPosition);
			current = current.parent;
		}
		path.Reverse();
		return path.ToArray();
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
