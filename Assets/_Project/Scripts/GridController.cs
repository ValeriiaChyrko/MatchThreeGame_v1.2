using System.Collections;
using System.Collections.Generic;
using MatchThreeGame._Project.Scripts.Enums;
using MatchThreeGame._Project.Scripts.GridPiece;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private int xDim;
        [SerializeField] private int yDim;
        [SerializeField] private float fillTime;
        
        [SerializeField] private PiecePrefab[] piecePrefabs;
        [SerializeField] private GameObject backgroundPrefab;

        private Dictionary<PieceType, GameObject> _piecePrefabDictionary;
        private Piece[,] _pieces;
        private bool _inverse;

        private Piece _source;
        private Piece _destination;

        private void Start()
        {
            _piecePrefabDictionary = new Dictionary<PieceType, GameObject>();

            foreach (var t in piecePrefabs)
                _piecePrefabDictionary.TryAdd(t.type, t.prefab);

            for (var i = 0; i < xDim; i++)
            {
                for (var j = 0; j < yDim; j++)
                {
                    var background = Instantiate(backgroundPrefab, GetWorldPosition(i, j), Quaternion.identity);
                    background.transform.parent = transform;
                }
            }

            _pieces = new Piece[xDim, yDim];
            for (var i = 0; i < xDim; i++)
                for (var j = 0; j < yDim; j++)
                    SpawnNewPiece(i, j, PieceType.EMPTY);
            
            Destroy(_pieces[2, 2].gameObject);
            SpawnNewPiece(2, 2, PieceType.OBSTACLE);
            
            StartCoroutine(Fill());
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            var position = transform.position;
            return new Vector2(position.x - xDim / 2.0f + x, position.y + yDim / 2.0f - y);
        }

        private void SpawnNewPiece(int x, int y, PieceType type)
        {
            var newPiece = Instantiate(_piecePrefabDictionary[type], GetWorldPosition(x, y), Quaternion.identity);
            newPiece.transform.parent = transform;

            _pieces[x, y] = newPiece.GetComponent<Piece>();
            _pieces[x, y].Init(x, y, type, this);
        }

        private IEnumerator Fill()
        {
            while (FillStep())
            {
                _inverse = !_inverse;
                yield return new WaitForSeconds(fillTime);
            }
        }

        private bool FillStep()
        {
            var movePiece = false;

            for (var y = yDim - 2; y >= 0; y--)
            {
                for (var loopX = 0; loopX < xDim; loopX++)
                {
                    var x = loopX;
                    if (_inverse) x = xDim - 1 - loopX;

                    var piece = _pieces[x, y];

                    if (!piece.IsMovable()) continue;
                    var pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);

                        piece.MovablePiece.Move(x, y + 1, fillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);

                        movePiece = true;
                    } 
                    else 
                    {
                        for (var diag = -1; diag <= 1; diag++)
                        {
                            if (diag == 0) continue;
                                
                            var diagX = x + diag;

                            if (_inverse)
                                diagX = x - diag;

                            if (diagX < 0 || diagX >= xDim) continue;
                            var diagPiece = _pieces[diagX, y + 1];

                            if (diagPiece.Type != PieceType.EMPTY) continue;
                            var hasPieceAbove = true;

                            for (var aboveY = y; aboveY >= 0; aboveY--)
                            {
                                var pieceAbove = _pieces[diagX, aboveY];

                                if (pieceAbove.IsMovable()) break;
                                if (pieceAbove.IsMovable() || pieceAbove.Type == PieceType.EMPTY) continue;
                                                
                                hasPieceAbove = false;
                                break;
                            }

                            if (hasPieceAbove) continue;
                                            
                            Destroy(diagPiece.gameObject);
                            piece.MovablePiece.Move(diagX, y + 1, fillTime);
                            _pieces[diagX, y + 1] = piece;
                            SpawnNewPiece(x, y, PieceType.EMPTY);
                            movePiece = true;
                            break;
                        }
                    }
                }
            }

            for (var x = 0; x < xDim; x++)
            {
                var pieceBelow = _pieces[x, 0];
                if (pieceBelow.Type != PieceType.EMPTY) continue;
                
                Destroy(pieceBelow.gameObject);
                var newPiece = Instantiate(_piecePrefabDictionary[PieceType.NORMAL], GetWorldPosition(x, -1),
                    Quaternion.identity);
                newPiece.transform.parent = transform;

                _pieces[x, 0] = newPiece.GetComponent<Piece>();
                _pieces[x, 0].Init(x, -1, PieceType.NORMAL, this);
                _pieces[x, 0].MovablePiece.Move(x, 0, fillTime);
                _pieces[x, 0].ColourPiece.ColourType = (ColourType)Random.Range(0, _pieces[x, 0].ColourPiece.ColourAmount);

                movePiece = true;
            }

            return movePiece;
        }

        private static bool IsAdjacent(Piece source, Piece destination)
        {
            return (source.X == destination.X && Mathf.Abs(source.Y - destination.Y) == 1)
                || (source.Y == destination.Y && Mathf.Abs(source.X - destination.X) == 1);
        }

        private void SwapPieces(Piece source, Piece destination)
        {
            if (!source.IsMovable() || !destination.IsMovable()) return;
            
            var sourceX = source.X;
            var sourceY = source.Y;
                
            _pieces[source.X, source.Y] = destination;
            _pieces[destination.X, destination.Y] = source;
                
            source.MovablePiece.Move(destination.X, destination.Y, fillTime);
            destination.MovablePiece.Move(sourceX, sourceY, fillTime);
        }

        public void Source(Piece piece)
        {
            _source = piece;
        }
        
        public void Destination(Piece piece)
        {
            _destination = piece;
        }

        public void Release()
        {
            if (IsAdjacent(_source, _destination))
                SwapPieces(_source, _destination);
        }
    }
}