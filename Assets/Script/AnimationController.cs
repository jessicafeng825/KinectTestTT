using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator Animatorforthis;
    //public GameObject animatedhands;
    public bool isTouched = false;
    // Start is called before the first frame update
    void Start()
    {
        Animatorforthis = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationRefresh();
    }
    public void AnimationRefresh()
    {
        //Animatorforthis.SetBool("Left", isTouched);
        Animatorforthis.SetBool("Spread", isTouched);
    }

}
