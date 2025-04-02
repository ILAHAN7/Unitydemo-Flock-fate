using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EventLoader : MonoBehaviour
{
    private Dictionary<string, GameEvent> eventDictionary;

    private void Awake()
    {
        LoadEventsFromJSON();
    }
    public List<string> GetAllEventIDs()
    {
        return new List<string>(eventDictionary.Keys);

    }
 private void LoadEventsFromJSON()
{
    TextAsset jsonFile = Resources.Load<TextAsset>("Event");
    if (jsonFile != null)
    {
        EventData eventData = JsonUtility.FromJson<EventData>(jsonFile.text);
        eventDictionary = new Dictionary<string, GameEvent>();

        foreach (GameEvent gameEvent in eventData.events)
        {
            // No need to convert to Sprite here, just store the image path
            eventDictionary[gameEvent.Title] = gameEvent;
        }
    }
    else
    {
        Debug.LogError("⚠️ Event JSON file not found!");
    }
}



    public GameEvent GetEventByID(string eventID)
    {
        return eventDictionary.ContainsKey(eventID) ? eventDictionary[eventID] : null;
    }
}

[System.Serializable]
public class EventData
{
    public List<GameEvent> events;
}