using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;
public class BodySourceView2 : MonoBehaviour
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    public GameObject mJointObject;
    public GameObject AnimatedHands;

    public bool istrackedone = true;
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    public ulong bodytrackingIDfornow;
    private BodySourceManager _BodyManager;

    public Transform LeftHand, RightHand, LeftShoulder, RightShoulder, ElbowLeft, ElbowRight, SpineMid,Head;
    public Transform LeftHandFirst, RightHandFirst, LeftShoulderFirst, RightShoulderFirst, ElbowLeftFirst, ElbowRightFirst, SpineMidFirst;
    public bool isfirsttracked = true;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    private List<Kinect.JointType> _joints = new List<Kinect.JointType>
    {
        Kinect.JointType.Head,
        Kinect.JointType.ShoulderLeft,
        Kinect.JointType.ElbowLeft,
        Kinect.JointType.WristLeft,
        Kinect.JointType.HandLeft,

        Kinect.JointType.SpineMid,
        Kinect.JointType.SpineShoulder,

        Kinect.JointType.ShoulderRight,
        Kinect.JointType.ElbowRight,
        Kinect.JointType.WristRight,
        Kinect.JointType.HandRight,
    };
    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        //Get data
        Kinect.Body[] data = _BodyManager.GetData();

        if (data == null)
        {
            return;
        }


        List<ulong> trackedIds = new List<ulong>(); //tracking id value

        foreach (var body in data)//track data
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);

            }
        }

        // First delete untracked bodies
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);


        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
                istrackedone = true;
                isfirsttracked = true;
            }
        }

        // create Kinect Bodies


        foreach (var body in data)
        {

            if (body == null)
            {
                continue;
            }
            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId) && istrackedone)
                {
                    //if body isn't tracked, create body
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                    bodytrackingIDfornow = body.TrackingId;
                    istrackedone = false;
                    
                }

                //update positions
                RefreshBodyObject(body, _Bodies[bodytrackingIDfornow]);
                //check post
                checkifhaveanyAction();

            }


        }

        /*
        foreach (var body in data)
        {
            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(data[0].TrackingId))
                {
                    //create the first tracking
                    _Bodies[data[0].TrackingId] = CreateBodyObject(data[0].TrackingId);
                }
                //update positions
                RefreshBodyObject(data[0], _Bodies[data[0].TrackingId]);
                //check post
                checkifhaveanyAction();

            }

        }
*/
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        //for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        //for (Kinect.JointType jt = Kinect.JointType.Head; jt <= Kinect.JointType.HandRight; jt++)
        foreach (Kinect.JointType jt in _joints)
        {
            //not render part


            //GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject jointObj = Instantiate(mJointObject);

            //渲染line
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);

            //jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
            
            
        }

        
        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        //for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        //for (Kinect.JointType jt = Kinect.JointType.Head; jt <= Kinect.JointType.HandRight; jt++)
        foreach (Kinect.JointType jt in _joints)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            //Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            //targetPosition.z = 0; //exist on the same plane
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            //jointObj.position = targetPosition;
            
            switch (jt)
            {
                case Kinect.JointType.ShoulderLeft:
                    LeftShoulder = jointObj;
                    break;
                case Kinect.JointType.HandLeft:
                    LeftHand = jointObj;
                    break;
                case Kinect.JointType.ShoulderRight:
                    RightShoulder = jointObj;
                    break;
                case Kinect.JointType.HandRight:
                    RightHand = jointObj;
                    break;
                case Kinect.JointType.ElbowLeft:
                    ElbowLeft = jointObj;
                    break;
                case Kinect.JointType.ElbowRight:
                    ElbowRight = jointObj;
                    break;
                case Kinect.JointType.SpineMid:
                    SpineMid = jointObj;
                    break;
                case Kinect.JointType.Head:
                    SpineMid = jointObj;
                    break;
            }
            //first enter position
            //cong 立着开始
            //if(isfirsttracked == true && Head.localPosition.y < RightHand.localPosition.y && Head.localPosition.y < LeftHand.localPosition.y)
            /*
            if (isfirsttracked == true)
            {
                isfirsttracked = false;
                switch (jt)
                {
                    case Kinect.JointType.ShoulderLeft:
                        LeftShoulderFirst = jointObj;
                        break;
                    case Kinect.JointType.HandLeft:
                        LeftHandFirst = jointObj;
                        break;
                    case Kinect.JointType.ShoulderRight:
                        RightShoulderFirst = jointObj;
                        break;
                    case Kinect.JointType.HandRight:
                        RightHandFirst = jointObj;
                        break;
                    case Kinect.JointType.ElbowLeft:
                        ElbowLeftFirst = jointObj;
                        break;
                    case Kinect.JointType.ElbowRight:
                        ElbowRightFirst = jointObj;
                        break;
                    case Kinect.JointType.SpineMid:
                        SpineMidFirst = jointObj;
                        break;
                }
            }*/

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    //

    private void checkifhaveanyAction()
    {
        //if (LeftHand.localPosition.y > LeftShoulder.localPosition.y && RightHand.localPosition.y > RightShoulder.localPosition.y)
        if (LeftHand.localPosition.y > LeftShoulder.localPosition.y && RightHand.localPosition.y > RightShoulder.localPosition.y && ElbowLeft.localPosition.y > SpineMid.localPosition.y && ElbowRight.localPosition.y > SpineMid.localPosition.y)
        {
            Debug.Log("PosAnimatedHand");
            //AnimatedHands.GetComponent<AnimationController>().isTouched = true;
        }
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
