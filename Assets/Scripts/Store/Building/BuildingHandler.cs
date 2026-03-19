using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private LayerMask gridMask;
    [SerializeField] private int placeDistance = 5;
    [SerializeField] private Grid grid;

    private IBuildable _heldBuildable;
    private GameObject _heldPrefab;
    private GridCell _targetCell;
    private float _currentRotationY = 0f;

    public IBuildable Buildable => _heldBuildable;

    private Vector3 GetWallOffset(float rotationY, float wallDepth)
    {
        Vector3 backDir = Quaternion.Euler(0, rotationY, 0) * Vector3.back;
        return backDir * wallDepth;
    }

    private void Update()
    {
        if (_heldBuildable == null) return;

        if(Input.GetKeyDown(KeyCode.R)) _currentRotationY = (_currentRotationY + 90f) % 360f;

        // snapping to grid
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (!Physics.Raycast(ray, out RaycastHit hit, placeDistance, gridMask))
        {
            if(_heldPrefab != null) _heldPrefab.SetActive(false);
            _targetCell = null;
            return;
        }

        Vector2Int baseCoords = grid.WorldToGrid(hit.point);
        if(!grid.TryGetCell(baseCoords, out _targetCell))
        {
            if(_heldPrefab != null) _heldPrefab.SetActive(false);
            return;
        }

        if (_heldPrefab == null)
            _heldPrefab = Instantiate(_heldBuildable.GhostPrefab);

        Vector2 size = _heldBuildable.Size;
        
        Vector3 visualCenter = CalculateVisualPositions(_targetCell, size, grid.SlotSize);
        Vector3 wallOffset = GetWallOffset(_currentRotationY, _heldBuildable.WallOffset);

        _heldPrefab.SetActive(true);
        _heldPrefab.transform.position = visualCenter + wallOffset;
        _heldPrefab.transform.rotation = Quaternion.Euler(0, _currentRotationY, 0);

        bool canPlace = buildingManager.CanPlace(baseCoords, size);
        _heldPrefab.GetComponentInChildren<Renderer>().material.color = canPlace ? Color.green : Color.red;
    }

    public void DoInteract(GameObject currentInteractable)
    {
        if (currentInteractable == null) return;
        if (_heldBuildable != null) return;
        
        var buildable = currentInteractable.GetComponentInParent<IBuildable>();
        if(buildable == null) return;
        
        //if (!currentInteractable.TryGetComponent<IBuildable>(out _)) return;

        Debug.Log($"[BuildingHandler] remembered: {currentInteractable.gameObject.name}");
        _heldBuildable = buildable;

        ((MonoBehaviour)_heldBuildable).gameObject.SetActive(false);

        GridCell cell = grid.GetCell(currentInteractable.transform.position);

        if (cell != null && cell.IsOccupied && cell.buildable == buildable)
        {
            buildingManager.TakeBuildable(currentInteractable.transform.position);
        }
    }

    public Vector3 CalculateVisualPositions(GridCell baseCell, Vector2 size, Vector2 slotSize)
    {
        Vector3 offset = new Vector3
        (
            (size.x - 1) * slotSize.x / 2f,
            0,
            (size.y - 1) * slotSize.y / 2f
        );

        return baseCell.worldPosittion + offset;
    }

    public void DoPlace()
    {
        if (_heldBuildable == null || _targetCell == null) return;

        Vector2 size = _heldBuildable.Size;
        if(!buildingManager.CanPlace(_targetCell.gridPosition, size)) return;

        var original = ((MonoBehaviour)_heldBuildable).gameObject;
        
        Vector3 finalPos = CalculateVisualPositions(_targetCell, size, grid.SlotSize) + GetWallOffset(_currentRotationY, _heldBuildable.WallOffset);

        original.transform.position = finalPos;
        original.transform.rotation = Quaternion.Euler(0, _currentRotationY, 0);
        original.SetActive(true);

        Destroy(_heldPrefab);
        _heldPrefab = null;

        buildingManager.PlaceBuildable(_targetCell.gridPosition, _heldBuildable, size);
        _heldBuildable = null;
        _targetCell = null;
        _currentRotationY = 0f;
    }

    public void CancelHolding()
    {
        if(_heldBuildable == null) return;

        ((MonoBehaviour)_heldBuildable).gameObject.SetActive(true);
        Destroy(_heldPrefab);
        _heldPrefab = null;
        _heldBuildable = null;
        _targetCell = null;
        _currentRotationY = 0f;
    }

    public bool HasHeldBuildable() => _heldBuildable != null;
}
