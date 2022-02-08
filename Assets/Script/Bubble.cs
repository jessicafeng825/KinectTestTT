using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Vector3 mMovementDirection = Vector3.zero;
    private Coroutine mCurrentChanger = null;

    
    public BubbleManager mBubbleManager = null;

    private void OnEnable()
    {

        mCurrentChanger = StartCoroutine(DirectChanger());
    }
    private void OnDisable()
    {
        StopCoroutine(mCurrentChanger);
    }
    private void OnBecameInvisible()
    {
        //gameObject.SetActive(false);
        transform.position = mBubbleManager.GetPlanePosition();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += mMovementDirection * Time.deltaTime * 0.5f;

        transform.Rotate(Vector3.forward * Time.deltaTime * mMovementDirection.x * 20, Space.Self);

    }

    private IEnumerator DirectChanger()
    {
        while (gameObject.activeSelf)
        {
            mMovementDirection = new Vector2(Random.Range(0,100) * 0.01f,Random.Range(0,100) * 0.01f);
            yield return new WaitForSeconds(3.0f);
        }
        
    }
   
}
