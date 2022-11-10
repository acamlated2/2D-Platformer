using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    private GuraScript _playerScript;
    private GameObject _playerObject;
    
    private Animator anim;
    private HashIDs hash;

    private void Awake()
    {
        _playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<GuraScript>();
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
    }

    private void Update()
    {
        // position
        if (_playerObject.transform.position.x > 0)
        {
            transform.position = _playerObject.transform.position + new Vector3(-18, 0, 0);
        }

        if (_playerObject.transform.position.x < 0)
        {
            transform.position = _playerObject.transform.position + new Vector3(18, 0, 0);
        }
        
        // states
        switch (_playerScript.currentState)
        {
            case GuraScript.States.Idle:
                anim.SetInteger(hash.playerState, 0);
                break;

            case GuraScript.States.Walking:
                anim.SetInteger(hash.playerState, 1);
                break;

            case GuraScript.States.Jumping:
                anim.SetInteger(hash.playerState, 2);
                break;

            case GuraScript.States.Falling:
                anim.SetInteger(hash.playerState, 3);
                break;

            case GuraScript.States.Attacking:
                anim.SetInteger(hash.playerState, 4);
                break;
        }
    }
}
