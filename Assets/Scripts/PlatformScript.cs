using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    // public
    public bool stepped = false;
    public int index;

    // private
    private float _platformHorizontalExtent;
    private float _platformVerticalExtent;

    private GameObject _player;
    private float _playerHorizontalExtent;
    private float _playerVerticalExtent;

    private void Awake()
    {
        _platformHorizontalExtent = GetComponent<Collider2D>().bounds.size.x / 2;
        _platformVerticalExtent = GetComponent<Collider2D>().bounds.size.y / 2;

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerHorizontalExtent = _player.GetComponent<Collider2D>().bounds.size.x / 2;
        _playerVerticalExtent = _player.GetComponent<Collider2D>().bounds.size.y / 2;
    }

    public bool PlayerBottomCollided(GameObject player)
    {
        if ((GetRight(player) > GetLeft(gameObject)) &&
            (GetLeft(player) < GetRight(gameObject)) &&
            (GetBottom(player) < GetTop(gameObject)) &&
            (GetBottom(player) > GetBottom(gameObject)))
        {
            return true;
        }
        return false;
    }
    
    public bool PlayerFell(GameObject player)
    {
        if ((GetRight(player) < GetLeft(gameObject)) ||
            (GetLeft(player) > GetRight(gameObject)))
        {
            return true;
        }
        return false;
    }

    private float GetLeft(GameObject gameObject)
    {
        float x = _platformHorizontalExtent;
        if (gameObject == _player)
        {
            x = _playerHorizontalExtent;
        }
        return gameObject.transform.position.x - x;
    }

    private float GetRight(GameObject gameObject)
    {
        float x = _platformHorizontalExtent;
        if (gameObject == _player)
        {
            x = _playerHorizontalExtent;
        }
        return gameObject.transform.position.x + x;
    }

    private float GetTop(GameObject gameObject)
    {
        float x = _platformVerticalExtent;
        if (gameObject == _player)
        {
            x = _playerVerticalExtent;
        }
        return gameObject.transform.position.y + x;
    }

    private float GetBottom(GameObject gameObject)
    {
        float x = _platformVerticalExtent;
        if (gameObject == _player)
        {
            x = _playerVerticalExtent;
        }
        return gameObject.transform.position.y - x;
    }
}
