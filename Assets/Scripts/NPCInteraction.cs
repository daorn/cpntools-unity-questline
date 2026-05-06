using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    public enum NPCType
    {
        OldLady,
        ShadyCharacter,
        Item
    }

    [Header("NPC Settings")]
    public NPCType npcType;
    public float interactionRange = 2f;
    public string interactionPrompt = "Press E to interact";

    private Transform player;
    private PlayerInput playerInput;
    private bool playerInRange = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
        playerInput = playerObj.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;

        // Player just entered range
        if (playerInRange && !wasInRange)
            QuestUI.Instance.ShowPrompt(interactionPrompt);

        // Player just left range
        if (!playerInRange && wasInRange)
            QuestUI.Instance.HidePrompt();
    }

    // Called automatically by PlayerInput component via SendMessage
    // Make sure your Interact action is named "Interact" in the InputActionAsset
    public void OnInteract()
    {
        Debug.Log($"OnInteract called on {gameObject.name}, playerInRange: {playerInRange}");

        if (!playerInRange) return;
        if (QuestUI.Instance.IsDialogueOpen()) return;
        Interact();
    }

    private void Interact()
    {
        QuestManager qm = QuestManager.Instance;
        if (qm == null) return;

        switch (npcType)
        {
            case NPCType.OldLady:
                HandleOldLadyInteraction(qm);
                break;
            case NPCType.ShadyCharacter:
                HandleShadyCharacterInteraction(qm);
                break;
            case NPCType.Item:
                HandleItemInteraction(qm);
                break;
        }
    }

    private void HandleOldLadyInteraction(QuestManager qm)
    {
        switch (qm.CurrentState)
        {
            case QuestManager.QuestState.Inactive:
                qm.ApproachLady();
                QuestUI.Instance.ShowDialogue(
                    "Old Lady: Please, find my medicine and bring it to me!",
                    "Accept", "Refuse",
                    () => qm.AcceptQuest(),
                    () => qm.RefuseQuest()
                );
                break;

            case QuestManager.QuestState.Active:
                QuestUI.Instance.ShowMessage("Old Lady: Please hurry, find my medicine!");
                break;

            case QuestManager.QuestState.ItemFound:
                qm.ChooseOldLady();
                qm.DeliverToOldLady();
                QuestUI.Instance.ShowMessage("Old Lady: Thank you so much! You are very kind.");
                break;

            case QuestManager.QuestState.CompleteGood:
                QuestUI.Instance.ShowMessage("Old Lady: Thank you again, dear.");
                break;

            case QuestManager.QuestState.Refused:
                QuestUI.Instance.ShowMessage("Old Lady: *sighs* I understand... I'll manage somehow.");
                break;

            default:
                QuestUI.Instance.ShowMessage("Old Lady: ...");
                break;
        }
    }

    private void HandleShadyCharacterInteraction(QuestManager qm)
    {
        switch (qm.CurrentState)
        {
            case QuestManager.QuestState.ItemFound:
                QuestUI.Instance.ShowDialogue(
                    "Stranger: I'll pay you handsomely for that medicine...",
                    "Give it to him", "Refuse",
                    () => {
                        qm.ChooseShadyCharacter();
                        qm.DeliverToShadyCharacter();
                        QuestUI.Instance.ShowMessage(
                            "Stranger: Pleasure doing business. The old woman will have to manage without it."
                        );
                    },
                    () => QuestUI.Instance.ShowMessage("Stranger: Hmph. Your loss.")
                );
                break;

            case QuestManager.QuestState.CompleteBetrayal:
                QuestUI.Instance.ShowMessage("Stranger: We don't need to speak again.");
                break;

            default:
                QuestUI.Instance.ShowMessage("Stranger: ...");
                break;
        }
    }

    private void HandleItemInteraction(QuestManager qm)
    {
        if (qm.CurrentState == QuestManager.QuestState.Active)
        {
            qm.FindItem();
            QuestUI.Instance.ShowMessage("You picked up the medicine!");
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}