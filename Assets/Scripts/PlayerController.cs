using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Physics")]
    public float movementSpeed;

    private float _horizontalInput;
    private float _verticalInput;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    
    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        
        // var temp = new Vector2(_horizontalInput, _verticalInput).normalized * movementSpeed ; 
        // transform.Translate(temp * Time.deltaTime);
    }

    void FixedUpdate()
    {
        _rb.velocity = new Vector2(_horizontalInput, _verticalInput).normalized * movementSpeed;
    }
}
