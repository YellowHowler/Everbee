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

        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        if ((Input.mousePosition.x >= Screen.width * mScrollBound) || (horizontalAxis > 0.5f))
        {
            mCamera.transform.Translate(Vector3.right * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }
        else if ((Input.mousePosition.x <= Screen.width * (1 - mScrollBound)) || (horizontalAxis < -0.5f))
        {
            mCamera.transform.Translate(Vector3.left * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }
        if ((Input.mousePosition.y >= Screen.height * mScrollBound) || (verticalAxis > 0.5f))
        {
            mCamera.transform.Translate(Vector3.up * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }
        else if ((Input.mousePosition.y <= Screen.height * (1 - mScrollBound)) || (verticalAxis < -0.5f))
        {
            mCamera.transform.Translate(Vector3.down * Time.deltaTime * mScrollSpeed, Space.World);
            StopFollow();
        }

        Vector3 pos = mCamera.transform.position;
        float ratio = (float)Screen.width / (float)Screen.height;
        float halfCameraWidth = mCamera.orthographicSize * ratio;
        float halfCameraHeight = mCamera.orthographicSize;
        const float threshold = 1;

        if (pos.x < Manager.Instance.WorldBoundary.xMin + halfCameraWidth - threshold)
            pos.x = Manager.Instance.WorldBoundary.xMin + halfCameraWidth - threshold;
		if (pos.x > Manager.Instance.WorldBoundary.xMax - halfCameraWidth + threshold)
			pos.x = Manager.Instance.WorldBoundary.xMax - halfCameraWidth + threshold;

        if (pos.y < Manager.Instance.WorldBoundary.yMin + halfCameraHeight - threshold)
            pos.y = Manager.Instance.WorldBoundary.yMin + halfCameraHeight - threshold;
		if (pos.y > Manager.Instance.WorldBoundary.yMax - halfCameraHeight + threshold)
			pos.y = Manager.Instance.WorldBoundary.yMax - halfCameraHeight + threshold;

        mCamera.transform.position = pos;
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
