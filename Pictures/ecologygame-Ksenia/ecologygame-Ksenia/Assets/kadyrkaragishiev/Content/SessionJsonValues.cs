using System.Collections.Generic;
using System.Linq;
using kadyrkaragishiev.Dialogues;
using Newtonsoft.Json;

namespace kadyrkaragishiev.Content
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SessionJsonValues
    {
        [JsonProperty("player-name")] public string PlayerName;
        [JsonProperty("dialogues")] public Dictionary<string, string> Dialogues = new();

        [JsonProperty("ecologic-percent")] public int EcologicPercent;

        [JsonProperty("well-fare-percent")] public int WellFarePercent;

        public SessionJsonValues() { }

        public SessionJsonValues(Session s)
        {
            PlayerName = s.PlayerName;
            Dialogues = s.Dialogues.ToDictionary(x => x.Key.Id, x => x.Value.Id);
            EcologicPercent = s.EcologicPercent;
            WellFarePercent = s.WellFarePercent;
        }

        public Session Populate(Session s, List<DialogueScriptableStateMachine> dialogues)
        {
            var idToQuest = dialogues.ToDictionary(x => x.Id, x => x);
            var idToQuestState = dialogues.SelectMany(y => y.Transitions.SelectMany(x => new[] {x.From, x.To})).Distinct()
                .ToDictionary(x => x.Id, x => x);

            s.PlayerName = PlayerName;
            s.Dialogues = Dialogues.ToDictionary(x => idToQuest[x.Key], x => idToQuestState[x.Value]);
            s.EcologicPercent = EcologicPercent;
            s.WellFarePercent = WellFarePercent;

            return s;
        }
    }
}
