using UnityEngine;

namespace ExternBoardSystem.Ui.Particles
{
    public class UiHoverParticleSystem : MonoBehaviour, IHoverEffect
    {
        [SerializeField] private ParticleSystem[] particles;
        [SerializeField] private Renderer[] renderers;

        public void Show(int layer = -1)
        {
            if (layer > 0)
                foreach (var i in renderers)
                    i.sortingOrder = layer;
            foreach (var i in particles)
                i.Play();
        }

        public void Hide(int layer)
        {
            if (layer > 0)
                foreach (var i in renderers)
                    i.sortingOrder = layer;

            foreach (var i in particles)
            {
                i.Stop();
                i.Clear();
            }
        }

        public void Show(Vector3 position) {
            transform.position = position;
            Show();
        }

        public void Hide() {
            Hide(-1);
        }
    }
}