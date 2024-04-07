using System.Collections.Generic;
using Types.Trait;

namespace Types.StoryPoint {
    public interface ITraitDecisionEffects {
        public ETrait DecidingTrait { get; set; }
        public int OutcomeID { get; set; }
        public string Decision { get; set; }
        public string Outcome { get; set; }
        public string OutcomeModification { get; set; }
        public Dictionary<ETrait, int> BoardEffect { get; set; }
    }
}
