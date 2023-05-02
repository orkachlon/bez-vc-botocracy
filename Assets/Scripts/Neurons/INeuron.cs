namespace Neurons {
    public interface INeuron {
        public int AllowedNeighbors();
        public void Show();
        public void Hide();
        public void Rotate(bool directionRight);
    }
}