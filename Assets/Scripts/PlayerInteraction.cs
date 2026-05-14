using QuestSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private GameObject questAcceptance;
    [SerializeField] private GameObject shadyInitDialogue;
    [SerializeField] private TextMeshProUGUI questStatus;
    
    private static bool inRangeGranny = false;
    private static bool inRangeItem = false;
    private static bool inRangeShady = false;
    
    private void OnInteract(InputValue value)
    {
        Debug.Log("Interact");
        if (inRangeGranny)
        {
            questManager.ApproachLady();
            questStatus.text = "Accept or refuse the quest";
            questAcceptance.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else if (inRangeItem)
        {
            questManager.FindItem();
            questStatus.text = "Item Found, deliver it or whatever";
            questManager.HasItem();
        } else if (inRangeShady)
        {
            shadyInitDialogue.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("OldLady"))
        {
            inRangeGranny = true;
            Debug.Log("Triggered");
        }
        else if (other.gameObject.CompareTag("Item"))
        {
            inRangeItem = true;
            Debug.Log("Triggered");
        }
        else if (other.gameObject.CompareTag("ShadyCharacter"))
        {
            inRangeShady = true;
            Debug.Log("Triggered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        inRangeGranny = false;
        inRangeItem = false;
        inRangeShady = false;
        Debug.Log("Unriggered");
    }
}
