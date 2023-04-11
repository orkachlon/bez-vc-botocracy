using UnityEngine;

public class SquareGrid : Grid {
    
    protected override void Start() {
        base.Start();
        Type = GridType.Square;
        // CreateGrid();
    }

    public override void CreateGrid() {
        for (var i = 0; i < height; i++) {
            for (var j = 0; j < width; j++) {
                // offset the grid from the origin to center it
                var gridOffset = origin - new Vector3((float)width / 2, (float)height / 2);
                var tile = Instantiate(tilePrefab, gridOffset, Quaternion.identity, transform);
                // place tile in grid position
                tile.transform.position += new Vector3(
                    2 * tile.Radius * j + tile.Radius,
                    2 * tile.Radius * i + tile.Radius
                    );
                tile.name = $"{i}, {j}";

                if ((i + j) % 2 == 1) {
                    tile.TileColor = Color.black;
                }

                tile.Init();
            }
        }
    }
}