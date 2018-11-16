using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>{
	public bool walkable;
	public bool isCover;
	public Vector3 worldPosition;

	//distance from start
	public int gCost;
	//distance from goal
	public int hCost;
	public int gridX;
	public int gridY;
	public Node parent;
	int heapIndex;
	public Node(bool pWalkable, Vector3 pWorldPosition, int pGridX, int pGridY) {
		walkable = pWalkable;
		worldPosition = pWorldPosition;
		gridX = pGridX;
		gridY = pGridY;
		isCover = false;
	}	

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if(compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
