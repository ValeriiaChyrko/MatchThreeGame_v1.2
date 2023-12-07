using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
            var needsRefill = true;
            while (needsRefill)
            {
                yield return new WaitForSeconds(fillTime);
                
                while (FillStep())
                {
                    _inverse = !_inverse;
                    yield return new WaitForSeconds(fillTime);
                }

                needsRefill = ClearAllValidMatches();
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

                        piece.MovableComponent.Move(x, y + 1, fillTime);
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
                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
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
                _pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                _pieces[x, 0].ColourComponent.ColourType = (ColourType)Random.Range(0, _pieces[x, 0].ColourComponent.ColourAmount);

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
            
            _pieces[source.X, source.Y] = destination;
            _pieces[destination.X, destination.Y] = source;
            
            if (GetMatch(source, destination.X, destination.Y) != null
                || GetMatch(destination, source.X, source.Y) != null)
            {
                var sourceX = source.X;
                var sourceY = source.Y;
                
                source.MovableComponent.Move(destination.X, destination.Y, fillTime);
                destination.MovableComponent.Move(sourceX, sourceY, fillTime);

                ClearAllValidMatches();
                StartCoroutine(Fill());
            }
            else
            {
                _pieces[source.X, source.Y] = source;
                _pieces[destination.X, destination.Y] = destination;
            }
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

        [CanBeNull]
        private List<Piece> GetMatch(Piece piece, int newX, int newY)
        {
            if (!piece.IsColour()) return null;
            var colour = piece.ColourComponent.ColourType;

            var horizontalPieces = new List<Piece>();
            var verticalPieces = new List<Piece>();
            var matchingPieces = new List<Piece>();
                
            horizontalPieces.Add(piece);
            for (var dir = 0; dir <= 1; dir++)
            {
                for (var xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if (dir == 0)
                        x = newX - xOffset;
                    else
                        x = newX + xOffset;

                    if (x < 0 || x >= xDim)
                        break;

                    if (_pieces[x, newY].IsColour() && _pieces[x, newY].ColourComponent.ColourType == colour)
                        horizontalPieces.Add(_pieces[x, newY]);
                    else
                        break;
                }
            }

            if (horizontalPieces.Count >= 3)
                matchingPieces.AddRange(horizontalPieces);

            if (horizontalPieces.Count >= 3)
            {
                foreach (var t in horizontalPieces)
                {
                    for (var dir = 0; dir < 1; dir++)
                    {
                        for (var yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;

                            if (dir == 0)
                                y = newY - yOffset;
                            else
                                y = newY + yOffset;

                            if (y < 0 || y >= yDim) break;

                            if (_pieces[t.X, y].IsColour()
                                && _pieces[t.X, y].ColourComponent.ColourType == colour)
                                verticalPieces.Add(_pieces[t.X, y]);
                            else
                                break;
                        }
                    }
                    if (verticalPieces.Count < 2)
                        verticalPieces.Clear();
                    else
                    {
                        matchingPieces.AddRange(verticalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
                return matchingPieces;
                
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);
            for (var dir = 0; dir <= 1; dir++)
            {
                for (var yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                        y = newY - yOffset;
                    else
                        y = newY + yOffset;

                    if (y < 0 || y >= yDim)
                        break;

                    if (_pieces[newX, y].IsColour() && _pieces[newX, y].ColourComponent.ColourType == colour)
                        verticalPieces.Add(_pieces[newX, y]);
                    else
                        break;
                }
            }
                
            if (verticalPieces.Count >= 3)
                matchingPieces.AddRange(verticalPieces);
                
            if (verticalPieces.Count >= 3)
            {
                for (var i = 0; i < verticalPieces.Count; i++)
                {
                    for (var dir = 0; dir < 1; dir++)
                    {
                        for (var xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                                x = newX - xOffset;
                            else
                                x = newX + xOffset;

                            if (x < 0 || x >= xDim) break;

                            if (_pieces[x, verticalPieces[i].Y].IsColour()
                                && _pieces[x, verticalPieces[i].Y].ColourComponent.ColourType == colour)
                                verticalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            else
                                break;
                        }
                    }
                    if (horizontalPieces.Count < 2)
                        horizontalPieces.Clear();
                    else
                    {
                        matchingPieces.AddRange(horizontalPieces);
                        break;
                    }
                }
            }
            return matchingPieces.Count >= 3 ? matchingPieces : null;
        }

        private bool ClearAllValidMatches()
        {
            var needsRefill = false;

            for (var y = 0; y < yDim; y++)
            {
                for (var x = 0; x < xDim; x++)
                {
                    if (!_pieces[x, y].IsClearable()) continue;
                    var match = GetMatch(_pieces[x, y], x, y);

                    if (match == null) continue;
                    foreach (var t in match.Where(t => ClearPiece(t.X, t.Y)))
                        needsRefill = true;
                }
            }

            return needsRefill;
        }

        private bool ClearPiece(int x, int y)
        {
            if (!_pieces[x, y].IsClearable() || _pieces[x, y].ClearableComponent.IsBeingCleared) return false;
            
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);

            return true;

        }
    }
}