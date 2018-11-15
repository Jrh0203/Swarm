using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public Vector2 gridWorldSize;
    public LayerMask unwalkable;
    public float nodeRadius;
    Node[,] grid;

    int gridWidth;
    int gridHeight;
    void Awake() {
        gridWidth = Mathf.RoundToInt(gridWorldSize.x / (nodeRadius * 2));
        gridHeight = Mathf.RoundToInt(gridWorldSize.y / (nodeRadius * 2));
        CreateGrid();
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
}
