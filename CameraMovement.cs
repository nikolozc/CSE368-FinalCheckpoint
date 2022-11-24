using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public int speed;

    private float step; //step is speed * Time.deltaTime
    private float minX; //minimum X camera can move to 
    private float maxX; //maximum X camera can move to
    private float minZ; //minimum Z camera can move to 
    private float maxZ; //maximum Z camera can move to
    private float minY; //minimum Y camera can move to 
    private float maxY; //maximum Y camera can move to

    // Start is called before the first frame update
    void Start()
    {
        step = speed * Time.deltaTime;
        //initialize min and max values for camera movement
        GameObject[,] gridMatrix = GetComponent<CameraRayCast>().projectManager.GetComponent<GridManager>().gridMatrix;
        GameObject firstElement = gridMatrix[0, 0];
        minX = firstElement.transform.position.x;
        maxZ = firstElement.transform.position.z;
        GameObject lastElement = gridMatrix[gridMatrix.GetUpperBound(0), gridMatrix.GetUpperBound(1)];
        maxX = lastElement.transform.position.x;
        minZ = lastElement.transform.position.z;
        minY = 5;
        maxY = 30;
    }

    // Update is called once per frame
    void Update()
    {
        //zoom out
        if (Input.GetKey("left shift") && transform.position.y <= maxY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + step, transform.position.z);
        }
        //zoom in
        if (Input.GetKey("left ctrl") && transform.position.y >= minY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - step, transform.position.z);
        }
        //move up
        if (Input.GetKey("up") && transform.position.z <= maxZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + step);
        }
        //move down
        if (Input.GetKey("down") && transform.position.z >= minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - step);
        }
        //move right
        if (Input.GetKey("right") && transform.position.x <= maxX)
        {
            transform.position = new Vector3(transform.position.x + step, transform.position.y, transform.position.z);
        }
        //move left
        if (Input.GetKey("left") && transform.position.x >= minX)
        {
            transform.position = new Vector3(transform.position.x - step, transform.position.y, transform.position.z);
        }
    }
}
