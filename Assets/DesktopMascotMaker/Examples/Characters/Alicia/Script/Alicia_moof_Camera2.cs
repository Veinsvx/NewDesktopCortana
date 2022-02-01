using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alicia_moof_Camera2 : MonoBehaviour {

    public Transform target;

    private float zoomSpeed = 0.5f;
    private float panSpeed = 1.0f;
    private float distance = 0.7f;

    private float orbitX = 17.0f;
    private float orbitY = 17.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    private float firstdistance = 0.0f;
    private Vector3 firsttagetPos;
    private float firstanglex = 0.0f;
    private float firstangley = 0.0f;

    public float rotationDamping = 3.0f;

    void Start()
    {
        firstdistance = distance;
        firsttagetPos = target.transform.position;

        var angles = transform.eulerAngles;
        x = angles.y;
        firstanglex = angles.y;
        y = angles.x;
        firstangley = angles.x;
    }

    void LateUpdate()
    {
        //zoom
        if (Input.GetMouseButton(1))
        {
            distance += Input.GetAxis("Mouse Y") * zoomSpeed;
            distance = Mathf.Clamp(distance, 0.5f, 2.0f);
        }
        else
        {
            distance -= (distance - firstdistance) * Time.deltaTime;
        }

        //orbit
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * orbitX;
            y -= Input.GetAxis("Mouse Y") * orbitY;
        }
        else
        {
            x -= (x - firstanglex) * Time.deltaTime;
            y -= (y - firstangley) * Time.deltaTime;
        }

        //pan
        target.transform.rotation = transform.rotation;

        if (Input.GetMouseButton(2))
        {
            var panVal = panSpeed * (distance / 20);
            if (Input.GetAxis("Mouse Y") != 0)
            {
                target.transform.Translate(-Vector3.up * Input.GetAxis("Mouse Y") * panVal);
                transform.Translate(-Vector3.up * Input.GetAxis("Mouse Y") * panVal);
            }
            if (Input.GetAxis("Mouse X") != 0)
            {
                target.transform.Translate(-Vector3.right * Input.GetAxis("Mouse X") * panVal);
                transform.Translate(-Vector3.right * Input.GetAxis("Mouse X") * panVal);
            }
        }
        else
        {
            target.transform.position -= (target.transform.position - firsttagetPos) * Time.deltaTime;
        }

        var rotation = Quaternion.Euler(y, x, 0);
        var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

        target.transform.rotation = rotation;
        transform.position = position;
    }
}
