using System;
using System.Collections.Generic;
using Core.EventSystem;
using Events.Board;
using Events.SP;
using MyHexBoardSystem.BoardSystem;
using Types.Board;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.UI.TraitHover {
    
    /// <summary>
    ///     This class highlights the traits affected by the next neuron placement
    /// </summary>
    public class MTraitHover : MonoBehaviour {
        [SerializeField] protected MTraitAccessor TraitAccessor;

        [Header("Highlighting"), SerializeField] protected TileBase positiveTile;
        [SerializeField] protected TileBase negativeTile;

        [Header("Event Managers"), SerializeField]
        protected SEventManager boardEventManager;
        [SerializeField] protected SEventManager storyEventManager;

        protected IStoryPoint CurrentSP;
        protected ETrait? CurrentHighlightedTrait;

        protected readonly HashSet<ETrait> CurrentPositive = new ();
        protected readonly HashSet<ETrait> CurrentNegative = new ();

        #region UnityMethods

        protected virtual void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassEnter, OnShow);
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassExit, OnHide);
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassHide, OnHide);
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
        }

        protected virtual void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassEnter, OnShow);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassExit, OnHide);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassHide, OnHide);
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
        }

        #endregion

        #region EventHandlers

        protected virtual void OnShow(EventArgs eventData) {
            if (eventData is not TraitCompassHoverEventArgs traitHoverArgs ||
                CurrentSP == null) {
                return;
            }

            var hoverTrait = traitHoverArgs.HighlightedTrait;
            // only highlight deciding traits
            if (!hoverTrait.HasValue || !CurrentSP.DecidingTraits.ContainsKey(hoverTrait.Value)) {
                return;
            }
            CacheHoverData(hoverTrait.Value);
            Show();
        }

        protected virtual void OnHide(EventArgs _) {
            if (!CurrentHighlightedTrait.HasValue) {
                return;
            }
            Hide();
            ClearHoverCache();
        }

        protected virtual void OnInitStory(EventArgs args) {
            if (args is not StoryEventArgs storyEventArgs) {
                return;
            }

            CurrentSP = storyEventArgs.Story;
        }
        
        #endregion

        protected virtual void CacheHoverData(ETrait hoverTrait) {
            CurrentHighlightedTrait = hoverTrait;
            var affectedTraits = CurrentSP.DecidingTraits[hoverTrait].BoardEffect;
            foreach (var trait in affectedTraits.Keys) {
                if (affectedTraits[trait] > 0) {
                    CurrentPositive.Add(trait);
                }
                else if (affectedTraits[trait] < 0) {
                    CurrentNegative.Add(trait);
                }
            }
        }

        private void ClearHoverCache() {
            CurrentHighlightedTrait = null;
            CurrentPositive.Clear();
            CurrentNegative.Clear();
        }

        protected virtual void Show() {
            if (!CurrentHighlightedTrait.HasValue) {
                return;
            }
            // TraitAccessor.SetTiles(_currentHighlightedTrait.Value, hoverTile, BoardConstants.TraitHoverTileLayer);
            foreach (var t in CurrentPositive) {
                TraitAccessor.SetTraitTiles(t, positiveTile, BoardConstants.SPEffectHoverTileLayer);
            }
            foreach (var t in CurrentNegative) {
                TraitAccessor.SetTraitTiles(t, negativeTile, BoardConstants.SPEffectHoverTileLayer);
            }
        }

        protected virtual void Hide() {
            if (!CurrentHighlightedTrait.HasValue) {
                return;
            }
            // TraitAccessor.SetTiles(_currentHighlightedTrait.Value, null, BoardConstants.TraitHoverTileLayer);
            foreach (var t in CurrentPositive) {
                TraitAccessor.SetTraitTiles(t, null, BoardConstants.SPEffectHoverTileLayer);
            }
            foreach (var t in CurrentNegative) {
                TraitAccessor.SetTraitTiles(t, null, BoardConstants.SPEffectHoverTileLayer);
            }
        }
    }
}