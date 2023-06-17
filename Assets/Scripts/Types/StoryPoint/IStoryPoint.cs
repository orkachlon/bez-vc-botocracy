using UnityEngine;

namespace Types.StoryPoint {
    public interface IStoryPoint {

        #region Properties

        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public int TurnsToEvaluation { get; }
        public int Reward { get; }
        public DecidingTraits DecidingTraits { get; }
        public Sprite Artwork { get; }
        
        // for after SP evaluation
        public TraitDecisionEffects DecisionEffects { get; }
        public bool Evaluated { get; }

        #endregion

        #region Methods

        public void InitData(StoryPointData spData);
        public void Destroy();

        #endregion
    }
}