using System;
using UnityEngine;

public class Neuron : MonoBehaviour {

    public enum NeuronType {
        Uni,
        Bi,
        Tri,
        Hex
    }

    public NeuronType Type { get; }

    private void Update() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            Rotate(true);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            Rotate(false);
        }
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
}