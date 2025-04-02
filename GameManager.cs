using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EventSystem eventSystem;
    public UIManager uiManager;
    public EventLoader eventLoader;

    private void Awake()
    {
        // 게임 시작 시, 필요한 시스템을 자동으로 찾음
        eventSystem = FindFirstObjectByType<EventSystem>();
        uiManager = FindFirstObjectByType<UIManager>();
        eventLoader = FindFirstObjectByType<EventLoader>();


        // 만약 오브젝트가 존재하지 않으면 경고 메시지 출력
        if (eventSystem == null) Debug.LogError("⚠️ EventSystem is missing!");
        if (uiManager == null) Debug.LogError("⚠️ UIManager is missing!");
        if (eventLoader == null) Debug.LogError("⚠️ EventLoader is missing!");

        Debug.Log("✅ GameManager Initialized Successfully!");
    }
} // <-- 클래스 닫는 중괄호가 반드시 있어야 함!