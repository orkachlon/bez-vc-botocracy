using System.Collections.Generic;
using System.Threading.Tasks;
using Types.Trait;
using UnityEngine;

namespace Types.StoryPoint {
    public interface IStoryPoint {

        #region Properties

        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public int TurnsToEvaluation { get; }
        public int Reward { get; }
        public IDictionary<ETrait, ITraitDecisionEffects> DecidingTraits { get; }
        public Sprite Artwork { get; }
        
        // for after SP evaluation
        public ITraitDecisionEffects DecisionEffects { get; }
        public bool Evaluated { get; }

        #endregion

        #region Methods

        void InitData(IStoryPointData spData);
        void RegisterOutcome(ISPProvider SPProvider);
        void Destroy();

        Task AwaitInitAnimation();
        Task AwaitRemoveAnimation();

        #endregion
    }
}