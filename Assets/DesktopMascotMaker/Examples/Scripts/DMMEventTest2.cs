using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMMEventTest2 : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip se;
    public GameObject unityChan;
    public GameObject ethan;

    void Start()
    {
        unityChan.SetActive(true);
        ethan.SetActive(false);
    }

    // Invoked from DMM UI Emulator's UnityEvent
    public void TestEvent()
    {
        audioSource.PlayOneShot(se);

        if (unityChan.activeSelf)
        {
            unityChan.SetActive(false);
            ethan.SetActive(true);
        }
        else
        {
            unityChan.SetActive(true);
            ethan.SetActive(false);
        }
    }
}
