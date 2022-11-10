using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformsScript : MonoBehaviour
{
    // public
    public GameObject platformPrefab;
    
    // private
    private GameObject _platforms;
    private int _platformAmount = 20;
    
    private float _windowHorizontalExtent;
    private float _windowVerticalExtent;
    
    private float _platformHorizontalExtent;
    private float _platformVerticalExtent;

    private float _platformRangeX = 3.0f;
    private float _platformRangeY = 2.5f;

    private GameObject _player;
    private float _height;
    
    private Camera _camera;

    void Awake()
    {
        _platforms = GameObject.FindGameObjectWithTag("Platforms");

        _camera = Camera.main;
        
        _windowVerticalExtent = _camera.orthographicSize;
        _windowHorizontalExtent = _windowVerticalExtent * Screen.width / Screen.height;

        PlatformSpawn();
        
        _platformHorizontalExtent = _platforms.transform.GetChild(0).gameObject.GetComponent<Collider2D>().bounds.size.x / 2;
        _platformVerticalExtent = _platforms.transform.GetChild(0).gameObject.GetComponent<Collider2D>().bounds.size.y / 2;

        _player = GameObject.FindGameObjectWithTag("Player");
        
        Debug.Log(_windowVerticalExtent);
    }
    
    void Update()
    {
        // get height
        _height = _player.transform.position.y;

        // platform update
        PlatformLoop();
    }

    private void PlatformSpawn()
    {
        float r0X = Random.Range(-6.0f, 6.0f);
        float r0Y = Random.Range(-_windowVerticalExtent + 2.0f, -_windowVerticalExtent + 3.5f);
        GameObject firstPlatform= Instantiate(platformPrefab, new Vector3(r0X, r0Y, 0), Quaternion.identity);
        firstPlatform.transform.parent = _platforms.transform;

        for (int i = 1; i < _platformAmount; ++i)
        {
            GameObject newPlatform = Instantiate(platformPrefab, new Vector3(r0X, r0Y, 0), Quaternion.identity);
            newPlatform.transform.parent = _platforms.transform;
            PlatformReset(i);
        }
    }
    
    private void PlatformReset(int index)
    {
        int prevIndex = index - 1;
        if (prevIndex < 0)
        {
            prevIndex = 19;
        }

        float prevX = _platforms.transform.GetChild(prevIndex).transform.position.x;
        float prevY = _platforms.transform.GetChild(prevIndex).transform.position.y;

        float rx = Random.Range(prevX - _platformRangeX, prevX + _platformRangeX);
        float ry = Random.Range(prevY, 1.0f + (prevY + _platformRangeY));
        
        if (rx < -_windowHorizontalExtent)
        {
            rx = _windowHorizontalExtent * 2 + rx;
        }
        if (rx > _windowHorizontalExtent)
        {
            rx -= _windowHorizontalExtent * 2;
        }

        _platforms.transform.GetChild(index).transform.position = new Vector3(rx, ry, 0);
    }

    private void PlatformLoop()
    {
        for (int i = 0; i < _platforms.transform.childCount; ++i)
        {
            if (_platforms.transform.GetChild(i).transform.position.y + _platformVerticalExtent < 
                _camera.transform.position.y - _windowVerticalExtent - 5)
            {
                PlatformReset(i);
            }
        }
    }
}
