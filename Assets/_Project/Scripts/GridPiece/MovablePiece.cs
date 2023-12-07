using System;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    public class MovablePiece : MonoBehaviour
    {
        private Piece _piece;

        private void Awake()
        {
            _piece = GetComponent<Piece>();
        }

        public void Move(int newX, int newY)
        {
            _piece.X = newX;
            _piece.Y = newY;

            _piece.transform.localPosition = _piece.GridRef.GetWorldPosition(newX, newY);
        }
    }
}
