using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    private Camera mCamera;
    private float mScrollSpeed = 7.5f;
    private float mScrollBound = 0.98f;

    private float mCurScrollDelta = 6;

    private Transform mFollowTarget;
    private bool mIsFollowing = false;

    private bool mFirst = true;

    void Awake()
    {
        Instance = this;
        mCamera = gameObject.GetComponent<Camera>();

        var scaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();

        // 비율이 16:9 가 아닐 경우 비율을 맞춘다.
        if (Screen.width * 9 != Screen.height * 16)
        {
            if (Screen.width * 9 > Screen.height * 16)
                scaler.matchWidthOrHeight = 1;
            else
                scaler.matchWidthOrHeight = 0;
        }
    }

	private void OnDestroy()
	{
		Instance = null;
	}

	// Update is called once per frame
	void LateUpdate()
    {   
        Zoom();

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

    void Zoom()
    {
        if(PopupBase.IsTherePopup())
        {
            return;
        }
        
        mCurScrollDelta -= Input.mouseScrollDelta.y;
        mCurScrollDelta = Mathf.Clamp(mCurScrollDelta, 5, 25);
        mScrollSpeed = mCurScrollDelta*2;
        Camera.main.orthographicSize = mCurScrollDelta;
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
        Mng.canvas.kQueen.Hide();
    }
}
