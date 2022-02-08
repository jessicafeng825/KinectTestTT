using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceView1 : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    public Material BoneMaterial;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {

        JointType.ShoulderLeft,
        JointType.ElbowLeft,
        JointType.WristLeft,
        JointType.HandLeft,
        //JointType.WristLeft,
        //JointType.ElbowLeft,
        //JointType.ShoulderLeft,
        
        JointType.ShoulderRight,
        JointType.ElbowRight,
        JointType.WristRight,
        JointType.HandRight,
    };
    private Dictionary<JointType, JointType> _BoneMap = new Dictionary<JointType, JointType>()
    {
        { JointType.FootLeft, JointType.AnkleLeft },
        { JointType.AnkleLeft, JointType.KneeLeft },
        { JointType.KneeLeft, JointType.HipLeft },
        { JointType.HipLeft, JointType.SpineBase },

        { JointType.FootRight, JointType.AnkleRight },
        { JointType.AnkleRight, JointType.KneeRight },
        { JointType.KneeRight, JointType.HipRight },
        { JointType.HipRight, JointType.SpineBase },

        { JointType.HandTipLeft, JointType.HandLeft },
        { JointType.ThumbLeft, JointType.HandLeft },
        { JointType.HandLeft, JointType.WristLeft },
        { JointType.WristLeft, JointType.ElbowLeft },
        { JointType.ElbowLeft, JointType.ShoulderLeft },
        { JointType.ShoulderLeft, JointType.SpineShoulder },

        { JointType.HandTipRight, JointType.HandRight },
        { JointType.ThumbRight, JointType.HandRight },
        { JointType.HandRight, JointType.WristRight },
        { JointType.WristRight, JointType.ElbowRight },
        { JointType.ElbowRight, JointType.ShoulderRight },
        { JointType.ShoulderRight, JointType.SpineShoulder },

        { JointType.SpineBase, JointType.SpineMid },
        { JointType.SpineMid, JointType.SpineShoulder },
        { JointType.SpineShoulder, JointType.Neck },
        { JointType.Neck, JointType.Head },
    };
    // Update is called once per frame
    void Update()
    {
        #region Get Kinect Data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
            return;

        List<ulong> trackedIds = new List<ulong>(); //list id
        foreach(var body in data)
        {
            if (body == null)
                continue;

            if (body.IsTracked)
                trackedIds.Add(body.TrackingId);
        }
        #endregion

        #region Delete Kinect bodies
        List<ulong> knowIds = new List<ulong>(mBodies.Keys);
        foreach(ulong trackingId in knowIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                //Destroy body object  
                Destroy(mBodies[trackingId]);

                // Remove from List
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect bodies
        foreach(var body in data)
        {
            if (body == null)
                continue;

            if(body.IsTracked)
            {
                // if body isn't tracked , create body
                if (!mBodies.ContainsKey(body.TrackingId))
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                
                //Update position
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Debug.Log(jt);
         }

    }
    private GameObject CreateBodyObject(ulong id)
    {
        //Create body parent
        GameObject body = new GameObject("Body:" + id);
        //Create joint
        //foreach(JointType _joint in _joints)
        for (JointType _joint = JointType.SpineBase; _joint <= JointType.ThumbRight; _joint++)
        {
            //Create Object
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.name = _joint.ToString();

            //create line 
            LineRenderer lr = newJoint.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);

            // Parent to body
            newJoint.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            newJoint.name = _joint.ToString();
            newJoint.transform.parent = body.transform;

        }
       
        return body;
    }

    private void UpdateBodyObject(Body body,GameObject bodyObject)
    {
        //Update joints
        //foreach(JointType _joint in _joints)
        for (JointType _joint = JointType.SpineBase; _joint <= JointType.ThumbRight; _joint++)
        {
            //Get new target position
            Joint sourceJoint = body.Joints[_joint];
            Joint? targetJoint = null;
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            targetPosition.z = 0; //exist on the same plane

            if (_BoneMap.ContainsKey(_joint))
            {
                targetJoint = body.Joints[_BoneMap[_joint]];
            }

            //Get joint,set new position
            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;

            LineRenderer lr = jointObject.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObject.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10,joint.Position.Y * 10,joint.Position.Z * 10);
    }

    //set color for linerender
    private static Color GetColorForState(TrackingState state)
    {
        switch (state)
        {
            case TrackingState.Tracked:
                return Color.green;

            case TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }
}
