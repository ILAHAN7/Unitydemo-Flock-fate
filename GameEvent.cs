using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GameEvent
{
    public string Title;
    public string Description;
    public EventType Type;
    public List<EventOption> Options;
    public string  ImagePath; 

    public string OptionA; 
    public string OptionB; 
    public string FollowUpEventID; // 후속 이벤트 ID

    public GameEvent(string title, string description, EventType type, List<EventOption> options, string imagePath, string followUpEventID = null)
    {
        Title = title;
        Description = description;
        Type = type;
        Options = options;
        ImagePath = imagePath;
        FollowUpEventID = followUpEventID;
    }
}

[Serializable]
public class EventOption
{
    public string Description;
    public int GoldChange;
    public int ReputationChange;
    public int NoblesAffinityChange;
    public int MerchantAffinityChange;
    public int MilitiaAffinityChange;
    public string FollowUpEventID;

    public void ApplyEffects(Player player)
    {
        player.ChangeGold(GoldChange);
        player.ChangeReputation(ReputationChange);
        player.ChangeAffinity("Nobles", NoblesAffinityChange);
        player.ChangeAffinity("MerchantGuild", MerchantAffinityChange);
        player.ChangeAffinity("Militia", MilitiaAffinityChange);
    }
}