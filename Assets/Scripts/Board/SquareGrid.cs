using UnityEngine;

namespace Board {
    
    public class SquareGrid : MonoBehaviour {

        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private Vector3 origin = Vector3.zero;
        [SerializeField] private Tile tilePrefab;

        private Vector3 _mouseOffsetForDrag;

        private void Start() {
            Tile.TileDraggedEvent += DragGrid;
            Tile.OnTileMouseDownEvent += SetMouseOffsetForDrag;
            CreateGrid();
        }

        private void OnDestroy() {
            Tile.TileDraggedEvent -= DragGrid;
            Tile.OnTileMouseDownEvent -= SetMouseOffsetForDrag;
        }

        private void CreateGrid() {
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    var gridOffset = origin - new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
                    var tilePos = gridOffset +
                                  new Vector3(i, j);
                    var tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, transform);
                    tile.name = $"{i}, {j}";

                    if (i % 2 == 0 && j % 2 == 1 || i % 2 == 1 && j % 2 == 0) {
                        tile.TileColor = Color.black;
                    }

                    tile.Init();
                }
            }
        }

        private void SetMouseOffsetForDrag(Vector3 offset) {
            var currPosition = transform.position;
            _mouseOffsetForDrag = currPosition - new Vector3(offset.x, offset.y, currPosition.z);
        }

        private void DragGrid(Tile tile) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
            transform.position = mousePos + _mouseOffsetForDrag;
        }
    }
}