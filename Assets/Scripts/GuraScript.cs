using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuraScript : MonoBehaviour
{
    public enum States
    {
        Idle,
        Walking,
        Attacking,
        Jumping,
        Falling
    }

    // public
    public States currentState;

    // private
    private GameObject _playerGhost;

    private Camera _camera;
    private float _cameraHorizontalExtent;

    private Animator _anim;
    private HashIDs _hash;

    private float _dt;

    public float _jumpMultiplier;
    private bool _jumping;
    private const float Speed = 25;
    private bool _movingX;
    private Vector2Int _dir;

    private GameObject[] _platforms;
    private float _platformHorizontalExtent;
    private float _platformVerticalExtent;

    private float _playerHorizontalExtent;
    private float _playerVerticalExtent;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
        currentState = States.Idle;
        _camera = Camera.main;
        _playerGhost = GameObject.FindGameObjectWithTag("PlayerGhost");
        _platforms = GameObject.FindGameObjectsWithTag("Platform");

        _cameraHorizontalExtent = _camera.orthographicSize * Screen.width / Screen.height;

        _platformHorizontalExtent = _platforms[0].GetComponent<Collider2D>().bounds.size.x / 2;
        _platformVerticalExtent = _platforms[0].GetComponent<Collider2D>().bounds.size.y / 2; 

        _playerHorizontalExtent = GetComponent<Collider2D>().bounds.size.x / 2;
        _playerVerticalExtent = GetComponent<Collider2D>().bounds.size.y / 2;

        _dir = new Vector2Int(0, 0);
    }
    
    void Update()
    {
        _dt = Time.deltaTime;
        ManageInput();
        PlatformCollider();
        jump();
        ManageCamera();

        // change from jump to falling
        if (Speed * _dt * _dir.y * 1 - _jumpMultiplier < 0)
        {
            currentState = States.Falling;
        }
        
        // land and move
        if ((currentState != States.Jumping) &&
            (currentState != States.Falling) &&
            (_movingX))
        {
            currentState = States.Walking;
        }

        // don't run off the screen
        if (transform.position.x < -_cameraHorizontalExtent)
        {
            transform.position += new Vector3(_cameraHorizontalExtent * 2, 0, 0);
        }

        if (transform.position.x > _cameraHorizontalExtent)
        {
            transform.position -= new Vector3(_cameraHorizontalExtent * 2, 0, 0);
        }
    }

    private void LateUpdate()
    {
        // state animations
        switch(currentState)
        {
            case States.Idle:
                _anim.SetInteger(_hash.playerState, 0);
                break;

            case States.Walking:
                _anim.SetInteger(_hash.playerState, 1);
                break;

            case States.Jumping:
                _anim.SetInteger(_hash.playerState, 2);
                break;

            case States.Falling:
                _anim.SetInteger(_hash.playerState, 3);
                break;

            case States.Attacking:
                _anim.SetInteger(_hash.playerState, 4);
                break;
        }
    }

    void FixedUpdate()
    {
        MovementManager();
    }

    private void MovementManager()
    {
        // x axis
        transform.Translate(Speed * _dir.x * _dt, 0, 0);

        // y axis
        if ((currentState == States.Jumping) ||
            (currentState == States.Falling))
        {
            transform.Translate(0, Speed * 1.3f * _dt * _dir.y * 1 - _jumpMultiplier, 0);
            _jumpMultiplier += 1.5f * _dt;
        }

        if (_dir.y == 0)
        {
            _jumpMultiplier = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject floor = GameObject.FindGameObjectWithTag("Floor");
        Collider2D floorCollider = floor.GetComponent<Collider2D>();

        // floor
        if (other == floorCollider)
        {
            _dir.y = 0;
            _jumping = false;
            _jumpMultiplier = 0;
            transform.position = new Vector3(transform.position.x,
                                            floor.transform.position.y + gameObject.GetComponent<Collider2D>().bounds.size.y,
                                            transform.position.z);
            
            if (_movingX)
            {
                currentState = States.Walking;
            }
            else
            {
                currentState = States.Idle;
            }
        }
    }

    private void ManageInput()
    {
        // x axis
        if ((Input.GetKeyUp("a")) ||
            (Input.GetKeyUp("d")))
        {
            _dir.x = 0;
            _movingX = false;

            if ((currentState != States.Jumping) &&
                (currentState != States.Falling))
            {
                currentState = States.Idle;
            }
        }
        if (Input.GetKey("d"))
        {
            _dir.x = 1;
            _movingX = true;
            TurnRight();
        }
        if (Input.GetKey("a"))
        {
            _dir.x = -1;
            _movingX = true;
            TurnLeft();
        }

        // y axis
        if (Input.GetKey("w"))
        {
            if ((currentState != States.Jumping) &&
                (currentState != States.Falling))
            {
                _jumping = true;
            }     
        }
    }

    private void TurnLeft()
    {
        transform.localScale = new Vector3(-1, 1, 1);
        _playerGhost.transform.localScale = new Vector3(-1, 1, 1);
    }

    private void TurnRight()
    {
        transform.localScale = new Vector3(1, 1, 1);
        _playerGhost.transform.localScale = new Vector3(1, 1, 1);
    }

    private void PlatformCollider()
    {
        for (int i = 0; i < _platforms.Length; i++)
        {
            if (currentState == States.Falling)
            {
                if (_platforms[i].GetComponent<PlatformScript>().PlayerBottomCollided(gameObject))
                {
                    transform.position = new Vector3(transform.position.x,
                                                    _platforms[i].transform.position.y + _platformVerticalExtent * 1.6f + _playerVerticalExtent);
                    _dir.y = 0;
                    _jumpMultiplier = 0;
                    _platforms[i].GetComponent<PlatformScript>().stepped = true;
                    _jumping = false;
                    
                    if (_movingX)
                    {
                        currentState = States.Walking;
                    }
                    else
                    {
                        currentState = States.Idle;
                    }
                }
            }

            if (_platforms[i].GetComponent<PlatformScript>().stepped)
            {
                if ((currentState != States.Jumping) &&
                    (currentState != States.Falling))
                {
                    if (_platforms[i].GetComponent<PlatformScript>().PlayerFell(gameObject))
                    {
                        _dir.y = 1;
                        _jumpMultiplier = 0.40f;
                        currentState = States.Falling;
                        _platforms[i].GetComponent<PlatformScript>().stepped = false;
                    }
                }
            }
        }
    }

    private void jump()
    {
        if (_jumping)
        {
            _dir.y = 1;

            if (_jumpMultiplier * _dir.y * _dt <= 0)
            {
                currentState = States.Jumping;
            }
        }
    }

    private void ManageCamera()
    {
        if (transform.position.y > 0)
        {
            _camera.transform.position = new Vector3(_camera.transform.position.x, 
                                                    transform.position.y, 
                                                    _camera.transform.position.z);
        }
    }
}
