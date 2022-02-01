using UnityEngine;
using System.Collections;

public class Swapper : MonoBehaviour
{

    public GameObject[] character;
    public int index;
    void Awake()
    {
        foreach (GameObject c in character)
        {
            c.SetActive(false);
        }
        character[0].SetActive(true);
    }
    public void SwitchSkin()
    {
        character[index].SetActive(false);
        index++;
        index %= character.Length;
        character[index].SetActive(true);
    }
}

