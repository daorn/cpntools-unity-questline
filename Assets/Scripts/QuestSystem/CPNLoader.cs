using UnityEngine;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace QuestSystem {
    public class CPNLoader : MonoBehaviour {

        public string cpnFileName = "DAT355projectnet_v2.cpn";
        public PetriNet net;

        void Start() {
            string path = Application.streamingAssetsPath + "/" + cpnFileName;
            Debug.Log("Loading CPN from: " + path);
            net = LoadNet(path);
            if (net != null) {
                Debug.Log($"Loaded page '{net.pageName}': " +
                          $"{net.places.Count} places, {net.transitions.Count} transitions");
                foreach (var p in net.places)
                    Debug.Log(p.ToString());
                foreach (var t in net.transitions)
                    Debug.Log(t.ToString());
            }
        }

        public PetriNet LoadNet(string path) {
            PetriNet result = new PetriNet();
            XDocument doc   = XDocument.Load(path);

            // The .cpn file has NO namespace — use XName directly (no ns prefix)
            // Page name
            var page = doc.Descendants("page").FirstOrDefault();
            if (page != null)
                result.pageName = (string)page.Element("pageattr")?.Attribute("name") ?? "Unknown";

            // ── Places ──────────────────────────────────────────────────────────
            // Structure:
            //   <place id="ID...">
            //     <text>PlaceName</text>           ← direct child, the label
            //     <type ...><text ...>COLORTYPE</text></type>
            //     <initmark ...><text ...>1`value</text></initmark>
            //   </place>
            foreach (var p in doc.Descendants("place")) {
                Place place   = new Place();
                place.id      = (string)p.Attribute("id");

                // Name: the direct <text> child of <place> (not nested deeper)
                place.name    = (string)p.Elements("text").FirstOrDefault() ?? place.id;

                // Color type: inside <type><text>
                place.colorType = (string)p.Element("type")
                                   ?.Descendants("text").FirstOrDefault() ?? "";

                // Initial marking: inside <initmark><text>
                place.initMark  = (string)p.Element("initmark")
                                   ?.Descendants("text").FirstOrDefault() ?? "";

                // Set initial token count based on initMark
                // e.g. "1`true" → 1 token, "" → 0 tokens
                place.tokens = string.IsNullOrWhiteSpace(place.initMark) ? 0 : 1;

                result.places.Add(place);
            }

            // ── Transitions ──────────────────────────────────────────────────────
            // Structure:
            //   <trans id="ID...">
            //     <text>TransitionName</text>      ← direct child
            //     <cond ...><text ...>[guard]</text></cond>
            //   </trans>
            foreach (var t in doc.Descendants("trans")) {
                Transition trans = new Transition();
                trans.id         = (string)t.Attribute("id");
                trans.name       = (string)t.Elements("text").FirstOrDefault() ?? trans.id;
                trans.guard      = (string)t.Element("cond")
                                    ?.Descendants("text").FirstOrDefault() ?? "";
                result.transitions.Add(trans);
            }

            // ── Arcs ─────────────────────────────────────────────────────────────
            // Structure:
            //   <arc id="ID..." orientation="PtoT" or "TtoP">
            //     <transend idref="ID..."/>
            //     <placeend idref="ID..."/>
            //   </arc>
            foreach (var a in doc.Descendants("arc")) {
                string orientation = (string)a.Attribute("orientation");
                string placeId     = (string)a.Element("placeend")?.Attribute("idref");
                string transId     = (string)a.Element("transend")?.Attribute("idref");

                if (string.IsNullOrEmpty(placeId) || string.IsNullOrEmpty(transId)) {
                    Debug.LogWarning($"Arc {a.Attribute("id")} missing endpoint — skipping");
                    continue;
                }

                Place      place = result.GetPlace(placeId);
                Transition trans = result.GetTransition(transId);

                if (place == null) { Debug.LogWarning("Unknown place id: " + placeId); continue; }
                if (trans == null) { Debug.LogWarning("Unknown trans id: " + transId); continue; }

                if      (orientation == "PtoT") trans.inputs.Add(place);
                else if (orientation == "TtoP") trans.outputs.Add(place);
                else Debug.LogWarning("Unknown arc orientation: " + orientation);
            }

            return result;
        }
    }
}
