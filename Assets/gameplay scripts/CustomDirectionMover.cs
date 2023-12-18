using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDirectionMover : MonoBehaviour
{
    [SerializeField] Vector3 directionToMove;
    [SerializeField] float speed;
    [SerializeField] bool stopMoving;

    // Update is called once per frame
    void Update()
    {
        MoveObjectToSpecifiedDirection(directionToMove);
    }


    void MoveObjectToSpecifiedDirection(Vector3 _direction)
    {
        if (stopMoving)return;
        transform.Translate(_direction * speed * Time.deltaTime);
    }


    public void StopMovingObject()
    {
        stopMoving = true;
    }
}
