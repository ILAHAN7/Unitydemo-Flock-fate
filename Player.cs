using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int Gold;
    public int Reputation;
    public int NoblesAffinity;
    public int MerchantAffinity;
    public int MilitiaAffinity;
    public int sheep;

    public void ChangeGold(int amount)
    {
        Gold += amount;
    }

    public void ChangeReputation(int amount)
    {
        Reputation += amount;
    }

    public void ChangeAffinity(string group, int amount)
    {
        switch (group)
        {
            case "Nobles":
                NoblesAffinity += amount;
                break;
            case "MerchantGuild":
                MerchantAffinity += amount;
                break;
            case "Militia":
                MilitiaAffinity += amount;
                break;
            case "sheep":
                sheep += amount;
                break;
            default:
                break;
        }
    }

    public void UpdateUI(Text goldText, Text reputationText, Text noblesAffinityText, Text merchantAffinityText, Text militiaAffinityText, Text sheepText)
    {
        goldText.text = "Gold: " + Gold;
        reputationText.text = "Reputation: " + Reputation;
        noblesAffinityText.text = "Nobles Affinity: " + NoblesAffinity;
        merchantAffinityText.text = "Merchant Guild Affinity: " + MerchantAffinity;
        militiaAffinityText.text = "Militia Affinity: " + MilitiaAffinity;
        sheepText.text = "Sheep: " + sheep;
    }

    public void HandleEventChoice(int choiceIndex)
    {
        // Event details from JSON
        if (choiceIndex == 0) // Accept the map
        {
            ChangeGold(-10);       // -10 Gold
            ChangeReputation(5);   // +5 Reputation
            TriggerFollowUpEvent("TreasureHunt"); // Trigger follow-up event
        }
        else if (choiceIndex == 1) // Decline
        {
            ChangeReputation(-5);  // -5 Reputation
        }
    }

    private void TriggerFollowUpEvent(string eventID)
    {
        if (eventID == "TreasureHunt")
        {
            // Handle the Treasure Hunt event here
            Debug.Log("The treasure hunt event has been triggered.");
        }
    }
}
