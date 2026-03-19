using UnityEngine;

public class NPCMover : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private bool isMoving = true;
    public bool IsMoving => isMoving;
    public void MoveTo(Vector3 target, float distanceReached)
    {
        if(Vector3.Distance(transform.position, target) < distanceReached)
        {
            isMoving = false;
            return;
        }

        isMoving = true;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
