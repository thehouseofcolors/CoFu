


using System.Threading.Tasks;
using GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileClickInput : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;

    private InputAction tileClickAction;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        var map = inputActionAsset.FindActionMap("Player", true);
        tileClickAction = map.FindAction("TileClick", true);
    }

    private  void OnEnable()
    {
 
        tileClickAction.performed += OnTileClickPerformed;
        tileClickAction.Enable();
    }
    async Task Update()
    {
        if (Pointer.current != null)
        {
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
                    await tile.HandleSelection(); // veya tile.Select() gibi
                }

            }
        }
        else
        {
            Debug.Log("Pointer.current is null");
        }
    }

    private void OnDisable()
    {
        tileClickAction.performed -= OnTileClickPerformed;
        tileClickAction.Disable();
    }
    private async Task TryHandleTileClick(InputAction.CallbackContext context)
    {
        Vector2 screenPos = context.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 rayOrigin = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

        if (hit.collider != null)
        {
            var tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                await tile.HandleSelection();
                Debug.Log($"Tile selected: {tile.name}");
            }
        }
        else
        {
            Debug.Log("No tile hit.");
        }
    }
    private void OnTileClickPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("OnTileClickPerformed tetiklendi");
        Vector2 screenPos = context.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 rayOrigin = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

        if (hit.collider != null)
        {
            var tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                tile.HandleSelection();
                Debug.Log($"Tile selected: {tile.name}");
            }
        }
        else
        {
            Debug.Log("No tile hit.");
        }
    }

}
