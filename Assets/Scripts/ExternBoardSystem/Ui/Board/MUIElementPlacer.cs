﻿using System;
using Core.EventSystem;
using ExternBoardSystem.Events;
using Types.Board;
using Types.Board.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board {
    public abstract class MUIElementPlacer<T, TUI> : MonoBehaviour 
        where T : IBoardElement
        where TUI : IUIBoardElement {
        
        [Header("Event Managers"), SerializeField] private SEventManager innerBoardEventManager;

        protected Tilemap TileMap { get; private set; }
        
        protected virtual void Awake() {
            TileMap = GetComponent<Tilemap>();
        }

        protected virtual void OnEnable() {
            innerBoardEventManager.Register(InnerBoardEvents.OnElementAdded, OnAddElement);
            innerBoardEventManager.Register(InnerBoardEvents.OnElementRemoved, OnRemoveElement);
            innerBoardEventManager.Register(InnerBoardEvents.OnElementMoved, OnMoveElement);
            innerBoardEventManager.Register(InnerBoardEvents.OnCreateBoard, OnCreateBoard);
        }

        protected virtual void OnDisable() {
            innerBoardEventManager.Unregister(InnerBoardEvents.OnElementAdded, OnAddElement);
            innerBoardEventManager.Unregister(InnerBoardEvents.OnElementRemoved, OnRemoveElement);
            innerBoardEventManager.Unregister(InnerBoardEvents.OnCreateBoard, OnCreateBoard);
        }

        #region EventHandlers

        protected virtual void OnAddElement(EventArgs eventData) {
            if (eventData is OnElementEventData<T> elementEventData) {
                OnAddElement(elementEventData.Element, elementEventData.Cell);
            }
        }
        
        protected virtual void OnCreateBoard(EventArgs eventData) {
            if (eventData is OnBoardEventData<T> boardEventData) {
                OnCreateBoard(boardEventData.Board);
            }
        }

        protected virtual void OnRemoveElement(EventArgs eventData) {
            if (eventData is OnElementEventData<T> elementEventData) {
                OnRemoveElement(elementEventData.Element, elementEventData.Cell);
            }
        }
        
        protected virtual void OnMoveElement(EventArgs eventData) {
            if (eventData is OnElementMovedEventData<T> elementEventData) {
                OnMoveElement(elementEventData.Element, elementEventData.Cell, elementEventData.ToCell);
            }
        }

        #endregion

        protected abstract void OnCreateBoard(IBoard<T> board);
        protected abstract void OnRemoveElement(T element, Vector3Int cell);
        protected abstract void OnAddElement(T element, Vector3Int cell);
        protected abstract void OnMoveElement(T element, Vector3Int fromCell, Vector3Int toCell);
    }
}