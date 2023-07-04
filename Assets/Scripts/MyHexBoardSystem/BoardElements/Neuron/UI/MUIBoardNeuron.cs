using System.Threading.Tasks;
using Audio;
using DG.Tweening;
using ExternBoardSystem.Ui.Board;
using Types.Board;
using Types.Board.UI;
using Types.Neuron.Data;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron.UI {
    public class MUIBoardNeuron : MUIBoardElement, IUIBoardNeuron {
        
        protected INeuronDataBase NeuronData => RuntimeData.DataProvider as INeuronDataBase;
        
        [Header("Animation"), SerializeField] protected float removeAnimationDuration;
        [SerializeField] protected float addAnimationDuration;
        [SerializeField] protected float moveAnimationDuration;

        [Header("Sprites"), SerializeField] protected SpriteRenderer neuronFace;
        [SerializeField] protected string hoverSortingLayer;
        [SerializeField] protected string belowConnSortingLayer;
        [SerializeField] protected string aboveConnSortingLayer;
        
        [Header("Sound"), SerializeField, Range(0, 1)] protected float addVolume = 0.5f;
        [SerializeField, Range(0, 1)] protected float removeVolume = 0.5f;
        [SerializeField, Range(0, 1)] protected float moveVolume = 0.5f;

        #region UnityMethods

        protected virtual void OnDestroy() {
            StopHoverAnimation();
        }

        #endregion
        
        #region Layers

        public virtual void ToHoverLayer() {
            SpriteRenderer.sortingLayerName = hoverSortingLayer;
            neuronFace.sortingLayerName = hoverSortingLayer;
            neuronFace.sortingOrder = SpriteRenderer.sortingOrder + 1;
            transform.localScale = 1.2f * Vector3.one;
        }

        public virtual void ToBoardLayer() {
            SpriteRenderer.sortingLayerName = belowConnSortingLayer;
            neuronFace.sortingLayerName = aboveConnSortingLayer;
            neuronFace.sortingOrder = 0;
            transform.localScale = Vector3.one;
        }

        #endregion

        #region Animation

        public virtual async Task PlayRemoveAnimation() {
            transform.localScale = Vector3.one;
            await transform.DOScale(0, removeAnimationDuration).SetEase(Ease.InBack).AsyncWaitForCompletion();
        }

        public virtual async Task PlayAddAnimation() {
            transform.localScale = Vector3.zero;
            await transform.DOScale(1, addAnimationDuration).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        }

        public virtual Task PlayTurnAnimation()  => Task.CompletedTask;

        public virtual Task PlayHoverAnimation() => Task.CompletedTask;

        public virtual void StopHoverAnimation() { }
        public virtual Task PlayMoveAnimation(Vector3 fromPos, Vector3 toPos) => Task.CompletedTask;

        #endregion

        #region Sound

        public virtual void PlayAddSound() {
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = addVolume;
            s.Source.PlayOneShot(RuntimeData.DataProvider.GetAddSound());
            AudioSpawner.ReleaseWhenDone(s);
        }

        public virtual void PlayRemoveSound() {
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = removeVolume;
            s.Source.PlayOneShot(RuntimeData.DataProvider.GetRemoveSound());
            AudioSpawner.ReleaseWhenDone(s);
        }
        
        public virtual void PlayMoveSound() { }

        #endregion

        #region Pooling
        
        public override void SetRuntimeElementData(IBoardElement data) {
            Default();
            base.SetRuntimeElementData(data);
        }

        protected override void UpdateView() {
            base.UpdateView();
            neuronFace.sprite = NeuronData.GetFaceSprite();
        }
        
        public override void Default() {
            base.Default();
            StopHoverAnimation();
            transform.localScale = Vector3.one;
            SpriteRenderer.color = Color.white;
        }

        #endregion
    }
}