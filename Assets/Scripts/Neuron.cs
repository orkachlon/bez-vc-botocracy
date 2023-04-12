using System;
using UnityEngine;
using Random = System.Random;

public class Neuron : MonoBehaviour {

    public enum NeuronType {
        Uni,
        Bi,
        Tri,
        Hex
    }

    public NeuronType Type { get; }

    private SpriteRenderer _neuronSprite;

    private void Awake() {
        _neuronSprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
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
        _neuronSprite.enabled = false;
    }

    public void Show() {
        _neuronSprite.enabled = true;
    }
}