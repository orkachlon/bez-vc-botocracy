using Main.MyHexBoardSystem.BoardSystem;
using UnityEngine;

namespace Main.Controls {
    public class CameraController : MonoBehaviour {

        [SerializeField] private float movementSpeed;
        [SerializeField] private MNeuronBoardController boardController;
        
        private Transform _camTransform;
        
        private void Awake() {
            _camTransform = GetComponent<Transform>();
        }

        private void Update() {
            var positionDelta = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) {
                positionDelta += Vector3.up * (movementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S)) {
                positionDelta += Vector3.down * (movementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A)) {
                positionDelta += Vector3.left * (movementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D)) {
                positionDelta += Vector3.right * (movementSpeed * Time.deltaTime);
            }

            // perform movement
            if (positionDelta == Vector3.zero) {
                return;
            }
            var farthestHexPos = boardController.HexToWorldPos(boardController.Manipulator.GetFarthestHex());
            if (farthestHexPos.magnitude <= (_camTransform.position + positionDelta).magnitude) {
                return;
            }
                
            _camTransform.position += positionDelta;
        }
    }
}