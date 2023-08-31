using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrailElement : MonoBehaviour
{
    [SerializeField]
    private Transform playerAvatar;
    [SerializeField]
    private float positionSmoothing;

    private Vector3 _velocity;

    void Awake()
    {
        if (playerAvatar == null)
        {
            playerAvatar = GameObject.Find("PlayerAvatar").transform;
        }
    }

    void Update()
    {
        var destination = playerAvatar.position;
        var position = transform.position;
        destination.z = position.z;
        
        position = Vector3.SmoothDamp(
            position,
            destination,
            ref _velocity,
            positionSmoothing);
        
        transform.position = position;
    }
}
