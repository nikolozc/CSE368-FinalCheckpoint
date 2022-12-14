using System.Collections.Generic;
using UnityEngine;

public class GridAlgorithms : MonoBehaviour
{
    public GameObject[,] gridMatrix; //matrix of GameObject (gridCubes) 
    public GameObject[,] gridTopMatrix; //matrix of GameObject (top of gridMatrix - startobject,endobject,walls,and weights)
    public int[] startIndex; //array holding start objects position [row,col]
    public int[] endIndex; //array holding end objects position [row,col]

    private LinkedList<int[]> nextToVisit; //holds position [row,col] of gameobject that will be visited next in the algorithm
    private LinkedList<GameObject> objectsToAnimate; //list of gameobjects that will be animated 
    private LinkedList<GameObject> path; //path from start to finish that will be animated after objectsToAnimate - if end is found
    private bool foundEnd;

    //initializes all values and starts DFS/BFS based on the passed in string
    public void Initialize(string algo)
    {
        //initialize values
        gridMatrix = GetComponent<GridManager>().gridMatrix;
        gridTopMatrix = GetComponent<GridManager>().gridTopMatrix;
        startIndex = GetComponent<GridManager>().startIndex;
        endIndex = GetComponent<GridManager>().endIndex;
        nextToVisit = new LinkedList<int[]>();
        objectsToAnimate = new LinkedList<GameObject>();
        path = new LinkedList<GameObject>();
        foundEnd = false;

        //add start node to nextToVisit before running algorithm
        nextToVisit.AddFirst(startIndex);
        //set first elements distance to 0 for Weighted algorithms
        gridMatrix[nextToVisit.First.Value[0], nextToVisit.First.Value[1]].GetComponent<gridCubeNode>().distance = 0;
        //set first elements AstarDistance to distanceToEndNode for A* algorithm
        gridMatrix[nextToVisit.First.Value[0], nextToVisit.First.Value[1]].GetComponent<gridCubeNode>().AStarDistance =
                gridMatrix[nextToVisit.First.Value[0], nextToVisit.First.Value[1]].GetComponent<gridCubeNode>().distanceToEndNode;
        AlgorithmLoop(algo);
    }

    private void AlgorithmLoop(string algoirthmToUse)
    {
        //loop while there are more nodes to visit
        while (nextToVisit.Count != 0)
        {
            /* get the first cube that coresponds to index values contained in first node of nextToVisit 
             * then remove the first node from nextToVisit
             */
            int[] currentIndex = nextToVisit.First.Value;
            GameObject node = gridMatrix[nextToVisit.First.Value[0], nextToVisit.First.Value[1]];
            nextToVisit.RemoveFirst();
            /* if not BFS then check to see if this node has already been visited - this is because when we get neighbors for DFS
             * we are not checking to see if a node is already in the nextToVisit 
             * if true then skip this iteration of the while loop
             */
            if (algoirthmToUse != "BFS" && node.GetComponent<gridCubeNode>().visited == true)
            {
                continue;
            }
            //set this node to visited
            node.GetComponent<gridCubeNode>().visited = true;
            //check to see if the current node is wall node if true break this iteration of the while loop
            if (gridTopMatrix[currentIndex[0], currentIndex[1]] != null &&
                    gridTopMatrix[currentIndex[0], currentIndex[1]].name[1] == 'a')
            {
                continue;
            }
            //add node to objectsToAnimate
            objectsToAnimate.AddLast(node);

            //check to see if the current node is end node
            if (gridTopMatrix[currentIndex[0], currentIndex[1]] != null &&
                    gridTopMatrix[currentIndex[0], currentIndex[1]].name[0] == 'E')
            {
                //rebuild path from start to end, start animation with parameter, and set foundEnd to true
                RebuildPath(node);
                foundEnd = true;
                GetComponent<GridAnimationManager>().AnimateGrid(objectsToAnimate, foundEnd, path);
                break;
            }
            //add this nodes neighbors to nextToVisit list.
            if (algoirthmToUse == "BFS")
            {
                GetNeighborsBFS(currentIndex);
            }
            else if (algoirthmToUse == "DFS")
            {
                GetNeighborsDFS(currentIndex);
            }
            else if (algoirthmToUse == "Dijkstra")
            {
                GetNeighborsDijkstra(currentIndex);
            }
            else if (algoirthmToUse == "AStar")
            {
                GetNeighborsAStar(currentIndex);
            }
        }
        //if End object was not found start animation with parameter foundEnd set to false
        if (!foundEnd)
        {
            GetComponent<GridAnimationManager>().AnimateGrid(objectsToAnimate, foundEnd);
        }
    }

    /* gets nodes neighbors that are up,right,down,left that have not been visited yet
     *  and adds them to the last of nextToVisit list then the list needs to be sorted based on distance to endNode+Weight
     */
    private void GetNeighborsAStar(int[] nodeIndex)
    {
        /* [row, col]
         * check if there is node on top of the given node and its not yet been visited
         * if given node distance  < top node distance then
         * add it to end of nextToVisit and make that nodes previous node node at gridMatrix[nodeIndex]
         */
        float nodeIndexDistance = gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().AStarDistance;

        int[] arrayToAdd = new int[] { nodeIndex[0] - 1, nodeIndex[1] };
        if (nodeIndex[0] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().AStarDistance > nodeIndexDistance)
        {
            GetNeighborsAStarHelper(arrayToAdd, nodeIndex);
        }

        //do same thing but right node
        arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] + 1 };
        if (nodeIndex[1] + 1 <= gridMatrix.GetUpperBound(1) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().AStarDistance > nodeIndexDistance)
        {
            GetNeighborsAStarHelper(arrayToAdd, nodeIndex);
        }

        //do same thing but below node
        arrayToAdd = new int[] { nodeIndex[0] + 1, nodeIndex[1] };
        if (nodeIndex[0] + 1 <= gridMatrix.GetUpperBound(0) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().AStarDistance > nodeIndexDistance)
        {
            GetNeighborsAStarHelper(arrayToAdd, nodeIndex);
        }

        //do same thing but left node
        arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] - 1 };
        if (nodeIndex[1] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().AStarDistance > nodeIndexDistance)
        {
            GetNeighborsAStarHelper(arrayToAdd, nodeIndex);
        }
    }

    /* gets nodes neighbors that are up,right,down,left that have not been visited yet
     *  and adds them to the last of nextToVisit list then the list needs to be sorted based on distance value
     */
    private void GetNeighborsDijkstra(int[] nodeIndex)
    {
        /* [row, col]
         * check if there is node on top of the given node and its not yet been visited
         * if given node distance < top node distance then
         * add it to end of nextToVisit and make that nodes previous node node at gridMatrix[nodeIndex]
         */

        int nodeIndexDistance = gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().distance;

        int[] arrayToAdd = new int[] { nodeIndex[0] - 1, nodeIndex[1] };
        if (nodeIndex[0] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance > nodeIndexDistance)
        {
            GetNeighborsDijkstraHelper(arrayToAdd, nodeIndex);
        }

        //do same thing but right node
        arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] + 1 };
        if (nodeIndex[1] + 1 <= gridMatrix.GetUpperBound(1) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance > nodeIndexDistance)
        {
            GetNeighborsDijkstraHelper(arrayToAdd, nodeIndex);
        }

        //do same thing but below node
        arrayToAdd = new int[] { nodeIndex[0] + 1, nodeIndex[1] };
        if (nodeIndex[0] + 1 <= gridMatrix.GetUpperBound(0) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance > nodeIndexDistance)
        {
            GetNeighborsDijkstraHelper(arrayToAdd, nodeIndex);
        }

        //do same thing but left node
        arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] - 1 };
        if (nodeIndex[1] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
             gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance > nodeIndexDistance)
        {
            GetNeighborsDijkstraHelper(arrayToAdd, nodeIndex);
        }
    }

    /* gets nodes neighbors that are left,down,right,up that have not been visited yet
         *  and adds them to the begining of nextToVisit list
         */
    private void GetNeighborsDFS(int[] nodeIndex)
    {
        /* [row, col]
         * if there is node to the left and its not been visited and if left node is not already in nextToVisit list
         * add it to beginging of nextToVisit and make that nodes previous node node at gridMatrix[nodeIndex]
         */
        int[] arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] - 1 };
        if (nodeIndex[1] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddFirst(arrayToAdd);
        }
        //do same thing but below node
        arrayToAdd = new int[] { nodeIndex[0] + 1, nodeIndex[1] };
        if (nodeIndex[0] + 1 <= gridMatrix.GetUpperBound(0) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddFirst(arrayToAdd);
        }
        //do same thing but right node
        arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] + 1 };
        if (nodeIndex[1] + 1 <= gridMatrix.GetUpperBound(1) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddFirst(arrayToAdd);
        }
        //do same thing but top node
        arrayToAdd = new int[] { nodeIndex[0] - 1, nodeIndex[1] };
        if (nodeIndex[0] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddFirst(arrayToAdd);
        }
    }

    /* gets nodes neighbors that are up,right,down,left that have not been visited yet
     *  and adds them to the end of nextToVisit list
     */
    private void GetNeighborsBFS(int[] nodeIndex)
    {
        /* [row, col]
         * check if there is node on top of the given node and 
         * if top node has already been visitred or not and if top node is not already in nextToVisit list
         * add it to end of nextToVisit and make that nodes previous node node at gridMatrix[nodeIndex]
         */
        int[] arrayToAdd = new int[] { nodeIndex[0] - 1, nodeIndex[1] };
        if (nodeIndex[0] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
            !CustomContains(arrayToAdd))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddLast(arrayToAdd);
        }

        //do same thing but right node
        arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] + 1 };
        if (nodeIndex[1] + 1 <= gridMatrix.GetUpperBound(1) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
            !CustomContains(arrayToAdd))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddLast(arrayToAdd);
        }

        //do same thing but below node
        arrayToAdd = new int[] { nodeIndex[0] + 1, nodeIndex[1] };
        if (nodeIndex[0] + 1 <= gridMatrix.GetUpperBound(0) &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
            !CustomContains(arrayToAdd))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddLast(arrayToAdd);
        }

        //do same thing but left node
        arrayToAdd = new int[] { nodeIndex[0], nodeIndex[1] - 1 };
        if (nodeIndex[1] - 1 >= 0 &&
            !(gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().visited) &&
            !CustomContains(arrayToAdd))
        {
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
            nextToVisit.AddLast(arrayToAdd);
        }
    }

    /*                  BFS
     * Try to find a better solution worst case is O(2n)
     * there is no next function in c# so I used copyto + traversing throug the copied array - O(n) + O(n) 
     * and im calling it 4 times each time I call GetNeighbors
     * 
     * Itterates through the linked list and checks if it has int[] with same values at int[0] and int[1]
     * if bool needToRemove is true then remove that element from the nextToVisit list
     */
    private bool CustomContains(int[] arr)
    {
        int[][] copiedArray = new int[nextToVisit.Count][];
        nextToVisit.CopyTo(copiedArray, 0);
        for (int i = 0; i < copiedArray.Length; i++)
        {
            if (arr[0] == copiedArray[i][0] && arr[1] == copiedArray[i][1])
            {
                return true;
            }
        }
        return false;
    }

    /*                      Weighted Algorithm
     * Itterate though linked list and move elements into new list
     * if arrToRemove elements match elements in any of nextToVisit lists elements then
     * leave that element out from the new list, then let nextToVisit list = the new list
     */
    private void RemoveDuplicate(int[] arrToRemove)
    {
        LinkedList<int[]> newNextToVisit = new LinkedList<int[]>();
        int count = nextToVisit.Count;
        for (int i = 0; i < count; i++)
        {
            int[] temp = nextToVisit.First.Value;
            //if there is arrToRemove in the list skip adding it else add temp to the new list
            if (temp[0] == arrToRemove[0] && temp[1] == arrToRemove[1])
            {
                nextToVisit.RemoveFirst();
                continue;
            }
            newNextToVisit.AddLast(temp);
            nextToVisit.RemoveFirst();
        }
        nextToVisit = newNextToVisit;
    }

    /*                      Weighted Algorithm
     * Itterate though linked list and move elements into new list
     * if arrToAdd.distance < nextToVisit(element at i).distance then
     * add arrToAdd before that element and copy the rest of the array
     * if bool AStar is true make comparison with AStarDistance else distance
     */
    private void AddNode(int[] arrToAdd, bool AStar)
    {
        LinkedList<int[]> newNextToVisit = new LinkedList<int[]>();
        int count = nextToVisit.Count;
        bool added = false; //checks to see if arrToAdd was added in between the list. if false after for loop then add arrToAdd at the end of list
        for (int i = 0; i < count; i++)
        {
            int[] temp = nextToVisit.First.Value;
            //if gridMatrix[arrToAdd].distance < gridMatrix[temp] then add arrToAdd then temp
            if (!added && !AStar && gridMatrix[temp[0], temp[1]].GetComponent<gridCubeNode>().distance > gridMatrix[arrToAdd[0], arrToAdd[1]].GetComponent<gridCubeNode>().distance)
            {
                newNextToVisit.AddLast(arrToAdd);
                added = true;
            }
            else if(!added && AStar && gridMatrix[temp[0], temp[1]].GetComponent<gridCubeNode>().AStarDistance > gridMatrix[arrToAdd[0], arrToAdd[1]].GetComponent<gridCubeNode>().AStarDistance)
            {
                newNextToVisit.AddLast(arrToAdd);
                added = true;
            }
            newNextToVisit.AddLast(temp);
            nextToVisit.RemoveFirst();
        }
        if (!added)
        {
            newNextToVisit.AddLast(arrToAdd);
        }
        nextToVisit = newNextToVisit;
    }

    /*                 AStar
     * helper method for AStar algorithm - exactly the same as DijkstraHelper except use AStarDistance instead of distance
     * code that repeats in every node (top, right, bottom, left) if the if statment passes do the following:
     */
    private void GetNeighborsAStarHelper(int[] arrayToAdd, int[] nodeIndex)
    {
        //check to see if new distance that will be applied to the node is actually less than the distance it currently has
        //check if there is a weight on top
        float newDistance;
        if (gridTopMatrix[arrayToAdd[0], arrayToAdd[1]] != null && gridTopMatrix[arrayToAdd[0], arrayToAdd[1]].name[1] == 'e')
        {
            newDistance = gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().AStarDistance + 10;
        }
        else
        {
            newDistance = gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().AStarDistance + 1;
        }

        //if new distance is less than what it had then update the node else return back 
        if (newDistance >= gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().AStarDistance)
        {
            return;
        }
        gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
        //set the updated distance of the node
        //if there is weight ontop of arrayToAdd then distance = nodeIndex.distance+10 else nodeIndex.distance+1
        gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance =
            (gridTopMatrix[arrayToAdd[0], arrayToAdd[1]] != null && gridTopMatrix[arrayToAdd[0], arrayToAdd[1]].name[1] == 'e') ?
            gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().distance + 10
            : gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().distance + 1;
        //set AStarDistance for the node
        gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().AStarDistance =
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance +
            gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distanceToEndNode;
        //check if top node was already in the nextToVisit list if true remove it
        RemoveDuplicate(arrayToAdd);
        AddNode(arrayToAdd, true);
    }

    /*                 Dijkstra
     * helper method for Dijkstra algorithm 
     * code that repeats in every node (top, right, bottom, left) if the if statment passes do the following:
     */
    private void GetNeighborsDijkstraHelper(int[] arrayToAdd, int[] nodeIndex)
    {
        //check to see if new distance that will be applied to the node is actually less than the distance it currently has
        //check if there is a weight on top
        int newDistance;
        if (gridTopMatrix[arrayToAdd[0], arrayToAdd[1]] != null && gridTopMatrix[arrayToAdd[0], arrayToAdd[1]].name[1] == 'e')
        {
            newDistance = gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().distance + 10;
        }
        else
        {
            newDistance = gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().distance + 1;
        }

        //if new distance is less than what it had then update the node else return back 
        if (newDistance >= gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance)
        {
            return;
        }
        gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().previousNode = gridMatrix[nodeIndex[0], nodeIndex[1]];
        //set the updated distance of the node
        //if there is weight ontop of arrayToAdd then distance = nodeIndex.distance+10 else nodeIndex.distance+1
        gridMatrix[arrayToAdd[0], arrayToAdd[1]].GetComponent<gridCubeNode>().distance =
            (gridTopMatrix[arrayToAdd[0], arrayToAdd[1]] != null && gridTopMatrix[arrayToAdd[0], arrayToAdd[1]].name[1] == 'e') ?
            gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().distance + 10
            : gridMatrix[nodeIndex[0], nodeIndex[1]].GetComponent<gridCubeNode>().distance + 1;
        //check if top node was already in the nextToVisit list if true remove it
        RemoveDuplicate(arrayToAdd);
        AddNode(arrayToAdd, false);
    }


    /* accepts a GameObject 'node' as a parameter and follows that nodes previousNode path 
     * until it reaches the start node - previousNode = null
     * fills LinkedList 'path' with nodes to later be animated
     */
    private void RebuildPath(GameObject node)
    {
        while (node != null)
        {
            path.AddFirst(node);
            node = node.GetComponent<gridCubeNode>().previousNode;
        }
    }
}