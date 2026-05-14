using System.Collections.Generic;

namespace QuestSystem {
    public class Transition {
        public string id;
        public string name;
        public string guard;       // e.g. "[item = HasItem]", "[choice = OldLady]"
        public List<Place> inputs  = new List<Place>();
        public List<Place> outputs = new List<Place>();

        // A transition is enabled if all input places have at least 1 token
        // Guard evaluation is intentionally left simple here — extend as needed
        public bool IsEnabled() {
            foreach (var p in inputs)
                if (p.tokens < 1) return false;
            return true;
        }

        // Fire: consume one token from each input, produce one token in each output
        public void Fire() {
            foreach (var p in inputs)  p.tokens--;
            foreach (var p in outputs) p.tokens++;
        }

        public override string ToString() {
            return $"[Transition] {name} guard='{guard}' enabled={IsEnabled()}";
        }
    }
}
