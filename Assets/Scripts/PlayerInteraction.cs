using QuestSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private int refuseLimit = 2;
    [Header("Old Lady Status")]
    [SerializeField] private GameObject oldLadyAlive;
    [SerializeField] private GameObject oldLadyDead;
    [Header("Item")]
    [SerializeField] private GameObject item;
    [Header("UI Panels")]
    [SerializeField] private GameObject questAcceptance;
    [SerializeField] private GameObject deliverOldLadyPanel;
    [SerializeField] private GameObject shadyInitDialogue;
    [SerializeField] private GameObject deliverShadyPanel;
    [SerializeField] private GameObject promptMessage;
    [SerializeField] private GameObject endStatePanel;
    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI questStatus;
    [SerializeField] private TextMeshProUGUI refuseCount;
    [SerializeField] private TextMeshProUGUI endStateText;
    [SerializeField] private TextMeshProUGUI finalRefuseDialogue;

    private bool endStateReached = false;
    private int currentRefuseCount = 0;
    private static bool inRangeGranny = false;
    private static bool inRangeItem = false;
    private static bool inRangeShady = false;
    private static bool inRangeWell = false;
    
    public enum EndStateAlternatives
    {
        Refused, Lost, Good, Betrayal
    }

    private void Start()
    {
        oldLadyDead.SetActive(false);
        questAcceptance.SetActive(false);
        deliverOldLadyPanel.SetActive(false);
        shadyInitDialogue.SetActive(false);
        deliverShadyPanel.SetActive(false);
        promptMessage.SetActive(false);
        endStatePanel.SetActive(false);
        questStatus.text = "Approach the old lady";
        refuseCount.text = "Refusals: " + currentRefuseCount + "/" + refuseLimit;
    }

    private void OnInteract(InputValue value)
    {
        if (!endStateReached)
        {
            promptMessage.SetActive(false);
            Debug.Log("Interact");
            if (inRangeGranny)
            {
                if (!questManager.IsQuestInProgress())
                {
                    if (currentRefuseCount >= refuseLimit)
                    {
                        finalRefuseDialogue.text = "Old Lady:\nPlease, little one. I really really really need it";
                    }
                    questManager.ApproachLady();
                    questManager.IsTalkingToLady();
                    questStatus.text = "Accept or Refuse the quest from the Old Lady";
                    questAcceptance.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else if (questManager.HasItem())
                {
                    deliverOldLadyPanel.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else if (inRangeItem && questManager.IsQuestInProgress())
            {
                questManager.FindItem();
                questStatus.text = "Item Found, deliver it to someone";
                questManager.HasItem();
                Destroy(item);
            }
            else if (inRangeShady)
            {
                if (!questManager.HasItem())
                {
                    shadyInitDialogue.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    deliverShadyPanel.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else if (inRangeWell && questManager.HasItem())
            {
                questManager.LoseItem();
                EndState("Lost");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!endStateReached)
        {
            promptMessage.SetActive(true);

            if (other.gameObject.CompareTag("OldLady"))
            {
                inRangeGranny = true;
                Debug.Log("Old Lady Triggered");
            }
            else if (other.gameObject.CompareTag("Item"))
            {
                inRangeItem = true;
                Debug.Log("Item Triggered");
            }
            else if (other.gameObject.CompareTag("ShadyCharacter"))
            {
                inRangeShady = true;
                Debug.Log("Shady Triggered");
            }
            else if (other.gameObject.CompareTag("Well"))
            {
                inRangeWell = true;
                Debug.Log("Well Triggered");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        promptMessage.SetActive(false);
        inRangeGranny = false;
        inRangeItem = false;
        inRangeShady = false;
        inRangeWell = false;
        Debug.Log("Unriggered");
    }

    public void EndState(string alternative)
    {
        endStateReached = true;
        endStatePanel.SetActive(true);

        if (alternative == nameof(EndStateAlternatives.Refused))
        {
            questStatus.text = "Quest Failed";
            endStateText.text = "Quest Refused";
            endStateText.color = Color.red;
            OldLadyDied();
        }
        else if (alternative == nameof(EndStateAlternatives.Lost))
        {
            questStatus.text = "Quest Failed";
            endStateText.text = "Item Lost";
            endStateText.color = Color.red;
            OldLadyDied();
        }
        else if (alternative == nameof(EndStateAlternatives.Good))
        {
            questStatus.text = "Quest Completed";
            endStateText.text = "Good Ending";
            endStateText.color = Color.green;
        }
        else if (alternative == nameof(EndStateAlternatives.Betrayal))
        {
            questStatus.text = "Quest Completed";
            endStateText.text = "Betrayal Ending";
            endStateText.color = Color.green;
            OldLadyDied();
        }
    }

    public void RefuseQuest()
    {
        if (currentRefuseCount < refuseLimit)
        {
            currentRefuseCount++;
            questManager.RefuseQuest();
            refuseCount.text = "Refusals: " + currentRefuseCount + "/" + refuseLimit;
        }
        else
        {
            questManager.RefuseQuestFinal();
            EndState("Refused");
            refuseCount.color = Color.red;
        }
    }

    private void OldLadyDied()
    {
        oldLadyAlive.SetActive(false);
        oldLadyDead.SetActive(true);
    }
}
