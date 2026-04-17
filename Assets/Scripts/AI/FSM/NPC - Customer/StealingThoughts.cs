using UnityEngine;

public class StealingThoughts : MonoBehaviour
{
    private int STEAL_DECISION_THRESHOLD = 65;
    private int STEAL_WAY_DECISION_THRESHOLD = 50;
    public bool StealItemOrNot()
    {
        int result = Random.Range(0, 100);
        if(result >= STEAL_DECISION_THRESHOLD) return true;
        return false;
    }

    public bool LoudStealOrQuiete()
    {
        // true => notify player
        // false => queite robbery

        int result = Random.Range(0, 100);
        if(result >= STEAL_WAY_DECISION_THRESHOLD) return true;
        return false;
    }
}
