using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using GameEvents;

public class TileInputHandler : Singleton<TileInputHandler>, IGameSystem
{
    [Header("Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private float positionTolerance = 0.1f;

    // Cached references
    private GameInput _input;
    private InputAction _tileClickAction;
    private InputAction _tilePointedAction;
    private bool _inputEnabled = false;
    
    private List<IDisposable> _eventSubscriptions = new List<IDisposable>();

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        _input = new GameInput();
        _tileClickAction = _input.GamePlay.TileClick;          // Button action for click
        _tilePointedAction = _input.GamePlay.TilePointed;      // Value<Vector2> action for pointer position
    }
    public void Initialize()
    {
        // if (mainCamera == null)
        //     mainCamera = Camera.main;

        // _input = new GameInput();
        // _tileClickAction = _input.GamePlay.TileClick;          
        // _tilePointedAction = _input.GamePlay.TilePointed;   
        SubscribeEvents();
    }
    public void Shutdown()
    {
        UnsubscribeEvents();
    }
    private void SubscribeEvents()
    {
        _eventSubscriptions.Add(EventBus.Subscribe<GameStartEvent>(HandleGameStart));
        _eventSubscriptions.Add(EventBus.Subscribe<GamePauseEvent>(HandleGamePause));
        _eventSubscriptions.Add(EventBus.Subscribe<GameResumeEvent>(HandleGameResume));
        _eventSubscriptions.Add(EventBus.Subscribe<GameWinEvent>(HandleGameWin));
    }
    private void UnsubscribeEvents()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
    }

    private void HandleGameStart(GameStartEvent e)
    {
        EnableInput();
    }
    private void HandleGameWin(GameWinEvent e)
    {
        EnableInput();
    }
    private void HandleGamePause(GamePauseEvent e)
    {
        DisableInput();
    }

    private void HandleGameResume(GameResumeEvent e)
    {
        EnableInput();
    }
    private void OnEnable()
    {
        EnableInput();
        _tileClickAction.started += HandleClickStart;
    }

    private void OnDisable()
    {
        _tileClickAction.started -= HandleClickStart;
        DisableInput();
    }

    private void EnableInput()
    {
        if (!_inputEnabled)
        {
            _input.Enable();
            _inputEnabled = true;
            Debug.Log("Tile input ENABLED");
        }
    }

    private void DisableInput()
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

        // Direkt TilePointed pozisyonu oku
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
        else
        {
            Debug.LogWarning($"No tile found at: {worldPos}");
        }
    }


}

// [RequireComponent(typeof(PlayerInput))]
// public class TileInputHandler : Singleton<TileInputHandler>, IGameSystem
// {
//     [Header("Settings")]
//     [SerializeField] private Camera _mainCamera;
//     [SerializeField] private LayerMask _tileLayerMask;
//     [SerializeField] private float _positionTolerance = 0.1f;

//     // Cached references
//     private GameInput _playerInput;
//     private InputAction _clickAction;
//     private InputAction _tilePointedAction;
//     private bool _isInputEnabled = false;
//     private readonly List<IDisposable> _eventSubscriptions = new List<IDisposable>();

//     public void Initialize()
//     {
//         _mainCamera ??= Camera.main;

//         SetupInputActions();
//         SubscribeEvents();

//     }

//     public void Shutdown()
//     {
//         UnsubscribeEvents();
//         CleanupInputActions();
//     }

//     private void SetupInputActions()
//     {
        
//         _playerInput = new GameInput();
//         _clickAction = _playerInput.GamePlay.TileClick;          // Button action for click
//         _tilePointedAction = _playerInput.GamePlay.TilePointed;      // Value<Vector2> action for pointer position
    

//         _clickAction.started += HandleClickStarted;
//     }

//     private void CleanupInputActions()
//     {
//         _clickAction.started -= HandleClickStarted;
//     }

//     private void SubscribeEvents()
//     {
//         _eventSubscriptions.Add(EventBus.Subscribe<GameStartEvent>(HandleGameStart));
//         _eventSubscriptions.Add(EventBus.Subscribe<GamePauseEvent>(HandleGamePause));
//         _eventSubscriptions.Add(EventBus.Subscribe<GameResumeEvent>(HandleGameResume));
//         _eventSubscriptions.Add(EventBus.Subscribe<GameWinEvent>(HandleGameWin));
//     }

//     private void UnsubscribeEvents()
//     {
//         foreach (var subscription in _eventSubscriptions)
//         {
//             subscription?.Dispose();
//         }
//         _eventSubscriptions.Clear();
//     }

//     private void HandleGameStart(GameStartEvent e) => EnableInput();
//     private void HandleGameWin(GameWinEvent e) => DisableInput();
//     private void HandleGamePause(GamePauseEvent e) => DisableInput();
//     private void HandleGameResume(GameResumeEvent e) => EnableInput();

//     private void EnableInput()
//     {
//         if (_isInputEnabled) return;

//         _playerInput.Enable();  // eklendi
//         _isInputEnabled = true;
//         Debug.Log("[Input] Tile input enabled");
//     }

//     private void DisableInput()
//     {
//         if (!_isInputEnabled) return;

//         _playerInput.Disable();  // eklendi
//         _isInputEnabled = false;
//         Debug.Log("[Input] Tile input disabled");
//     }


//     private void HandleClickStarted(InputAction.CallbackContext context)
//     {
//         if (!_isInputEnabled) return;

//         var screenPos = _tilePointedAction.ReadValue<Vector2>();  // Ekle bunu
//         var worldPos = ScreenToWorldPosition(screenPos);
//         Debug.DrawRay(worldPos, Vector3.forward, Color.green, 1f);
//         ProcessTileSelection(worldPos);
//     }


//     private Vector2 ScreenToWorldPosition(Vector2 screenPos)
//     {
//         var zDistance = Mathf.Abs(_mainCamera.transform.position.z);
//         return _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
//     }

//     private void ProcessTileSelection(Vector2 worldPos)
//     {
//         var tile = FindColorAtPosition(worldPos);

//         try
//         {
//             tile.OnClicked().Forget();
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"Tile interaction failed: {ex.Message}");
//         }
//     }

//     private Tile FindColorAtPosition(Vector2 worldPos)
//     {
//         var hit = Physics2D.OverlapPoint(worldPos, _tileLayerMask)
//                ?? Physics2D.OverlapCircle(worldPos, _positionTolerance, _tileLayerMask);

//         return hit?.GetComponent<Tile>();
//     }

// }

