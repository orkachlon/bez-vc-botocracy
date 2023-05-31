﻿using Main.MyHexBoardSystem.BoardElements;
using Main.StoryPoints.SPProviders;

namespace Main.StoryPoints {
    public interface IStoryPoint {

        #region Properties

        public string Description { get; }
        public int TurnsToEvaluation { get; }
        public int Reward { get; }
        public TraitDecisionEffects DecisionEffects { get; }
        public bool Evaluated { get; }

        #endregion

        #region Methods

        public void Decrement();
        public void Evaluate(IBoardNeuronController controller);
        public void InitData(StoryPointData spData);
        public void Destroy();

        #endregion
    }
}