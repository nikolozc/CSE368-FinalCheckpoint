using UnityEngine;
using Michsky.UI.ModernUIPack;

public class SpaceBarPrompt : MonoBehaviour
{

    private Vector3 startObjPos; //start objects position which was set before grid animation that will be passed into resetgrid function
    private Vector3 endObjPos; //end objects position which was set before grid animation that will be passed into resetgrid function
    private NotificationManager notificationSpaceBar; //popup notification telling user to click spacebar to reset grid
    private bool foundEnd; //determines the popup description

    /* Pop up top right prompting user to press spacebar
     * initiate grid reset after user clicks spacebar
     */
    void Start()
    {
        notificationSpaceBar = GameObject.FindGameObjectWithTag("NotificationSpaceBar").GetComponent<NotificationManager>();
        if (foundEnd)
        {
            notificationSpaceBar.description = "Target was found!\nClick 'spacebar' to reset the grid";
        }
        else
        {
            notificationSpaceBar.description = "There is no path to the Target!\nClick 'spacebar' to reset the grid";
        }
        notificationSpaceBar.UpdateUI();
        notificationSpaceBar.OpenNotification();
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            notificationSpaceBar.CloseNotification();
            GetComponent<GridManager>().ResetGrid(startObjPos, endObjPos);
            Destroy(this);
        }
        
    }

    public void InitializeValues(Vector3 startPos, Vector3 endPos, bool foundEndSaved)
    {
        startObjPos = startPos;
        endObjPos = endPos;
        foundEnd = foundEndSaved;
    }
}
