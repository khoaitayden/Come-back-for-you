using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance;

    private PlayerControls controls;
    public Vector2 MoveInput { get; private set; }
    public bool LaunchPressed { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}