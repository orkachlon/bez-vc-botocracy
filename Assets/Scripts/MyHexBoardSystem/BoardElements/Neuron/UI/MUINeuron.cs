using Core.Utils;
using DG.Tweening;
using System.Threading.Tasks;
using Types.Neuron.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MyHexBoardSystem.BoardElements.Neuron.UI {
    public class MUINeuron : MonoBehaviour, IUINeuron {
        public Types.Neuron.Runtime.IStackNeuron RuntimeData { get; set; }
        protected Image BaseImage { get; set; }
        protected Image FaceImage { get; set; }
        protected RectTransform RectTransform { get; set; }

        protected virtual void Awake() {
            RectTransform = GetComponent<RectTransform>();
            BaseImage = gameObject.FindComponentInChildWithTag<Image>("NeuronBaseRenderer");
            FaceImage = gameObject.FindComponentInChildWithTag<Image>("NeuronFaceRenderer");
        }

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

        public async Task AnimateDequeue() {
            await DOTween.Sequence()
                .Insert(0, RectTransform.DOAnchorPosY(RectTransform.anchoredPosition.y + 100, 0.5f).SetEase(Ease.OutCirc))
                .Insert(0, BaseImage.DOFade(0, 0.5f).SetEase(Ease.OutCirc))
                .Insert(0, FaceImage.DOFade(0, 0.5f).SetEase(Ease.OutCirc))
                .OnComplete(() => gameObject.SetActive(false))
                .AsyncWaitForCompletion();
        }

        public async Task AnimateQueueShift(int queueIndex) {
            var shiftAmount = 50;
            if (queueIndex <= 2) {
                BaseImage.sprite = RuntimeData.DataProvider.GetBoardArtwork();
                FaceImage.sprite = RuntimeData.DataProvider.GetFaceSprite();
                FaceImage.color = Color.white;
                shiftAmount = 100;
            }

            RuntimeData.PlaceInQueue = queueIndex;
            await RectTransform
                .DOAnchorPosY(RectTransform.anchoredPosition.y + shiftAmount, 0.5f)
                .AsyncWaitForCompletion();
        }
    }

}