using UnityEngine;

[CreateAssetMenu(fileName = "Release", menuName = "Throwable Settings")]
public class ThrowableSettings : ScriptableObject
{
    [SerializeField] private float throwForce = 10f;

    public float GetThrowForce() => throwForce;
}
