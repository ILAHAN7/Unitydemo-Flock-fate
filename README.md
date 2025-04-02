# Unitydemo-Flock-fate
------------------------------------------------------------------------------------------
                             +----------------------+
                             |     GameManager      |
                             | (Initializes game)   |
                             +----------+-----------+
                                        |
        +-------------------------------+-------------------------------+
        |                               |                               |
        v                               v                               v
+----------------------+     +----------------------+     +----------------------+
|     EventLoader       |     |      UIManager       |     |     EventSystem      |
| (Loads event.json →   |     | (Handles UI, drag,   |     | (Fetches events      |
|  Dictionary<string,   |     |  button logic, etc)  |     |  from EventLoader)   |
|      GameEvent>)      |     +----------+-----------+     +----------+-----------+
+----------+------------+                |                              |
           |                             |                              |
           v                             v                              v
+----------------------+     +----------------------+     +----------------------+
|     GameEvent         |<----|       Player         |<----|     EventOption      |
| (Title, Desc, Options,|     | (Stats: Gold, Rep,   |     | (Effect, FollowUpID) |
|  ImagePath, FollowUp) |     |  Affinities, Sheep)  |     +----------------------+
+----------+------------+     +----------------------+
           |
           v
+----------------------+
|     event.json       |
| (Raw event data)     |
+----------------------+
----------------------------------------------------------------------------------------------

1. GameManager.cs
Initializes the game.
Finds and links references to:
EventSystem
UIManager
EventLoader
If any reference is missing, it logs a warning.
Acts as the starting point of the game setup.

2. EventLoader.cs
Responsible for loading events from Resources/Event.json.
Deserializes JSON into a list of GameEvent objects.
Stores them in a dictionary using the event title as the key.
Provides:
---------------------------------------------------------
GetAllEventIDs() – returns all loaded event keys.
GetEventByID() – fetches a specific event by title.
----------------------------------------------------------
3. GameEvent.cs
Defines the structure of a single event:
----------------------------------------------------------
class GameEvent {
    string Title;
    string Description;
    List<EventOption> Options;
    string ImagePath;
    string FollowUpEventID;
}
Each event includes options (choices), each defined as:
---------------------------------------------------------
--------------------------------------------------------------
class EventOption {
    string Description;
    int GoldChange;
    int ReputationChange;
    int NoblesAffinityChange;
    ...
    string FollowUpEventID;
    
    void ApplyEffects(Player player);
}
------------------------------------------------------------
So, an option might give gold, reduce reputation, or trigger a follow-up event.

4. UIManager.cs
Handles all UI logic, including:
Displaying the current event.
Updating resource texts (gold, reputation, sheep, etc.).
Handling button clicks or swipe gestures to choose options.
Managing day/night transitions.
Checking for game-over conditions (e.g., if gold reaches 0).
Playing sounds and handling visual effects (fade in/out).

Workflow:
On button click or card swipe → ApplyChoice(int index) is called.
Effects are applied to the Player.
Loads a new event (follow-up or random).
Updates UI elements and handles animations.

5. Player.cs
Represents the player's stats:
Gold, Reputation, Affinities (Nobles, Merchant, Militia), and Sheep.
Methods to change those stats:
ChangeGold(), ChangeReputation(), ChangeAffinity(), etc.
Optionally updates the UI through UpdateUI().
Has a small hardcoded example of an event choice handler (HandleEventChoice()).

6. EventSystem.cs
A basic class that can fetch events via EventLoader.
Currently returns a hardcoded event ("RandomEvent1"), but can be extended.

7. event.json
The game’s actual event content, written in JSON format.
Each event includes:
Title, Description, Type, Options, and optional FollowUpEventID.
Supports branching narrative:
Some options lead to follow-up events like "TreasureHunt" or "GratefulTraveler".
Others can trigger game-ending events such as "WolfKingEnding"






