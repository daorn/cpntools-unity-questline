using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance { get; private set; }

    [Header("Quest Status Panel")]
    public GameObject statusPanel;
    public TextMeshProUGUI questStatusText;
    public TextMeshProUGUI refusalCountText;

    [Header("Message Panel")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public Button messageCloseButton;

    [Header("Dialogue Panel")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button dialogueOptionA;
    public Button dialogueOptionB;
    public TextMeshProUGUI dialogueOptionAText;
    public TextMeshProUGUI dialogueOptionBText;

    [Header("Interaction Prompt")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        // Close buttons
        messageCloseButton.onClick.AddListener(HideMessage);

        // Hide everything at start
        HideAll();
    }

    private void Update()
    {
        // Update quest status every frame
        UpdateStatusPanel();

        // Close message with E or Escape
        if (messagePanel.activeSelf &&
            (Keyboard.current.escapeKey.wasPressedThisFrame ||
             Keyboard.current.eKey.wasPressedThisFrame))
        {
            HideMessage();
        }
    }

    // --- Status Panel ---

    private void UpdateStatusPanel()
    {
        if (QuestManager.Instance == null) return;

        questStatusText.text = "Quest: " + 
            QuestManager.Instance.CurrentState.ToString();

        int refusals = QuestManager.Instance.GetRefusalCount();
        int maxRefusals = QuestManager.Instance.GetMaxRefusals();

        if (QuestManager.Instance.CurrentState == 
            QuestManager.QuestState.Inactive ||
            QuestManager.Instance.CurrentState == 
            QuestManager.QuestState.TalkingToLady)
        {
            refusalCountText.text = $"Refusals: {refusals}/{maxRefusals}";
            refusalCountText.gameObject.SetActive(true);
        }
        else
        {
            refusalCountText.gameObject.SetActive(false);
        }
    }

    // --- Message Panel ---

    public void ShowMessage(string message)
    {
        HideDialogue();
        messageText.text = message;
        messagePanel.SetActive(true);
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
    }

    // --- Dialogue Panel ---

    public void ShowDialogue(
        string dialogue,
        string optionALabel,
        string optionBLabel,
        Action onOptionA,
        Action onOptionB)
    {
        HideMessage();

        dialogueText.text = dialogue;
        dialogueOptionAText.text = optionALabel;
        dialogueOptionBText.text = optionBLabel;

        // Clear previous listeners
        dialogueOptionA.onClick.RemoveAllListeners();
        dialogueOptionB.onClick.RemoveAllListeners();

        dialogueOptionA.onClick.AddListener(() =>
        {
            onOptionA?.Invoke();
            HideDialogue();
        });

        dialogueOptionB.onClick.AddListener(() =>
        {
            onOptionB?.Invoke();
            HideDialogue();
        });

        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
    
    public bool IsDialogueOpen()
    {
        return dialoguePanel.activeSelf;
    }

    // --- Interaction Prompt ---

    public void ShowPrompt(string message)
    {
        promptText.text = message;
        promptPanel.SetActive(true);
    }

    public void HidePrompt()
    {
        promptPanel.SetActive(false);
    }

    // --- Utility ---

    private void HideAll()
    {
        messagePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        promptPanel.SetActive(false);
    }
}