using System;
using MatchThreeGame._Project.Scripts.Enums;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    public class Piece : MonoBehaviour
    {
        private int _x;
        public int X
        {
            get => _x;
            set
            {
                if (IsMovable())
                {
                    _x = value;
                }
            }
        }
        
        private int _y;

        public int Y
        {
            get => _y;
            set
            {
                if (IsMovable())
                {
                    _y = value;
                }
            }
        }

        public PieceType Type { get; private set; }
        public GridController GridRef { get; private set; }

        public MovablePiece MovablePiece { get; private set; }
        public bool IsMovable() => MovablePiece != null;
        public ColourPiece ColourPiece { get; private set; }
        public bool IsColour() => ColourPiece != null;

        private void Awake()
        {
            MovablePiece = GetComponent<MovablePiece>();
            ColourPiece = GetComponent<ColourPiece>();
        }

        public void Init(int x, int y, PieceType type, GridController gridRef)
        {
            X = x;
            Y = y;
            Type = type;
            GridRef = gridRef;
        }

        private void OnMouseEnter()
        {
            Debug.Log("Destination" + _x + ',' + _y);
            GridRef.Destination(this);
        }

        private void OnMouseUp()
        {
            GridRef.Release();
        }

        private void OnMouseDown()
        {
            Debug.Log("Source" + _x + ',' + _y);
            GridRef.Source(this);
        }
    }
}
