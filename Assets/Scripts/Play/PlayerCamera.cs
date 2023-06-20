using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    private Camera mCamera;
    private float mScrollSpeed = 2.5f;
    private float mScrollBound = 0.98f;

    void Awake()
    {
        Instance = this;
        mCamera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x >= Screen.width * mScrollBound)
        {
            mCamera.transform.Translate(Vector3.right * Time.deltaTime * mScrollSpeed, Space.World);
        }
        else if (Input.mousePosition.x <= Screen.width * (1 - mScrollBound))
        {
            mCamera.transform.Translate(Vector3.left * Time.deltaTime * mScrollSpeed, Space.World);
        }
        if (Input.mousePosition.y >= Screen.height * mScrollBound)
        {
            mCamera.transform.Translate(Vector3.up * Time.deltaTime * mScrollSpeed, Space.World);
        }
        else if (Input.mousePosition.y <= Screen.height * (1 - mScrollBound))
        {
            mCamera.transform.Translate(Vector3.down * Time.deltaTime * mScrollSpeed, Space.World);
        }
    }

    public void SetFollow(Transform _target)
    {
        
    }
}
