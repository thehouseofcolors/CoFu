


using System.Threading.Tasks;
using GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

// public class TileClickInput : MonoBehaviour
// {
//     [SerializeField] private InputActionAsset inputActionAsset;

//     private InputAction tileClickAction;
//     private Camera mainCamera;

//     private void Awake()
//     {
//         mainCamera = Camera.main;
//         var map = inputActionAsset.FindActionMap("Player", true);
//         tileClickAction = map.FindAction("TileClick", true);
//     }

//     private  void OnEnable()
//     {
//         tileClickAction.Enable();
//     }
//     void Update()
//     {
//         if (Pointer.current != null)
//         {
//             Vector2 screenPos = Pointer.current.position.ReadValue();
//             if (Pointer.current.press.wasPressedThisFrame)
//             {
//                 Debug.Log("Pointer clicked at: " + screenPos);

//                 Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
//                 Vector2 point = new Vector2(worldPos.x, worldPos.y);
//                 RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
//                 var tile = hit.collider.GetComponent<Tile>();
//                 if (tile != null)
//                 {
//                     EventBus.PublishAuto(new TileSelectionEvent(tile));
//                 }

//             }
//         }
//         else
//         {
//             Debug.Log("Pointer.current is null");
//         }
//     }

//     private void OnDisable()
//     {
//         tileClickAction.Disable();
//     }

// }


// // // using System.Threading.Tasks;
// // // using GameEvents;
// // // using UnityEngine;
// // // using UnityEngine.InputSystem;

// // // public class TileClickInput : MonoBehaviour
// // // {
// // //     [SerializeField] private InputActionAsset inputActionAsset;

// // //     private InputAction tileClickAction;
// // //     private Camera mainCamera;

// // //     private void Awake()
// // //     {
// // //         mainCamera = Camera.main;
// // //         var map = inputActionAsset.FindActionMap("Player", true);
// // //         tileClickAction = map.FindAction("TileClick", true);
// // //     }

// // //     private void OnEnable()
// // //     {
// // //         tileClickAction.performed += OnTileClickPerformed;
// // //         tileClickAction.Enable();
// // //     }

// // //     private void OnDisable()
// // //     {
// // //         tileClickAction.performed -= OnTileClickPerformed;
// // //         tileClickAction.Disable();
// // //     }

// // //     private async void OnTileClickPerformed(InputAction.CallbackContext context)
// // //     {
// // //         Vector2 screenPos = context.ReadValue<Vector2>();
// // //         Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
// // //         Vector2 rayOrigin = new Vector2(worldPos.x, worldPos.y);

// // //         RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

// // //         if (hit.collider != null && hit.collider.TryGetComponent<Tile>(out var tile))
// // //         {
// // //             Debug.Log($"Tile selected: {tile.name}");
// // //             await EventBus.PublishAuto(new TileSelectionEvent(tile));
// // //         }
// // //         else
// // //         {
// // //             Debug.Log("No tile hit.");
// // //         }
// // //     }
// // // }



public class TileClickInput : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private float clickCooldown = 0.2f;

    private Camera _mainCamera;
    private bool _isInCooldown;

    private void Awake()
    {
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("Main camera not found!", this);
            enabled = false;
        }
    }
    void Update()
    {
        if (GameStateMachine.CurrentStateName != "GamePlayState") return;

        if (Pointer.current != null)
        {
            Debug.Log("pointer");
            Vector2 screenPos = Pointer.current.position.ReadValue();
            if (Pointer.current.press.wasPressedThisFrame)
            {
                Debug.Log("Pointer clicked at: " + screenPos);

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                Vector2 point = new Vector2(worldPos.x, worldPos.y);
                RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
                var tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    Debug.Log("tile");
                    EventBus.PublishAuto(new TileSelectionEvent(tile));
                }

            }
        }
        else
        {
            Debug.Log("Pointer.current is null");
        }
    }

    public void OnTileClick(InputAction.CallbackContext context)
    {
        if (!context.performed || _isInCooldown) return;
        Debug.Log("click");
        ProcessClickAsync();
    }

    private async void ProcessClickAsync()
    {
        _isInCooldown = true;

        try
        {
            // Get current input method position
            Vector2 screenPos = GetInputPosition();
            Debug.Log($"Processing click at: {screenPos}");

            // Convert to world position with proper Z position
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_mainCamera.transform.position.z))
            );

            // Raycast to find tiles
            var hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, tileLayerMask);

            if (hit.collider != null && hit.collider.TryGetComponent<Tile>(out var tile))
            {
                Debug.Log($"Tile hit: {tile.name}");
                await EventBus.PublishAuto(new TileSelectionEvent(tile));
            }
            else
            {
                Debug.Log("No tile hit");
            }
        }
        finally
        {
            await Task.Delay((int)(clickCooldown * 1000));
            _isInCooldown = false;
        }
    }

    private Vector2 GetInputPosition()
    {
        // Try mouse first
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        // Then try touchscreen
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        return Vector2.zero;
    }
}

