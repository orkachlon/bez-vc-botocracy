using ExternBoardSystem.BoardSystem.Position;
using UnityEngine;
using Grid = Grids.Grid;

namespace Neurons {
    public abstract class Neuron : MonoBehaviour, INeuron {

        public enum ENeuronType {
            Undefined,
            Invulnerable,
            Exploding,
            Expanding
        }

        public virtual ENeuronType Type => ENeuronType.Undefined;

        protected SpriteRenderer NeuronSprite;

        protected virtual void Awake() {
            NeuronSprite = GetComponent<SpriteRenderer>();
        }
        
        public void Rotate(bool directionRight) {
            var angle = Grid.GridType.Hex == Grid.Instance.Type ? 60f : 90f;
            if (directionRight) {
                // rotate right
                transform.Rotate(Vector3.back, angle);
                return;
            }
            // rotate left
            transform.Rotate(Vector3.back, -angle);
        }

        public void Hide() {
            NeuronSprite.enabled = false;
        }

        public void Show() {
            NeuronSprite.enabled = true;
        }

        public abstract int AllowedNeighbors();
    }
}