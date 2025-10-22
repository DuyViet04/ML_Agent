using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private InputSystem_Actions inputSystemActions;
    [SerializeField] private float moveSpeed = 1.875f;
    
    private void Awake()
    {
        this.inputSystemActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        this.inputSystemActions.Enable();
    }
    
    private void OnDisable()
    {
        this.inputSystemActions.Disable();
    }

    private void FixedUpdate()
    {
        this.Move();
    }

    void Move()
    {
        Vector2 moveDirVec2 = this.inputSystemActions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirVec3 = new Vector3(moveDirVec2.x, 0, moveDirVec2.y);
        this.transform.position += moveDirVec3 * (this.moveSpeed * Time.fixedDeltaTime);
    }
}