using Inputs;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestCharacterController : NetworkBehaviour
{
    [SerializeField]
    InputReader inputReader;
    [SerializeField]
    private float speed = 5f;

    private CharacterController characterController;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        characterController = GetComponent<CharacterController>();
        //inputReader.MoveEvent += OnMove;
    }

    // despawn
    public override void OnNetworkDespawn()
    {
        //inputReader.MoveEvent -= OnMove;
        base.OnNetworkDespawn();
    }

    private void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.SimpleMove(movement * speed);
    }

    private void OnMove(Vector2 movement)
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        characterController.SimpleMove(move * speed);

    }
}
