using System;
using UnityEngine;

public abstract class Tile : MonoBehaviour {

    public static Action<Tile> OnTileDragEvent;
    public static Action<Vector3> OnTileMouseDownEvent;
    public static Action<Tile> OnTileClickedEvent;

    [SerializeField] private float mouseClickThreshold;
    
    private float _mouseDownTime;

    protected Neuron Occupant = null;
    protected SpriteRenderer SpriteRenderer;
    
    public Color TileColor { get; set; }
    public float Radius { get; private set; }
    public float Width { get; private set; }

    protected virtual void Awake() {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Radius = SpriteRenderer.size.y / 2;
        Width = SpriteRenderer.size.x;
        TileColor = Color.white;
    }

    private void OnMouseEnter() {
        // todo: fix highlighting
        SpriteRenderer.color = Color.blue;
    }

    private void OnMouseExit() {
        SpriteRenderer.color = TileColor;
    }

    private void OnMouseDrag() {
        OnTileDragEvent?.Invoke(this);
    }

    private void OnMouseDown() {
        _mouseDownTime = Time.time;
        // todo: remove camera reference somehow (maybe with a utils class)
        OnTileMouseDownEvent?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void OnMouseUpAsButton() {
        if (Time.time - _mouseDownTime > mouseClickThreshold) {
            // not a click
            return;
        }

        if (IsEmpty()) {
            Occupant = Instantiate(NeuronManager.Instance.GetNextNeuron(), transform.position, Quaternion.identity, transform);
        }
        else {
            Debug.Log("Tile is occupied!");
        }
        OnTileClickedEvent?.Invoke(this);
    }

    public void Init() {
        SpriteRenderer.color = TileColor;
    }
    
    public bool IsEmpty() {
        return Occupant == null;
    }
}