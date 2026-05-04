using UnityEngine;

public class WaitAction : INPCAction
{
    private readonly float _duration;
    private float _timer;
    public string Name => "Wait";

    public WaitAction(float duration) => _duration = duration;

    public bool CanExecute(MonoBehaviour npc) => true;
    
    public void OnStart(MonoBehaviour npc) => _timer = _duration;
    
    public bool OnUpdate(MonoBehaviour npc)
    {
        _timer -= Time.deltaTime;
        return _timer <= 0f;
    }
    
    public void OnCancel(MonoBehaviour npc) { }
    public void OnComplete(MonoBehaviour npc) { }
}
