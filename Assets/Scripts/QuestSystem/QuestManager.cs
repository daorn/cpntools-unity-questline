using UnityEngine;
using System.Collections.Generic;

namespace QuestSystem {

    // Matches the CHOICE colset in your CPN model
    public enum DeliverChoice { OldLady, ShadyCharacter }

    public class QuestManager : MonoBehaviour {

        private PetriNet net;

        void Start() {
            var loader = GetComponent<CPNLoader>();
            if (loader == null) {
                Debug.LogError("QuestManager requires a CPNLoader on the same GameObject");
                return;
            }
            net = loader.net;
            PrintState();
        }

        // ── Public API — call these from your game UI / dialogue system ──────────

        /// Player walks up and talks to the old lady → fires ApproachLady
        public void ApproachLady() => FireByName("ApproachLady");

        /// Player accepts the quest → fires AcceptQuest
        public void AcceptQuest() => FireByName("AcceptQuest");

        /// Player refuses the quest → fires RefuseQuest
        public void RefuseQuest() => FireByName("RefuseQuest");
        
        /// Player refuses the quest for the last time → fires RefuseQuestFinal
        public void RefuseQuestFinal() => FireByName("RefuseQuestFinal");

        /// Player finds the item in the world → fires FindItem
        public void FindItem() => FireByName("FindItem");
        
        /// Player loses the item → fires LoseItem
        public void LoseItem() => FireByName("LoseItem");

        /// Player delivers to old lady → fires ChooseOldLady
        public void DeliverToOldLady() => FireByName("ChooseOldLady");

        /// Player delivers to shady character → fires ChooseShadyCharacter
        public void DeliverToShadyCharacter() => FireByName("ChooseShadyCharacter");

        /// Player delivers to old lady's contact → fires DeliverToOldLady
        public void CompleteDeliveryGood() => FireByName("DeliverToOldLady");

        /// Player delivers to shady character's contact → fires DeliverToShadyCharacter
        public void CompleteDeliveryBetrayal() => FireByName("DeliverToShadyCharacter");

        // ── State queries ─────────────────────────────────────────────────────────

        public bool IsQuestActive()        => TokensIn("QuestActive") > 0;
        public bool IsQuestFailed()        => TokensIn("QuestFailed") > 0;
        public bool IsQuestCompleteGood()  => TokensIn("QuestCompleteGood") > 0;
        public bool HasItem()              => TokensIn("ItemFound") > 0;
        public bool IsTalkingToLady()      => TokensIn("TalkingToLady") > 0;
        public bool IsQuestInProgress() => TokensIn("QuestActive") > 0 || TokensIn("ItemFound") > 0;

        public List<string> GetEnabledActionNames() {
            var names = new List<string>();
            foreach (var t in net.GetEnabledTransitions())
                names.Add(t.name);
            return names;
        }

        // ── Internal helpers ──────────────────────────────────────────────────────

        void FireByName(string transitionName) {
            var t = net.GetTransitionByName(transitionName);
            if (t == null) {
                Debug.LogWarning($"Transition '{transitionName}' not found in net");
                return;
            }
            if (!t.IsEnabled()) {
                Debug.LogWarning($"Transition '{transitionName}' is not enabled right now");
                return;
            }
            t.Fire();
            Debug.Log($"Fired: {transitionName}");
            PrintState();
        }

        int TokensIn(string placeName) {
            var p = net.GetPlaceByName(placeName);
            return p?.tokens ?? 0;
        }

        void PrintState() {
            Debug.Log("── Quest State ──────────────────");
            foreach (var p in net.places)
                if (p.tokens > 0)
                    Debug.Log($"  {p.name}: {p.tokens} token(s)");
            var enabled = GetEnabledActionNames();
            Debug.Log("Enabled actions: " + string.Join(", ", enabled));
            Debug.Log("─────────────────────────────────");
        }
    }
}
