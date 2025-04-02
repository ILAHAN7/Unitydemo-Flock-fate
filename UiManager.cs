using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Text eventTitle;
    public Text eventTitle2;
    public Text eventDescription;
    public Button optionLeft;
    public Button optionRight;
    public Text resultText;
    public Text resultDescriptionText;
    public Image eventBackground;
    public RectTransform cardPanel;

    private Vector3 originalCardPosition;
    private EventLoader eventLoader;

    public Text goldText;
    public Text reputationText;
    public Text noblesAffinityText;
    public Text dayText;
    public Text sheepText;

    public Player player;

    public GameObject dieImage;

    private bool isGameOver = false;

    // For sound effect
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip deathSound;

    // For button visual effect
    public Color buttonClickColor = Color.green; // Color to apply on click
    private Color originalButtonColor;
    public Sprite statIconImage;

    public Sprite goldSprite;
    public Sprite reputationSprite;
    public Sprite sheep;

    public Image backgroundImage; // 배경 이미지를 public으로 선언
    public Sprite daySprite;  // 낮 스프라이트
    public Sprite nightSprite;  // 밤 스프라이트

    public int dragCount = 0;  // 카드 드래그 횟수
    public int realDragCount = 0;
    private bool isDay = true;  // 낮/밤 상태

    private CanvasGroup eventDescriptionCanvasGroup; 

    private void Start()
    {
        if (cardPanel == null)
        {
            Debug.LogError("❌ cardPanel is missing! Assign it in the Inspector.");
            return;
        }

        eventLoader = FindObjectOfType<EventLoader>();

        if (eventLoader == null)
        {
            Debug.LogError("⚠️ EventLoader not found in scene!");
            return;
        }

        originalCardPosition = cardPanel.localPosition;

        optionLeft.onClick.AddListener(() => OnOptionSelected(0));
        optionRight.onClick.AddListener(() => OnOptionSelected(1));

        originalButtonColor = optionLeft.GetComponent<Image>().color;

        eventDescriptionCanvasGroup = eventDescription.GetComponent<CanvasGroup>();
        if (eventDescriptionCanvasGroup == null)
        {
            eventDescriptionCanvasGroup = eventDescription.gameObject.AddComponent<CanvasGroup>();
        }

        eventDescriptionCanvasGroup.alpha = 0;

        LoadRandomEvent();
    }

    void Update()
    {
        if (isGameOver)
            return;

        TextFile();
        CheckStatDeaths();
    }

    public void TextFile()
    {
        goldText.text = "GOLD : " + player.Gold.ToString();
        reputationText.text = "Reputation : " + player.Reputation.ToString();
        sheepText.text = "sheep : " + player.sheep.ToString();
        dayText.text = "Day : " + realDragCount.ToString();
    }

    private void CheckStatDeaths()
    {
        if (player.Gold <= 0 && !isGameOver)
        {
            HandleStatDeath("Gold", "Your Gold has run out! You are bankrupt!", goldSprite);
        }

        if (player.Reputation <= 0 && !isGameOver)
        {
            HandleStatDeath("Reputation", "Your reputation is destroyed! No one trusts you anymore!", reputationSprite);
        }

        if (player.NoblesAffinity <= 0 && !isGameOver)
        {
            HandleStatDeath("Nobles Affinity", "The Nobles no longer support you! You have lost their favor!", sheep);
        }
    }

    private void HandleStatDeath(string statName, string deathMessage, Sprite statSprite)
    {
        if (dieImage != null)
        {
            dieImage.SetActive(true);
        }

        goldText.text = "0";
        reputationText.text = "0";
        sheepText.text = "0";

        optionLeft.interactable = false;
        optionRight.interactable = false;

        Time.timeScale = 0f;

        Debug.Log(deathMessage);

        resultText.text = "DIE";
        resultDescriptionText.text = deathMessage;
        Destroy(eventBackground);
        isGameOver = true;

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }

    private void LoadRandomEvent()
    {
        if (eventLoader == null)
        {
            Debug.LogError("❌ eventLoader is null! Make sure it's assigned in the scene.");
            return;
        }

        var eventKeys = new List<string>(eventLoader.GetAllEventIDs());
        if (eventKeys.Count == 0)
        {
            Debug.LogError("🚨 No events found!");
            return;
        }

        string randomEventID = eventKeys[Random.Range(0, eventKeys.Count)];
        GameEvent gameEvent = eventLoader.GetEventByID(randomEventID);

        if (gameEvent == null)
        {
            Debug.LogError($"❌ GameEvent not found for EventID: {randomEventID}");
            return;
        }

        eventTitle.text = gameEvent.Title;
        eventTitle2.text = gameEvent.Title;
        eventDescription.text = gameEvent.Description;

        if (!string.IsNullOrEmpty(gameEvent.ImagePath))
        {
            Sprite eventImage = Resources.Load<Sprite>(gameEvent.ImagePath);
            if (eventImage != null)
            {
                eventBackground.sprite = eventImage;
                eventBackground.gameObject.SetActive(true); 
            }
            else
            {
                Debug.LogWarning($"Image not found at path: {gameEvent.ImagePath}");
                eventBackground.gameObject.SetActive(false); 
            }
        }

        if (gameEvent.Options != null && gameEvent.Options.Count >= 2)
        {
            optionLeft.GetComponentInChildren<Text>().text = gameEvent.Options[0].Description;
            optionRight.GetComponentInChildren<Text>().text = gameEvent.Options[1].Description;
        }
        StartCoroutine(FadeOutEventDescription(() =>
        {
            LoadEventAndFadeIn(gameEvent);
        }));
    }

    private void LoadEventAndFadeIn(GameEvent gameEvent)
    {
        eventTitle.text = gameEvent.Title;
        eventTitle2.text = gameEvent.Title;
        eventDescription.text = gameEvent.Description;
        if (!string.IsNullOrEmpty(gameEvent.ImagePath))
        {
            Sprite eventImage = Resources.Load<Sprite>(gameEvent.ImagePath);
            if (eventImage != null)
            {
                eventBackground.sprite = eventImage;
                eventBackground.gameObject.SetActive(true); 
            }
            else
            {
                Debug.LogWarning($"Image not found at path: {gameEvent.ImagePath}");
                eventBackground.gameObject.SetActive(false);  
            }
        }

        if (gameEvent.Options != null && gameEvent.Options.Count >= 2)
        {
            optionLeft.GetComponentInChildren<Text>().text = gameEvent.Options[0].Description;
            optionRight.GetComponentInChildren<Text>().text = gameEvent.Options[1].Description;
        }
        StartCoroutine(FadeInEventDescription());
    }

    private IEnumerator FadeOutEventDescription(System.Action onComplete)
    {
        float duration = 0.5f;
        float targetAlpha = 0f;
        float startAlpha = eventDescriptionCanvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            eventDescriptionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        eventDescriptionCanvasGroup.alpha = targetAlpha; 
        onComplete?.Invoke();
    }

    private IEnumerator FadeInEventDescription()
    {
        float duration = 1f;
        float targetAlpha = 1f;
        float startAlpha = eventDescriptionCanvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            eventDescriptionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        eventDescriptionCanvasGroup.alpha = targetAlpha;
    }


    private void ApplyChoice(int choiceIndex)
    {
        if (isGameOver) return;

        Debug.Log($"✔ Choice {choiceIndex} selected!");
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        StartCoroutine(ButtonClickEffect());
        StartCoroutine(FadeOutEventDescription());

        var eventKeys = new List<string>(eventLoader.GetAllEventIDs());
        if (eventKeys.Count == 0)
        {
            Debug.LogError("🚨 No events found!");
            return;
        }

        string randomEventID = eventKeys[Random.Range(0, eventKeys.Count)];
        GameEvent gameEvent = eventLoader.GetEventByID(randomEventID);

        if (gameEvent != null)
        {
            if (gameEvent.Options != null && gameEvent.Options.Count > choiceIndex)
            {
                var option = gameEvent.Options[choiceIndex];
                player.ChangeGold(option.GoldChange);
                player.ChangeReputation(option.ReputationChange);

                if (!string.IsNullOrEmpty(option.FollowUpEventID))
                {
                    Debug.Log($"Triggering follow-up event: {option.FollowUpEventID}");
                }
            }

            LoadRandomEvent();
        }

        TextFile(); 
    }

    private IEnumerator FadeOutEventDescription()
    {
        float duration = 0.5f;
        float targetAlpha = 0f;
        float startAlpha = eventDescriptionCanvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            eventDescriptionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        eventDescriptionCanvasGroup.alpha = targetAlpha; 
    }

    private IEnumerator ButtonClickEffect()
    {
        optionLeft.GetComponent<Image>().color = buttonClickColor;
        optionRight.GetComponent<Image>().color = buttonClickColor;

        yield return new WaitForSeconds(0.2f);

        optionLeft.GetComponent<Image>().color = originalButtonColor;
        optionRight.GetComponent<Image>().color = originalButtonColor;
    }

    private void OnOptionSelected(int optionIndex)
    {
        if (isGameOver) return;

        Debug.Log($"Option {optionIndex} selected!");
        ApplyChoice(optionIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cardPanel != null && !isGameOver)
        {
            cardPanel.position += new Vector3(eventData.delta.x, 0, 0);
        }
    }
    public void ButtonClick()
    {
        if (isGameOver) return;

        dragCount++;
        if (dragCount >= 3)
        {
            if (isDay)
            {
                SwitchToNight();
            }
            dragCount = 0;
        }
        else if (dragCount >= 2)
        {
            if (!isDay)
            {
                SwitchToDay();
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {


        float dragThreshold = 100f;
        if (cardPanel != null && Mathf.Abs(cardPanel.localPosition.x - originalCardPosition.x) > dragThreshold)
        {
            if (cardPanel.localPosition.x > originalCardPosition.x)
            {
                Debug.Log("👉 Swiped Right (Option B)");
                ApplyChoice(1);
            }
            else
            {
                Debug.Log("👈 Swiped Left (Option A)");
                ApplyChoice(0);
            }
        }
        StartCoroutine(ResetCardPosition());
    }

    private void SwitchToNight()
    {
        isDay = false;
        if (backgroundImage != null)
        {
            backgroundImage.sprite = nightSprite;  // 밤 스프라이트로 변경
        }
        Debug.Log("Switched to Night!");
    }

    private void SwitchToDay()
    {
        isDay = true;
        realDragCount++; // Increment day count only when switching to day
        if (backgroundImage != null)
        {
            backgroundImage.sprite = daySprite;  // 낮 스프라이트로 변경
        }
        Debug.Log("Switched to Day!");
    }

    private IEnumerator ResetCardPosition()
    {
        float duration = 0.3f;
        Vector3 startPosition = cardPanel.localPosition;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            if (cardPanel != null)
                cardPanel.localPosition = Vector3.Lerp(startPosition, originalCardPosition, t / duration);
            yield return null;
        }
    }
}
