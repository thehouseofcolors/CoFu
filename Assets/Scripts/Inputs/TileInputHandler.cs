using UnityEngine;
using UnityEngine.InputSystem;

public class TileInputHandler : Singleton<TileInputHandler>
{
    [Header("Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask tileLayerMask;
    private float positionTolerance = 0.1f;

    // Cached references
    private GameInput _input;
    private InputAction _tileClickAction;
    private InputAction _tilePointedAction;
    private bool _inputEnabled = false;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        _input = new GameInput();
        _tileClickAction = _input.GamePlay.TileClick;          // Button action for click
        _tilePointedAction = _input.GamePlay.TilePointed;      // Value<Vector2> action for pointer position
    }
    private void OnEnable()
    {
        _tileClickAction.started += HandleClickStart;
    }

    private void OnDisable()
    {
        _tileClickAction.started -= HandleClickStart;
    }

    public void EnableInput()
    {
        if (!_inputEnabled)
        {
            _input.Enable();
            _inputEnabled = true;
            Debug.Log("Tile input ENABLED");
        }
    }

    public void DisableInput()
    {
        if (_inputEnabled)
        {
            _input.Disable();
            _inputEnabled = false;
            Debug.Log("Tile input DISABLED");
        }
    }

    private void HandleClickStart(InputAction.CallbackContext context)
    {
        if (!_inputEnabled) return;
        //if game state is playing
        if (GameStateMachine.IsInState<NonPlayingState>()) return;

        
        Vector2 screenPosition = _tilePointedAction.ReadValue<Vector2>();

        Vector2 worldPosition = ScreenToWorldPrecise(screenPosition);

        Debug.DrawRay(worldPosition, Vector3.forward, Color.green, 1f);

        CheckForTileAtPosition(worldPosition);
    }

    private Vector2 ScreenToWorldPrecise(Vector2 screenPos)
    {
        // Kamera Z pozisyonu uzaklığı alınır (örneğin 10 ise)
        float zDistance = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));

        Debug.Log($"Screen: {screenPos} → World: {worldPoint}");
        return worldPoint;
    }

    private void CheckForTileAtPosition(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos, tileLayerMask);

        if (hit == null)
        {
            hit = Physics2D.OverlapCircle(worldPos, positionTolerance, tileLayerMask);
        }

        if (hit != null && hit.TryGetComponent<Tile>(out Tile tile) && tile.CanSelectable)
        {
            Debug.Log($"Selected tile at: {tile.transform.position}");
            tile.OnClicked().Forget();
        }
        // else
        // {
        //     Debug.LogWarning($"No tile found at: {worldPos}");
        // }
    }


}

