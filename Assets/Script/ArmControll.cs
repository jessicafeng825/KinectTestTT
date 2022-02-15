using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControll : MonoBehaviour
{
    public GameObject target;
    public BodySourceView2 bodysource;
    Vector3 righthandfirst;
    Vector3 righthandupdate;
    Vector3 targetstart;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!bodysource.istrackedone)
        {
            if (isfirstframe)
            {
                isfirstframe = false;
                righthandfirst = bodysource.RightHand.localPosition;
                targetstart = target.transform.localPosition;
            }
            righthandupdate.y = Mathf.SmoothDamp(righthandupdate.y,bodysource.RightHand.localPosition.y - righthandfirst.y,ref yVelocity, smoothTime); //change overtime and just change one pivot
            changeposition();
        }
            
    }
    void changeposition()
    {
        Debug.Log(bodysource.RightHand.localPosition);
        //Debug.Log(righthandfirst);
        //Debug.Log(targetstart);
        Debug.Log(target);
        Debug.Log(righthandupdate);
        
        target.transform.localPosition = targetstart + 2 * righthandupdate;
        
        //start from the top 
        //target.transform.localPosition = targetstart - 2 * righthandupdate; 

    }
}
