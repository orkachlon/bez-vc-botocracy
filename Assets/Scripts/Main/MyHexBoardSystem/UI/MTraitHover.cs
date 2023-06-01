using System;
using ExternBoardSystem.Tools.Input.Mouse;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.Neurons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.UI {
    
    /// <summary>
    ///     This class highlights the traits affected by the next neuron placement
    /// </summary>
    [RequireComponent(typeof(IMouseInput)), RequireComponent(typeof(MTraitAccessor))]
    public class MTraitHover : MonoBehaviour {
        
        [Header("Current Neuron"), SerializeField]
        private SNeuronData currentNeuron;

        [Header("Tilemap and Tiles"), SerializeField]
        private TileBase hoverTile;

        private IMouseInput _mouseInput;
        private Camera _cam;
        private ITraitAccessor _traitAccessor;

        private void Awake() {
            _cam = Camera.main;
            _mouseInput = GetComponent<IMouseInput>();
        }

        private void OnEnable() {
            _mouseInput.OnPointerEnter += Show;
        }

        private void OnDisable() {
            _mouseInput.OnPointerEnter -= Show;
        }

        // todo consider checking if we are already highlighting this trait
        private void Show(PointerEventData eventData) {
            // when board is disabled don't show this effect
            if (ENeuronType.Undefined.Equals(currentNeuron.Type)) {
                return;
            }
            // get mouse world pos
            var mouseWorldPos = _cam.ScreenToWorldPoint(_mouseInput.MousePosition);
            // figure out which trait the hex is in
            var trait = _traitAccessor.WorldPosToTrait(mouseWorldPos);

            // show outline of that trait
            _traitAccessor.SetTiles(trait, hoverTile, BoardConstants.TraitHoverTileLayer);
        }
    }
}