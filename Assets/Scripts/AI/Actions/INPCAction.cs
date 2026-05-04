using UnityEngine;

public interface INPCAction
{
    string Name { get; }

    // Можно ли выполнить прямо сейчас? (предусловия)
    bool CanExecute(MonoBehaviour npc);

    // Старт действия (один раз)
    void OnStart(MonoBehaviour npc);

    // Каждый кадр. Возвращает true, если действие завершено
    bool OnUpdate(MonoBehaviour npc);

    // Принудительная отмена (например, магазин закрылся)
    void OnCancel(MonoBehaviour npc);

    // Успешное завершение
    void OnComplete(MonoBehaviour npc);
}