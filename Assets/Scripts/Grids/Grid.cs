using Tiles;
using UnityEngine;

namespace Grids {
    public abstract class Grid : MonoBehaviour {
    
        [SerializeField] protected int width;
        [SerializeField] protected int height;
        [SerializeField] protected Vector3 origin = Vector3.zero;
        [SerializeField] protected Tile tilePrefab;

        private Vector3 _mouseOffsetForDrag;
    
        public static Grid Instance { get; private set; }
    
        public GridType Type { get; protected set; }

        public enum GridType {
            Square,
            Hex
        }

        protected virtual void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
        }

        protected virtual void Start() {
            Tile.OnTileDragEvent += DragGrid;
            Tile.OnTileMouseDownEvent += SetMouseOffsetForDrag;
        }
    
        protected virtual void OnDestroy() {
            Tile.OnTileDragEvent -= DragGrid;
            Tile.OnTileMouseDownEvent -= SetMouseOffsetForDrag;
        }

        private void SetMouseOffsetForDrag(Vector3 offset) {
            var currPosition = transform.position;
            _mouseOffsetForDrag = currPosition - new Vector3(offset.x, offset.y, currPosition.z);
        }

        private void DragGrid(Tile tile) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
            transform.position = mousePos + _mouseOffsetForDrag;
        }

        public abstract void CreateGrid();

        public abstract Tile GetNearestTile(Vector3 position);
    }
}