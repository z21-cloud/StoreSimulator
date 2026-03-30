using StoreSimulator.InteractableObjects;
using UnityEngine;

public class BuildableItem : MonoBehaviour, IBuildable, IInteractable
{
    [SerializeField] private Vector2 itemSize;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float wallOffset = 0f;
    [SerializeField] private float yOffset = .5f;

    public Vector2 Size => itemSize;
    public GameObject GhostPrefab => ghostPrefab;
    public float WallOffset => wallOffset;
    public float YOffset => yOffset;
    public Vector3 BuildPosition { get; private set; }
    public float BuildRotationY => transform.eulerAngles.y;

    private void Start()
    {
        BuildingService.Instance.RegisterBuildable(this);
        GetMinPosition();
    }

    private void GetMinPosition()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            BuildPosition = transform.position;
            return;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        BuildPosition = bounds.min;
    }

    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlaced()
    {
        throw new System.NotImplementedException();
    }

    public void OnRemoved()
    {
        throw new System.NotImplementedException();
    }
}
