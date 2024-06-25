using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseFollowForHand : MonoBehaviour
{
    public float yOffset = 0f;
    [FormerlySerializedAs("zOffset")] public float xOffset = 0f;
    private Animator _animator;
    private Camera _mainCam;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.y += yOffset;
        mousePosition.x += xOffset;
        transform.position = Vector3.Lerp(transform.position, mousePosition, 5f);

        if (Input.GetMouseButtonDown(0))
        {
            _animator.Play("handanim");
        }
    }
}