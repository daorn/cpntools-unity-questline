using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public enum QuestState
    {
        Inactive, 
        TalkingToLady, 
        Active, 
        ItemFound, 
        DeliverChoice, 
        CompleteGood, 
        CompleteBetrayal, 
        Failed, 
        Refused
    }
    
    public QuestState CurrentState { get; private set; } = QuestState.Inactive;
    
    private int refusalCount = 0;
    private const int MAX_REFUSALS = 2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public bool ApproachLady()
    {
        if (CurrentState != QuestState.Inactive)
            return false;
        CurrentState = QuestState.TalkingToLady;
        Debug.Log("Quest: Talking to Old Lady");
        return true;
    }

    public bool AcceptQuest()
    {
        if (CurrentState != QuestState.TalkingToLady)
            return false;
        CurrentState = QuestState.Active;
        Debug.Log("Quest: Accepted");
        return true;
    }

    public bool RefuseQuest()
    {
        if (CurrentState != QuestState.TalkingToLady) return false;

        if (refusalCount >= MAX_REFUSALS)
        {
            CurrentState = QuestState.Refused;
            Debug.Log("Quest: Refused permanently");
        }
        else
        {
            refusalCount++;
            CurrentState = QuestState.Inactive;
            Debug.Log($"Quest: Refused ({refusalCount}/{MAX_REFUSALS})");
        }
        return true;
    }

    public bool FindItem()
    {
        if (CurrentState != QuestState.Active)
            return false;
        CurrentState = QuestState.ItemFound;
        Debug.Log("Quest: Item Found");
        return true;
    }

    public bool ItemLost()
    {
        if (CurrentState != QuestState.ItemFound)
            return false;
        CurrentState = QuestState.Failed;
        Debug.Log("Quest: Item Lost - quest failed");
        return true;
    }

    public bool ChooseOldLady()
    {
        if (CurrentState != QuestState.ItemFound)
            return false;
        CurrentState = QuestState.DeliverChoice;
        Debug.Log("Quest: Chose to deliver to Old Lady");
        return true;
    }

    public bool ChooseShadyCharacter()
    {
        if (CurrentState != QuestState.ItemFound)
            return false;
        CurrentState = QuestState.DeliverChoice;
        Debug.Log("Quest: Chose to deliver to Shady Character");
        return true;
    }
    
    public bool DeliverToOldLady()
    {
        if (CurrentState != QuestState.DeliverChoice) return false;
        CurrentState = QuestState.CompleteGood;
        Debug.Log("Quest: Delivered to Old Lady - Good ending!");
        return true;
    }

    public bool DeliverToShadyCharacter()
    {
        if (CurrentState != QuestState.DeliverChoice) return false;
        CurrentState = QuestState.CompleteBetrayal;
        Debug.Log("Quest: Delivered to Shady Character - Betrayal ending!");
        return true;
    }

    public int GetRefusalCount() => refusalCount;
    public int GetMaxRefusals() => MAX_REFUSALS;
}
