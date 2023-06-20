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
            await transform.DOScale(0, 0.4f).SetEase(Ease.InBack).AsyncWaitForCompletion();
        }

        public virtual async Task PlayAddAnimation() {
            transform.localScale = Vector3.zero;
            await transform.DOScale(1, 0.4f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        }

        public virtual Task PlayTurnAnimation()  => Task.CompletedTask;

        public virtual Task PlayHoverAnimation() => Task.CompletedTask;

        public virtual void StopHoverAnimation() { }
        public async Task PlayMoveAnimation() {
            await transform.DOScale(0.5f, 0.25f).SetLoops(2, LoopType.Yoyo).AsyncWaitForCompletion();
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