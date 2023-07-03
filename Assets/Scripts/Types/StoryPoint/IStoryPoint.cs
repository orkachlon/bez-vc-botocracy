using System.Threading.Tasks;
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

        void InitData(StoryPointData spData);
        void RegisterOutcome(ISPProvider SPProvider);
        void Destroy();

        Task AwaitInitAnimation();
        Task AwaitRemoveAnimation();

        #endregion
    }
}