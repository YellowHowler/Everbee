using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    private Camera mCamera;
    private float mScrollSpeed = 3.5f;
    private float mScrollBound = 0.98f;

    private Transform mFollowTarget;
    private bool mIsFollowing = false;

    private bool mFirst = true;

    void Awake()
    {
        Instance = this;
        mCamera = gameObject.GetComponent<Camera>();
    }

	private void OnDestroy()
	{
		Instance = null;
	}

	// Update is called once per frame
	void LateUpdate()
    {   
        if (mFirst)
        {
            Vector3 startPos = mCamera.transform.position;
            startPos.x = (PlayManager.Instance.kHiveXBound.start + PlayManager.Instance.kHiveXBound.end) / 2;
            startPos.y = (PlayManager.Instance.kHiveYBound.start + PlayManager.Instance.kHiveYBound.end) / 2;
            mCamera.transform.position = startPos;

            mFirst = false;
        }
        else
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
        }

        Vector3 pos = mCamera.transform.position;
        float ratio = (float)Screen.width / (float)Screen.height;
        float halfCameraWidth = mCamera.orthographicSize * ratio;
        float halfCameraHeight = mCamera.orthographicSize;
        const float threshold = 1;

        if (pos.x < PlayManager.Instance.WorldBoundary.xMin + halfCameraWidth - threshold)
            pos.x = PlayManager.Instance.WorldBoundary.xMin + halfCameraWidth - threshold;
		if (pos.x > PlayManager.Instance.WorldBoundary.xMax - halfCameraWidth + threshold)
			pos.x = PlayManager.Instance.WorldBoundary.xMax - halfCameraWidth + threshold;

        if (pos.y < PlayManager.Instance.WorldBoundary.yMin + halfCameraHeight - threshold)
            pos.y = PlayManager.Instance.WorldBoundary.yMin + halfCameraHeight - threshold;
		if (pos.y > PlayManager.Instance.WorldBoundary.yMax - halfCameraHeight + threshold)
			pos.y = PlayManager.Instance.WorldBoundary.yMax - halfCameraHeight + threshold;

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

        Mng.canvas.kBeeInfo.Hide();
    }
}
