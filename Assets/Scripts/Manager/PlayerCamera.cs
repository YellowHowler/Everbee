using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    private Camera mCamera;
    private float mScrollSpeed = 3.5f;
    private float mScrollBound = 0.98f;

    private Transform mFollowTarget;
    private bool mIsFollowing = false;

    void Awake()
    {
        Instance = this;
        mCamera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(mIsFollowing == true)
        {
            transform.position = Mng.play.SetZ(mFollowTarget.position, transform.position.z);
        }

        if (Input.mousePosition.x >= Screen.width * mScrollBound)
        {
            mCamera.transform.Translate(Vector3.right * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }
        else if (Input.mousePosition.x <= Screen.width * (1 - mScrollBound))
        {
            mCamera.transform.Translate(Vector3.left * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }
        if (Input.mousePosition.y >= Screen.height * mScrollBound)
        {
            mCamera.transform.Translate(Vector3.up * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }
        else if (Input.mousePosition.y <= Screen.height * (1 - mScrollBound))
        {
            mCamera.transform.Translate(Vector3.down * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }
    }

    public void SetFollow(Transform _target)
    {
        mIsFollowing = true;
        mFollowTarget = _target;
    }

    public void StopFollow()
    {
        mIsFollowing = false;
        mFollowTarget = null;

        Mng.canvas.kBeeInfo.gameObject.SetActive(false);
    }
}
