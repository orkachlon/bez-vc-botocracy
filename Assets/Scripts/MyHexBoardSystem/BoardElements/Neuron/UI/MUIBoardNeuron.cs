using System.Threading.Tasks;
using DG.Tweening;
using ExternBoardSystem.Ui.Board;
using Types.Board;
using Types.Board.UI;
using Types.Neuron.Data;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron.UI {
    public class MUIBoardNeuron : MUIBoardElement, IUIBoardNeuron {
        
        public AudioSource Source { get; private set; }
        protected INeuronDataBase NeuronData => RuntimeData.DataProvider as INeuronDataBase;
        
        [Header("Animation"), SerializeField] protected float removeAnimationDuration;
        [SerializeField] protected float addAnimationDuration;
        [SerializeField] protected float moveAnimationDuration;

        [Header("Sprites"), SerializeField] protected SpriteRenderer neuronFace;
        [SerializeField] protected string hoverSortingLayer;
        [SerializeField] protected string belowConnSortingLayer;
        [SerializeField] protected string aboveConnSortingLayer;

        protected override void Awake() {
            base.Awake();
            Source = GetComponent<AudioSource>();
        }

        public override void SetRuntimeElementData(IBoardElement data) {
            Default();
            base.SetRuntimeElementData(data);
        }

        protected override void UpdateView() {
            base.UpdateView();
            neuronFace.sprite = NeuronData.GetFaceSprite();
        }

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

        public void PlayAddSound() {
            Source.PlayOneShot(RuntimeData.DataProvider.GetAddSound());
        }

        public void PlayRemoveSound() {
            Source.PlayOneShot(RuntimeData.DataProvider.GetRemoveSound());
        }

        public override void Default() {
            base.Default();
            StopHoverAnimation();
            transform.localScale = Vector3.one;
            SpriteRenderer.color = Color.white;
        }
    }
}