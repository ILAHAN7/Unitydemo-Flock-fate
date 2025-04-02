using UnityEngine;

public class EventSystem : MonoBehaviour
{
    private EventLoader eventLoader;

    private void Start()
    {
        eventLoader = FindFirstObjectByType<EventLoader>();
    }

    public GameEvent GenerateRandomEvent()
    {
        return eventLoader.GetEventByID("RandomEvent1"); // 랜덤 이벤트 ID를 JSON에서 가져옴
    }
}