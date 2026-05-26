## Иерархическая конечно-автоматная система (FSM AI)

В проекте реализована классическая архитектура **Finite State Machine (State Pattern)** для управления поведением покупателей (NPC) в симуляторе магазина. Система полностью развязана (decoupled): контроллер `NPCController` выступает в роли контекста, а логика состояний инкапсулирована в отдельных классах, наследующих интерфейс `INPCState`.

### Граф состояний ИИ (State Transition Diagram)

```mermaid
stateDiagram-v2
    [*] --> PickStoreState : Спавн и инициализация
    
    PickStoreState --> MovingToStore : Точка входа в магазин определена
    
    MovingToStore --> LeavingState : Магазин закрыт
    MovingToStore --> ShoppingState : Магазин открыт / NPC пришел
    
    ShoppingState --> LeavingState : Нужных товаров нет в наличии
    ShoppingState --> MovingToStorage : Список покупок сформирован
    
    MovingToStorage --> MovingToStorage : Переход к следующему стеллажу
    MovingToStorage --> LeavingState : Сделка отклонена / Закончились деньги
    MovingToStorage --> MovingToCheckout : Покупки завершены (Корзина полная/Стеллажи кончились)
    
    MovingToCheckout --> MovingToCheckout : Касса не найдена (Повторный поиск)
    MovingToCheckout --> BuyingState : Очередь на кассе достигнута
    
    BuyingState --> LeavingState : Товары оплачены или сброшены
    BuyingState --> BuyingState : Кассира нет (Ожидание)
    
    LeavingState --> [*] : Точка выхода достигнута / Деспавн
```
