using System.Collections.Generic;

namespace QuestSystem {
    public class PetriNet {
        public string pageName;
        public List<Place>      places      = new List<Place>();
        public List<Transition> transitions = new List<Transition>();

        public Place GetPlace(string id) =>
            places.Find(p => p.id == id);

        public Place GetPlaceByName(string name) =>
            places.Find(p => p.name == name);

        public Transition GetTransition(string id) =>
            transitions.Find(t => t.id == id);

        public Transition GetTransitionByName(string name) =>
            transitions.Find(t => t.name == name);

        public List<Transition> GetEnabledTransitions() =>
            transitions.FindAll(t => t.IsEnabled());
    }
}
