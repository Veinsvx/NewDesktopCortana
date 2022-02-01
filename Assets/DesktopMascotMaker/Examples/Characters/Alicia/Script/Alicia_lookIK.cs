using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alicia_lookIK : MonoBehaviour {

    Animator anim;

    float weight = 0;
    public float bodyWeight = 0.5f;
    public float headWeight = 1.0f;
    public float eyesWeight = 0.0f;
    public float clampWeight = 1.0f;

    public Transform lookTarget;

    void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Assert(lookTarget != null, "lookTarget is null!", transform);
    }

    void OnAnimatorIK()
    {
        anim.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);        
        anim.SetLookAtPosition(lookTarget.position);
    }

    public void SetIKWeight(float _weight)
    {
        weight = _weight;
    }
}
