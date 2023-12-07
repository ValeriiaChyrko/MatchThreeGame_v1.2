using System.Collections;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    public class MovablePiece : MonoBehaviour
    {
        private Piece _piece;
        private IEnumerator moveCoroutine;

        private void Awake()
        {
            _piece = GetComponent<Piece>();
        }

        public void Move(int newX, int newY, float time)
        {
            if (moveCoroutine != null)
            {
               StopCoroutine(moveCoroutine);
            }

            moveCoroutine = MoveCoroutine(newX, newY, time);
            StartCoroutine(moveCoroutine);
        }

        private IEnumerator MoveCoroutine(int newX, int newY, float time)
        {
            _piece.X = newX;
            _piece.Y = newY;
            
            var startPos = transform.position;
            var endPos = _piece.GridRef.GetWorldPosition(newX, newY);

            for (float i = 0; i <= 1 * time; i += Time.deltaTime)
            {
                _piece.transform.position = Vector3.Lerp(startPos, endPos, i / time);
                yield return 0;
            }

            _piece.transform.position = endPos;
        }
    }
}
