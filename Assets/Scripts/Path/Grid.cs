using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Grid : MonoBehaviour {
    private int inSightPenalty = 1000;
    public Vector2 gridWorldSize;
    public LayerMask unwalkable;
    public float nodeRadius;
    Node[,] grid;

    public bool drawGizmos;

    int gridWidth;
    int gridHeight;
    void Awake() {
        gridWidth = Mathf.RoundToInt(gridWorldSize.x / (nodeRadius * 2));
        gridHeight = Mathf.RoundToInt(gridWorldSize.y / (nodeRadius * 2));
        CreateGrid();
    }

    public void UpdateCover() {
        for(int x = 0; x < gridWidth; x++) {
            for(int y = 0; y < gridHeight; y++) {
            	grid[x,y].isCircle = false;
                grid[x,y].capacity = 0;
                Vector3 toPosition = WorldFromNodeXY(x,y);
                Vector3 fromPosition = GameManager.Instance.PlayerObj.transform.position;
                if(Vector3.Distance(toPosition, fromPosition) < GameManager.Instance.PlayerObj.Range) {
                    RaycastHit hit;
                    Vector3 direction = toPosition - fromPosition;
                    int layer_mask = LayerMask.GetMask("Wall");
                    
                    if (Physics.Raycast (fromPosition, direction, out hit, direction.magnitude,layer_mask)) {
                        grid[x,y].isCover = true;
                    } else {
                        grid[x,y].isCover = false;
                    }
                } else {
                    grid[x,y].isCover = true;
                }
            }
        }
    }

    public List<Node> UpdateBattleCircle(){
    	List<Node> spots = new List<Node>();

    	Vector3 direction = new Vector3(1,0,0);
    	direction = direction * GameManager.Instance.PlayerObj.Range;
    	int layer_mask = LayerMask.GetMask("Wall");
    	Vector3 fromPosition = GameManager.Instance.PlayerObj.transform.position;
    	int inc = 3;
    	for (int deg = 0; deg < 360; deg += inc){
    		Quaternion rotation = Quaternion.Euler(0,inc,0);
			Vector3 myVector = Vector3.one;
			direction = rotation * direction;
			RaycastHit hit;
			Node gridNode = NodeFromWorldPos(fromPosition+direction);
			if (!Physics.Raycast (fromPosition, direction, out hit, direction.magnitude,layer_mask)) {
                gridNode.isCircle = true;
                spots.Add(gridNode);
            }
    	}
        return spots;
    }
    public int MaxSize {
        get {
            return gridWidth * gridHeight;
        }
    }
    void CreateGrid() {
        grid = new Node[gridWidth,gridHeight];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
        for(int x = 0; x < gridWidth; x++) {
            for(int y = 0; y < gridHeight; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeRadius * 2 + nodeRadius) + Vector3.forward * (y * nodeRadius * 2 + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkable));
                grid[x,y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    //pass in vector of world position relative to grid
    public Node NodeFromWorldPos(Vector3 worldPosition) {
        int xPos = Mathf.Clamp(Mathf.FloorToInt((worldPosition.x + gridWorldSize.x/2) / (nodeRadius * 2)), 0, gridWidth - 1);
        int yPos = Mathf.Clamp(Mathf.FloorToInt((worldPosition.z + gridWorldSize.y/2) / (nodeRadius * 2)), 0, gridHeight - 1); 
        return grid[xPos, yPos];
    }

    public Vector3 WorldFromNodeXY(int x, int y){
        float width = nodeRadius*2;
        var outX = x*width+nodeRadius - gridWorldSize.x/2;
        var outY = y*width+nodeRadius - gridWorldSize.y/2;
        return new Vector3(outX,.5f,outY);
    }

    public List<Node> GetNeighbors(Node n) {
        List<Node> neighbors = new List<Node>();
        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0) {
                    continue;
                }
                int checkX = n.gridX + x;
                int checkY = n.gridY + y;
           
                if(checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight) {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    public void FindPath(PathRequest request, Action<PathResult> callback) {
		bool foundPath = false;
		
        Vector3[] waypoints = new Vector3[0];
		Node startNode = NodeFromWorldPos(request.startPos);
		Node endNode = NodeFromWorldPos(request.endPos);
		
		Heap<Node> openSet = new Heap<Node>(MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while(openSet.Count > 0) {
			Node min = openSet.RemoveFirst();
			closedSet.Add(min);

			if(min == endNode) {
				foundPath = true;
				break;
			}

			List<Node> neighbors = GetNeighbors(min);
			foreach(Node neighbor in neighbors) {
				if(closedSet.Contains(neighbor) || !neighbor.walkable) {
					continue;
				}
				int newDistance = min.gCost + GetDistance(min, neighbor) + ((!neighbor.isCover) ? inSightPenalty : 0);
				if(neighbor.gCost > newDistance || !openSet.Contains(neighbor)) {
					neighbor.gCost = newDistance;
					neighbor.hCost = GetDistance(neighbor, endNode);
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
			waypoints = RetracePath(startNode, endNode);
            foundPath = waypoints.Length > 0;
		}
		callback.Invoke(new PathResult(waypoints, foundPath, request.callback));
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

    void OnDrawGizmos() {
        if(drawGizmos && GameManager.Instance != null) {
            Node playerNode = NodeFromWorldPos(GameManager.Instance.PlayerObj.transform.position);
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
            if(grid != null) {
                for(int x = 0; x < gridWidth; x++) {
                    for(int y = 0; y < gridHeight; y++) {
                        Gizmos.color = (grid[x,y].walkable) ? Color.white : Color.red;
                        if (!grid[x,y].walkable){
                            Gizmos.color = Color.red;
                        }
                        if(grid[x,y].isCover) {
                            Gizmos.color = Color.blue;
                        }
                        if(playerNode == grid[x,y]) {
                            Gizmos.color = Color.cyan;
                        }
                        if (grid[x,y].isCircle){
                        	Gizmos.color = Color.green;
                        }
                        Gizmos.DrawCube(grid[x  ,y].worldPosition, Vector3.one * (nodeRadius * 2 - .1f)); 
                    }
                }
            }
        }
    }
}
