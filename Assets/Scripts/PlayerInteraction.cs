using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        // Find all NPCInteraction scripts in the scene and notify them
        NPCInteraction[] npcs = FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None);
        foreach (NPCInteraction npc in npcs)
        {
            npc.OnInteract();
        }
    }
}