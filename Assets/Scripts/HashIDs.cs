using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashIDs : MonoBehaviour
{
    public int playerState;

    void Awake()
    {
        playerState = Animator.StringToHash("States");
    }
}
