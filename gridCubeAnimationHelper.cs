using UnityEngine;

public class gridCubeAnimationHelper : MonoBehaviour
{
    public GameObject projectManager;

    private Color cubeColor; //Color that cube will Lerp its color into
    private float animationSpeed; //passed in float from GridAnimationManager that determines the speed in which cube should animate
    private bool flag = false; //flag for cube reaching its target to move back down
    private bool isHalfDone = false; //flag for cube coming back down to its original position - used to disable the script
    private bool isFinished = false; //flag for cube finishing the moveTowards - this + check if color fully changed = destory script
    private bool isPath = false; //isPath bool taken from "GridAnimationManager" to determine color to choose for cube
    private Vector3 pos; //position set before moving the object on the Y axis - to be able to come back to its original position

    // Start is called before the first frame update
    void Start()
    {
        projectManager = GameObject.FindWithTag("ProjectManager");
        isPath = projectManager.GetComponent<GridAnimationManager>().isPath;
        cubeColor = isPath ? projectManager.GetComponent<GridAnimationManager>().pathColor : projectManager.GetComponent<GridAnimationManager>().cubeColor;
        animationSpeed = projectManager.GetComponent<GridAnimationManager>().animationSpeed;
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //lerps from current color to specified 'cubeColor' speed of 1/8*lerpSpeed
        GetComponent<MeshRenderer>().material.color = Color.Lerp(
            GetComponent<MeshRenderer>().material.color, cubeColor, Mathf.PingPong(Time.time, 1/(8*animationSpeed)));
        //move the cube up speed of 2/lerpSpeed
        float step = (2/animationSpeed) * Time.deltaTime;
        Vector3 target = new Vector3(pos.x, pos.y + 1, pos.z);

        /* check if it reached target if true set flag to true
         * also set isHalfDone to true to stop MoveTowards
         */
        if (Vector3.Distance(transform.position, target) < 0.005f)
        {
            flag = true;
            isHalfDone = true;
        }
        if (isHalfDone && Vector3.Distance(transform.position, pos) < 0.005f)
        {
            step = 0;
            isFinished = true;
        }
        //keep moving it up until it reaches target then move it back down
        if (!flag)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, step);
        }

        //if movement is finished check if color Lerp finished if true turn emission on and destory self
        if(isFinished && GetComponent<MeshRenderer>().material.color == cubeColor)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", cubeColor * (isPath ? 4 : 2));
            Destroy(this);
        }
    }
}
