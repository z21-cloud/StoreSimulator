using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class StealingState : INPCState
{
    private readonly NPCController _ctx;

    public StealingState(NPCController ctx) => _ctx = ctx;

    private bool notifyPlayer = false;

    public void Enter()
    {
        // переход на состояние осуществляется автоматически, если еда и вода критически низкие, 
        // т.е. при сильной жажде или голоде NPC начинает думать: а украсть ли мне?
        // к этой системе неплохо было бы прикрутить лояльность, что вот если достигли 90+, то краж не будет
        // и наоборот, если лояльность очень низкая, скажем, 20, то NPC в основном будут воровать

        // или вообще надо это сделать просто отдельным состоянием, а всю логику вычислений вынести
        // в отдельный скрипт для Psycho. В духе: "StealingThoughts", что вот денег нет, кушать хочется, надо попытаться украсть
        // Условно, вызываться он будет при условии, что NPC хочет что-то купить, денег не хватает. 
        // Тогда он будет проверять текущие потребности. Если они не критичные, то воровать не будет. 
        // Если же критичные и лояльность не в прайме, то пытаемся своровать и если успешно, то переключаем просто на этот state. 
        // Тогда вычисления идут в другом скрипте, а тут просто выполнение действий 

        // когда входим в состояние - считаем 2 вероятности
        // 1 будет ли красть NPC или нет
        // 2 будет ли это "тихая" кража, без уведомления для игрока, или это громкая кража, игроку придет уведомление

        // В любом случае воровство у NPC всегда вероятность, вариант воровства (тихий / громкий тоже)
        if (_ctx.Psycho.LoudStealOrQuiete()) notifyPlayer = true;
    }

    public void Tick()
    {
        // NPC пытается взять предмет
        // Независимо от результата уходим
        // Берется за одну кражу пока что только 1 предмет.
        // А в дальнейшем хочу сделать так, что NPC ворует ровно столько, чтобы восполнить параметры до максимума
        // В теории, это повысит частоту краж

        _ctx.transform.LookAt(_ctx.CurrentShelf.InteractionPoint);

        if (_ctx.CurrentShelf.CanTakeItem())
        {

            GameObject go = _ctx.CurrentShelf.PeekItem();
            if (go.TryGetComponent<IStoreable>(out var storeable))
            {
                _ctx.BoughtItems.Add(storeable);

                GameObject boughtGO = storeable.OnPickedFromStore();
                boughtGO.transform.position = _ctx.StorageForItems.position;
                boughtGO.transform.parent = _ctx.StorageForItems;

                Debug.Log($"[AI - {_ctx.gameObject.name} - StealingState]: steal storeable - {boughtGO.name}");
            }

            else Debug.LogWarning($"[AI - {_ctx.gameObject.name} - StealingState]: Item has no storeable component!");
        }
        else
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - StealingState]: Can't steal item. Shelf is empty. Leaving...");
        }

        // for now need this statement for debugging 
        if (notifyPlayer) Debug.Log($"[AI - {_ctx.gameObject.name} - StealingState]: Somebody is stealing LOUDLY!");
        else Debug.Log($"[AI - {_ctx.gameObject.name} - StealingState]: Somebody is stealing QUETLY!");

        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    public void Exit()
    {
        notifyPlayer = false;

        while (_ctx.BoughtItems.Count != 0)
        {
            _ctx.Psycho.IncreaseParameters(_ctx.BoughtItems[0].Data.FoodRestore, _ctx.BoughtItems[0].Data.ThirstRestore);
            var storeable = _ctx.BoughtItems[0];
            ((MonoBehaviour)storeable).gameObject.SetActive(false);
            _ctx.BoughtItems.RemoveAt(0);
        }

        // _ctx.RecordVisit(0f);
    }
}
