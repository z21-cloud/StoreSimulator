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

    public void DoPlace()
    {
        if (_heldBuildable == null) return;
        if (_targetCell == null) return;
        if (_targetCell.IsOccupied) return;

        var original = ((MonoBehaviour)_heldBuildable).gameObject;
        original.transform.position = _targetCell.worldPosittion;
        original.SetActive(true);

        Destroy(_heldPrefab);
        _heldPrefab = null;

        buildingManager.PlaceBuildable(_targetCell.worldPosittion, _heldBuildable);
        _heldBuildable = null;
        _targetCell = null;
    }

    public void CancelHolding()
    {
        if(_heldBuildable == null) return;

        ((MonoBehaviour)_heldBuildable).gameObject.SetActive(true);
        Destroy(_heldPrefab);
        _heldPrefab = null;
        _heldBuildable = null;
        _targetCell = null;
    }

    private void Update()
    {
        if (_heldBuildable == null) return;

        // snapping to grid
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (!Physics.Raycast(ray, out RaycastHit hit, placeDistance, gridMask))
        {
            if(_heldPrefab != null) _heldPrefab.SetActive(false);
            _targetCell = null;
            return;
        }

        _targetCell = grid.GetCell(hit.point);
        if (_targetCell == null) return;

        if (_heldPrefab == null)
            _heldPrefab = Instantiate(_heldBuildable.GhostPrefab);
        
        _heldPrefab.SetActive(true);
        _heldPrefab.transform.position = _targetCell.worldPosittion;

        bool canPlace = buildingManager.CanPlace(hit.point);
        _heldPrefab.GetComponentInChildren<Renderer>().material.color = canPlace 
                                                                    ? Color.green 
                                                                    : Color.red; 
    }
}
