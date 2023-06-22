using System.Threading.Tasks;
using DG.Tweening;
using ExternBoardSystem.Ui.Board;
using Types.Board;
using Types.Board.UI;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron.UI {
    public class MUIBoardNeuron : MUIBoardElement, IUIBoardNeuron {
        
        public AudioSource Source { get; private set; }

        [Header("Sorting orders"), SerializeField]
        protected int hoverSortingOrder;
        [SerializeField] protected int boardSortingOrder;

        [Header("Animation"), SerializeField] private float removeAnimationDuration;
        [SerializeField] private float addAnimationDuration;
        [SerializeField] private float moveAnimationDuration;

        protected override void Awake() {
            base.Awake();
            Source = GetComponent<AudioSource>();
        }

        public override void SetRuntimeElementData(IBoardElement data) {
            Default();
            base.SetRuntimeElementData(data);
        }

        public virtual void ToHoverLayer() {
            SpriteRenderer.sortingOrder = hoverSortingOrder;
            transform.localScale = 1.2f * Vector3.one;
        }

        public virtual void ToBoardLayer() {
            SpriteRenderer.sortingOrder = boardSortingOrder;
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
        public async Task PlayMoveAnimation(Vector3 fromPos, Vector3 toPos) {
            transform.DOMove(toPos, 0.25f);
            await transform.DOScale(0.5f, 0.1f).SetLoops(2, LoopType.Yoyo).AsyncWaitForCompletion();
        }

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
            SpriteRenderer.sortingOrder = boardSortingOrder;
        }
    }
}