using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileInputHandler : MonoBehaviour, IGameSystem, IPausable, IQuittable
{
    [Header("Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private float positionTolerance = 0.1f;
    [SerializeField] private float clickCooldown = 0.2f;

    private float lastClickTime;
    private GameInput _input;
    private InputAction _tileClickAction;
    private InputAction _tilePointedAction;
    private bool _inputEnabled = false;

    private List<IDisposable> _eventSubscription = new List<IDisposable>();
    private bool _isTransitioning;
    private ScreenType? _lastScreenBeforePause;

    #region IGameSystem-IPausable-IQuittable Implementation

    public async Task Initialize()
    {
        try
        {
            _eventSubscription.Add(EventBus.Subscribe<GamePlayActivityChangedEvent>(HandleInputActivity));
        }
        catch (Exception ex)
        {
            Debug.LogError($"UI Manager initialization failed: {ex}");
            throw;
        }
        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        try
        {
            foreach (var sub in _eventSubscription)
            {
                sub?.Dispose();
                
            }
            _eventSubscription.Clear();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Debug.LogError($"UI Manager shutdown error: {ex}");
            throw;
        }
    }

    public void OnPause()
    {
        DisableInput();
    }

    public void OnResume()
    {
        EnableInput();
    }

    public void OnQuit()
    {
        DisableInput();
    }

    #endregion

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
    public void HandleInputActivity(GamePlayActivityChangedEvent e)
    {
        if (e.IsActive)
        {
            EnableInput();
        }
        else
        {
            DisableInput();
        }
        Debug.Log($"Input status {e.IsActive}");
    }


    private void HandleClickStart(InputAction.CallbackContext context)
    {
        if (!_inputEnabled) return;
        if (!GameStateMachine.IsInState<PlayingState>()) return;
        
        // Cooldown check
        if (Time.time < lastClickTime + clickCooldown) return;
        lastClickTime = Time.time;

        try
        {
            Vector2 screenPosition = _tilePointedAction.ReadValue<Vector2>();
            Vector2 worldPosition = ScreenToWorldPrecise(screenPosition);
            
            Debug.DrawRay(worldPosition, Vector3.forward, Color.green, 1f);
            
            if (TryGetTileAtPosition(worldPosition, out Tile tile))
            {
                Debug.Log($"Selected tile at: {tile.transform.position}");
                tile.OnClicked().Forget();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error handling tile click: {ex}");
        }
    }

    private bool TryGetTileAtPosition(Vector2 worldPos, out Tile tile)
    {
        tile = null;
        var hit = Physics2D.OverlapPoint(worldPos, tileLayerMask) ?? 
                Physics2D.OverlapCircle(worldPos, positionTolerance, tileLayerMask);

        if (hit != null)
        {
            if (hit.TryGetComponent<Tile>(out tile))
            {
                return true;
            }
        }
        if (hit != null)
        {
            tile = hit.GetComponent<Tile>();
            if (tile != null)
            {
                Debug.Log($"Tile found: {tile.name}, IsEmpty: {tile.IsEmpty()}");
            }
            else
            {
                Debug.LogWarning("Hit collider does not have Tile component");
            }
        }

        return false;
    }

    private Vector2 ScreenToWorldPrecise(Vector2 screenPos)
    {
        // Kamera Z pozisyonu uzaklığı alınır (örneğin 10 ise)
        float zDistance = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));

        Debug.Log($"Screen: → World: ");
        Debug.Log($"Clicked screen pos: {screenPos}");
        Debug.Log($"Converted world pos: {worldPoint}");
        return worldPoint;
    }



}

