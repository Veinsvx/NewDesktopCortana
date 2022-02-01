using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camAngles2 : MonoBehaviour {

    private SkinnedMeshRenderer facemoof;

    public Transform camObj;
    public GameObject faceObj;
    public Transform constObj;

    public float moofVal = 20;

    private float hAngleR = 0.0f;
    private float hAngleL = 0.0f;
    private float vAngleU = 0.0f;
    private float vAngleD = 0.0f;
    private Vector3 camPos = new Vector3(0.0f, 0.0f, 0.0f);
    //private Vector3 constangles = new Vector3(0.0f, 0.0f, 0.0f);
    //private Vector3 hangles = new Vector3(0.0f, 0.0f, 180f);

    //private float hAngle = 0.0f;
    //private float vAngle = 0.0f;

    //private Vector3 constPos = new Vector3(0.0f, 0.0f, 0.0f);
    //private Quaternion constRot = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

    void Start()
    {
        facemoof = faceObj.GetComponent<SkinnedMeshRenderer>();
    }

    void Update()
    {
        //ターゲットから角度を算出する;
        camPos = constObj.transform.InverseTransformPoint(camObj.transform.position);
        var hangle = Mathf.Atan2(Mathf.Abs(camPos.x), Mathf.Max(Mathf.Abs(camPos.z), 0.2f)) * Mathf.Rad2Deg;
        var vangle = Mathf.Atan2(Mathf.Abs(camPos.y), Mathf.Max(Mathf.Abs(camPos.z), 0.2f)) * Mathf.Rad2Deg;

        hAngleR = Mathf.Max(hangle - vangle, 0.0f);
        vAngleU = Mathf.Max(vangle - hangle, 0.0f);

        hAngleR = (hAngleR * hAngleR + 5f) / moofVal;
        vAngleU = (vAngleU * vAngleU + 5f) / moofVal;

        hAngleL = hAngleR;
        vAngleD = vAngleU;

        if (camPos.x < 0f)
        {
            hAngleR *= -1f;
        }
        else
        {
            hAngleL *= -1f;
        }

        //顔モーフ番号;
        facemoof.SetBlendShapeWeight(45, hAngleR);
        facemoof.SetBlendShapeWeight(44, hAngleL);
        facemoof.SetBlendShapeWeight(43, vAngleU);
        facemoof.SetBlendShapeWeight(43, vAngleD);
    }
}

