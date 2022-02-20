using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControll : MonoBehaviour
{
    public GameObject target;
    //public GameObject targetForDownArm;
    public BodySourceView2 bodysource;
    Vector3 righthandfirst;
    //Vector2 lefthandfirst;
    Vector3 righthandupdate;
    //Vector3 leftandupdate;
    Vector3 targetstartTop;
    //Vector3 targetstartDown;
    bool isfirstframe = true;

    // time change smooth
    float smoothTime = 0.3f;
    float yVelocity = 0.0f;
    private void OnEnable()
    {
        isfirstframe = true;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        targetstartTop = target.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!bodysource.istrackedone)
        {
            if (isfirstframe)
            {
                isfirstframe = false;
                if (target.tag == "Top")
                {
                    righthandfirst = bodysource.RightHand.localPosition;
                }
                else if (target.tag == "Down")
                {
                    righthandfirst = bodysource.LeftHand.localPosition;
                }
                   
                    targetstartTop = target.transform.localPosition;
                
            }
            if (target.tag == "Top")
            {
                righthandupdate.y = Mathf.SmoothDamp(righthandupdate.y, bodysource.RightHand.localPosition.y - righthandfirst.y, ref yVelocity, smoothTime); //change overtime and just change one pivot
            }
            else if (target.tag == "Down")
            {
                righthandupdate.y = Mathf.SmoothDamp(righthandupdate.y, bodysource.LeftHand.localPosition.y - righthandfirst.y, ref yVelocity, smoothTime);
            }
            
            
            
            changeposition();
        }
        else if(bodysource.istrackedone)
        {
            target.transform.localPosition = targetstartTop;
            isfirstframe = true;
        }
            
    }
    void changeposition()
    {
       // Debug.Log(bodysource.RightHand.localPosition);
        //Debug.Log(righthandfirst);
        //Debug.Log(targetstart);
       // Debug.Log(target);
       // Debug.Log(righthandupdate);

        if(target.tag == "Top")
        {
            target.transform.localPosition = targetstartTop + 2 * righthandupdate;

        }
        else if(target.tag == "Down")
        {
            target.transform.localPosition = targetstartTop - 2 * righthandupdate; 
        }
        
        
        //start from the top 
        //target.transform.localPosition = targetstart - 2 * righthandupdate; 

    }
}
