using Core.Utils;
using DG.Tweening;
using System.Threading.Tasks;
using Types.Neuron.Data;
using Types.Neuron.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MyHexBoardSystem.BoardElements.Neuron.UI {
    public class MUIQueueNeuron : MonoBehaviour, IUIQueueNeuron {
        public Types.Neuron.Runtime.IStackNeuron RuntimeData { get; set; }
        public GameObject GO => gameObject;

        [Header("Animation"), SerializeField] protected float fadeDuration;


        protected INeuronDataBase NeuronData => RuntimeData.DataProvider;
        protected Image BaseImage { get; set; }
        protected Image FaceImage { get; set; }
        protected RectTransform RectTransform { get; set; }


        #region UnityMethods

        protected virtual void Awake() {
            RectTransform = GetComponent<RectTransform>();
            BaseImage = gameObject.FindComponentInChildWithTag<Image>("NeuronBaseRenderer");
            FaceImage = gameObject.FindComponentInChildWithTag<Image>("NeuronFaceRenderer");
        } 

        #endregion

        public virtual void SetRuntimeElementData(Types.Neuron.Runtime.IStackNeuron data) {
            RuntimeData = data;
            UpdateView();
        }

        public virtual void SetQueuePosition(float height) {
            var pos = RectTransform.anchoredPosition;
            RectTransform.anchoredPosition = new Vector2(pos.x, height);
        }

        protected virtual void UpdateView() {
            if (RuntimeData.PlaceInQueue > 2) {
                FaceImage.color = Color.clear;
                BaseImage.sprite = RuntimeData.DataProvider.GetQueueStackArtwork();
                return;
            }
            FaceImage.sprite = RuntimeData.DataProvider.GetFaceSprite();
            BaseImage.sprite = RuntimeData.DataProvider.GetBoardArtwork();
        }

        #region Animation

        public virtual Task PlayAnimation() => Task.CompletedTask;
        public virtual void StopAnimation() { }

        public virtual async Task AnimateDequeue() {
            await DOTween.Sequence()
                .Insert(0, RectTransform.DOAnchorPosY(RectTransform.anchoredPosition.y + 100, 0.5f).SetEase(Ease.OutCirc))
                .Insert(0, BaseImage.DOFade(0, fadeDuration).SetEase(Ease.OutCirc))
                .Insert(0, FaceImage.DOFade(0, fadeDuration).SetEase(Ease.OutCirc))
                .OnComplete(() => gameObject.SetActive(false))
                .AsyncWaitForCompletion();
        }

        public virtual async Task AnimateQueueShift(int queueIndex, int stackShiftAmount, int Top3ShiftAmount) {
            var shiftAmount = stackShiftAmount;
            if (queueIndex <= 2) {
                BaseImage.sprite = RuntimeData.DataProvider.GetBoardArtwork();
                FaceImage.sprite = RuntimeData.DataProvider.GetFaceSprite();
                FaceImage.color = Color.white;
                shiftAmount = Top3ShiftAmount;
            }

            await RectTransform
                .DOAnchorPosY(RectTransform.anchoredPosition.y + shiftAmount, 0.5f)
                .AsyncWaitForCompletion();
        }

        public virtual void Default() {
            BaseImage.sprite = RuntimeData.DataProvider.GetBoardArtwork();
            BaseImage.color = Color.white;
            FaceImage.sprite = RuntimeData.DataProvider.GetFaceSprite();
            FaceImage.color = Color.white;
            RectTransform.anchoredPosition = Vector2.zero;
        }

        #endregion
    }

}