namespace QuestSystem {
    public class Place {
        public string id;
        public string name;
        public string colorType;   // e.g. BOOL, ITEM, STATUS, CHOICE, REFUSALS, LADY
        public string initMark;    // e.g. "1`true", "1`0", "1`Alive", "" for empty
        public int tokens;         // simplified token count for plain firing

        public override string ToString() {
            return $"[Place] {name} ({colorType}) tokens={tokens}";
        }
    }
}
