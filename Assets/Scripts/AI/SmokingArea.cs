using UnityEngine;

public class SmokingArea : MonoBehaviour
{
    [SerializeField] private Transform centerZone;
    [SerializeField] private float width = 2f;
    [SerializeField] private float height = 2f;

    public Vector3 GetRandomPointInZone()
    {
        float x = Random.Range(-width / 2, width / 2);
        float z = Random.Range(-height / 2, height / 2);
        return centerZone.position + new Vector3(x, 0, z);
    }
}
