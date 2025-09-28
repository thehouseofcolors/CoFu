using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;
public class BackInputHandler : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    private GameInput _input;
    private InputAction _backInput;
    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        _input = new GameInput();
        _backInput = _input.Mobile.BackClick;

    }


    private void OnEnable()
    {
        _input.Enable();
        _backInput.performed += OnBackPressed;
    }

    private void OnDisable()
    {
        _backInput.performed -= OnBackPressed;
        _input.Disable();
    }

    private void OnBackPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Geri tuşuna basıldı, menüye dön!");
        // Menüye dönüş işlemini buraya koy
        // Örneğin:
        
    }

}


