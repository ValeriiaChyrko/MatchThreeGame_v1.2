using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MatchThreeGame._Project.Scripts.Audio;
using MatchThreeGame._Project.Scripts.Enums;
using MatchThreeGame._Project.Scripts.GridPiece;
using MatchThreeGame._Project.Scripts.Level;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private int xDim;
        [SerializeField] private int yDim;
        [SerializeField] private float fillTime;

        [SerializeField] private PiecePrefab[] piecePrefabs;
        [SerializeField] private PiecePosition[] piecesPositions;
        [SerializeField] private GameObject backgroundPrefab;
        
        [SerializeField] private GameObject explosion;
        
        [SerializeField] public LevelController level;

        private Dictionary<PieceType, GameObject> _piecePrefabDictionary;
        private Piece[,] _pieces;
        private bool _inverse;
        private bool _gameOver;

        private AudioManager _audioManager;

        public bool IsFilling { get; private set; }

        private Piece _source;
        private Piece _destination;

        private void Awake()
        {
            _audioManager = GetComponent<AudioManager>();
            _piecePrefabDictionary = new Dictionary<PieceType, GameObject>();

            foreach (var t in piecePrefabs)
                _piecePrefabDictionary.TryAdd(t.type, t.prefab);

            for (var i = 0; i < xDim; i++)
                for (var j = 0; j < yDim; j++)
                {
                    var background = Instantiate(backgroundPrefab, GetWorldPosition(i, j), Quaternion.identity);
                    background.transform.parent = transform;
                }

            _pieces = new Piece[xDim, yDim];
            
            foreach (var t in piecesPositions)
                if (t.x >= 0 && t.x < xDim && t.y >= 0 && t.y < yDim)
                    SpawnNewPiece(t.x, t.y, t.type);

            for (var i = 0; i < xDim; i++)
                for (var j = 0; j < yDim; j++)
                    if (_pieces[i, j] == null)
                        SpawnNewEmptyPiece(i, j);

            StartCoroutine(Fill());
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            var position = transform.position;
            return new Vector2(position.x - xDim / 2.0f + x, position.y + yDim / 2.0f - y);
        }

        private Piece SpawnNewPiece(int x, int y, PieceType type)
        {
            var newPiece = Instantiate(_piecePrefabDictionary[type], GetWorldPosition(x, y), Quaternion.identity);
            newPiece.transform.parent = transform;

            _pieces[x, y] = newPiece.GetComponent<Piece>();
            _pieces[x, y].Init(x, y, type, this);

            return _pieces[x, y];
        }

        private IEnumerator Fill()
        {
            IsFilling = true;
            while (true)
            {
                yield return new WaitForSeconds(fillTime * 2.0f);

                while (FillStep())
                {
                    _inverse = !_inverse;
                    yield return new WaitForSeconds(fillTime);
                }

                if (!ClearAllValidMatches()) break;
            }

            IsFilling = false;
        }

        private bool FillStep()
        {
            var movePiece = false;

            movePiece |= MovePiecesDown();
            movePiece |= FillTopRow();

            return movePiece;
        }

        private bool MovePiecesDown()
        {
            var movePiece = false;

            for (var y = yDim - 2; y >= 0; y--)
            {
                for (var loopX = 0; loopX < xDim; loopX++)
                {
                    var x = loopX;
                    if (_inverse) x = xDim - 1 - loopX;

                    movePiece |= TryMovePieceDown(x, y);
                }
            }

            return movePiece;
        }

        private bool TryMovePieceDown(int x, int y)
        {
            var movePiece = false;
            var piece = _pieces[x, y];

            if (!piece.IsMovable()) return false;

            var pieceBelow = _pieces[x, y + 1];

            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);

                piece.MovableComponent.Move(x, y + 1, fillTime);
                _pieces[x, y + 1] = piece;
                SpawnNewEmptyPiece(x, y);

                movePiece = true;
            }
            else
            {
                movePiece |= TryMovePieceDiagonally(piece, x, y);
            }

            _audioManager.PlayWoosh();
            return movePiece;
        }

        private bool TryMovePieceDiagonally(Piece piece, int x, int y)
        {
            var movePiece = false;

            for (var diag = -1; diag <= 1; diag++)
            {
                if (diag == 0) continue;
                var diagX = x + diag;

                if (_inverse) diagX = x - diag;

                if (diagX < 0 || diagX >= xDim) continue;
                var diagPiece = _pieces[diagX, y + 1];

                if (diagPiece.Type != PieceType.EMPTY) continue;
                var hasPieceAbove = HasPieceAbove(diagX, y);

                if (!hasPieceAbove)
                {
                    Destroy(diagPiece.gameObject);
                    piece.MovableComponent.Move(diagX, y + 1, fillTime);
                    _pieces[diagX, y + 1] = piece;
                    SpawnNewEmptyPiece(x, y);
                    movePiece = true;
                    break;
                }
            }

            return movePiece;
        }

        private bool HasPieceAbove(int x, int y)
        {
            for (var aboveY = y; aboveY >= 0; aboveY--)
            {
                var pieceAbove = _pieces[x, aboveY];

                if (pieceAbove.IsMovable()) break;
                if (pieceAbove.IsMovable() || pieceAbove.Type == PieceType.EMPTY) continue;

                return false;
            }

            return true;
        }

        private bool FillTopRow()
        {
            var movePiece = false;

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
                _pieces[x, 0].ColourComponent.ColourType =
                    (ColourType)Random.Range(0, _pieces[x, 0].ColourComponent.ColourAmount);

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
            if (_gameOver) return;
            if (!source.IsMovable() || !destination.IsMovable())
            {
                _audioManager.PlayNoMatch();
                return;
            }

            _pieces[source.X, source.Y] = destination;
            _pieces[destination.X, destination.Y] = source;

            if (GetMatch(source, destination.X, destination.Y) != null
                || GetMatch(destination, source.X, source.Y) != null
                || source.Type == PieceType.RAINBOW || destination.Type == PieceType.RAINBOW)
            {
                var sourceX = source.X;
                var sourceY = source.Y;

                source.MovableComponent.Move(destination.X, destination.Y, fillTime);
                destination.MovableComponent.Move(sourceX, sourceY, fillTime);
                _audioManager.PlayMatch();

                if (source.Type == PieceType.RAINBOW && source.IsClearable() && 
                    destination.IsClearable())
                {
                    var clearColour = source.GetComponent<ClearColorPiece>();

                    if (clearColour)
                        clearColour.Colour = destination.ColourComponent.ColourType;
                    
                    ClearPiece(source.X, source.Y);
                }
                
                if (destination.Type == PieceType.RAINBOW && source.IsClearable() && 
                    destination.IsClearable())
                {
                    var clearColour = destination.GetComponent<ClearColorPiece>();

                    if (clearColour)
                        clearColour.Colour = source.ColourComponent.ColourType;
                    
                    ClearPiece(destination.X, destination.Y);
                }
                
                ClearAllValidMatches();

                StartCoroutine(Fill());
                level.OnMove();
            }
            else
            {
                _pieces[source.X, source.Y] = source;
                _pieces[destination.X, destination.Y] = destination;
                _audioManager.PlayNoMatch();
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

            var horizontalPieces = GetHorizontalPieces(piece, newX, newY, colour);
            var verticalPieces = GetVerticalPieces(piece, newX, newY, colour);
            var matchingPieces = new List<Piece>();

            if (horizontalPieces.Count >= 3)
                matchingPieces.AddRange(horizontalPieces);

            if (verticalPieces.Count >= 3)
                matchingPieces.AddRange(verticalPieces);

            return matchingPieces.Count >= 3 ? matchingPieces : null;
        }

        private List<Piece> GetHorizontalPieces(Piece piece, int newX, int newY, ColourType colour)
        {
            var horizontalPieces = new List<Piece> { piece };

            for (var dir = 0; dir <= 1; dir++)
            {
                for (var xOffset = 1; xOffset < xDim; xOffset++)
                {
                    var x = (dir == 0) ? newX - xOffset : newX + xOffset;

                    if (x < 0 || x >= xDim)
                        break;

                    if (_pieces[x, newY].IsColour() && _pieces[x, newY].ColourComponent.ColourType == colour)
                        horizontalPieces.Add(_pieces[x, newY]);
                    else
                        break;
                }
            }

            return horizontalPieces;
        }

        private List<Piece> GetVerticalPieces(Piece piece, int newX, int newY, ColourType colour)
        {
            var verticalPieces = new List<Piece> { piece };

            for (var dir = 0; dir <= 1; dir++)
            {
                for (var yOffset = 1; yOffset < yDim; yOffset++)
                {
                    var y = (dir == 0) ? newY - yOffset : newY + yOffset;

                    if (y < 0 || y >= yDim)
                        break;

                    if (_pieces[newX, y].IsColour() && _pieces[newX, y].ColourComponent.ColourType == colour)
                        verticalPieces.Add(_pieces[newX, y]);
                    else
                        break;
                }
            }

            return verticalPieces;
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
                    var specialPieceType = DetermineSpecialPieceType(match);
                    var specialPiecePosition = DetermineSpecialPiecePosition(match, specialPieceType);

                    ProcessMatch(match, specialPiecePosition, ref needsRefill, specialPieceType);
                }
            }

            return needsRefill;
        }

        private void ProcessMatch(List<Piece> match, (int, int) specialPiecePosition, ref bool needsRefill,
            PieceType specialPieceType)
        {
            foreach (var t in match.Where(t => ClearPiece(t.X, t.Y)))
            {
                needsRefill = true;

                if (IsSpecialPiecePosition(t.X, t.Y, specialPiecePosition))
                    specialPiecePosition = (t.X, t.Y);
            }

            if (specialPieceType == PieceType.COUNT) return;
            Destroy(_pieces[specialPiecePosition.Item1, specialPiecePosition.Item2]);
            var newPiece = SpawnNewPiece(specialPiecePosition.Item1, specialPiecePosition.Item2, specialPieceType);

            TransferColourIfNecessary(newPiece);
        }

        private static bool IsSpecialPiecePosition(int x, int y, (int, int) specialPiecePosition)
        {
            return x == specialPiecePosition.Item1 && y == specialPiecePosition.Item2;
        }

        private PieceType DetermineSpecialPieceType(ICollection match)
        { 
            return match.Count == 5 ? PieceType.RAINBOW : PieceType.COUNT;
        }

        private static (int, int) DetermineSpecialPiecePosition(List<Piece> match, PieceType specialPieceType)
        {
            if (specialPieceType == PieceType.COUNT) return (0, 0);

            var randomPiece = match[Random.Range(0, match.Count)];
            return (randomPiece.X, randomPiece.Y);
        }

        private static void TransferColourIfNecessary(Piece newPiece)
        {
            if (newPiece.Type == PieceType.RAINBOW && newPiece.IsColour())
                newPiece.ColourComponent.ColourType = ColourType.ANY;
        }


        private bool ClearPiece(int x, int y)
        {
            if (!_pieces[x, y].IsClearable() || _pieces[x, y].ClearableComponent.IsBeingCleared) return false;

            ExplodeVFX(x, y);
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewEmptyPiece(x, y);
            
            return true;
        }
        
        private void SpawnNewEmptyPiece(int x, int y)
        {
            SpawnNewPiece(x, y, PieceType.EMPTY);
        }

        public void ClearColour(ColourType colour)
        {
            for (var x = 0; x < xDim; x++)
            {
                for (var y = 0; y < yDim; y++)
                {
                    if (_pieces[x, y].IsColour() && (_pieces[x, y].ColourComponent.ColourType == colour
                        || colour == ColourType.ANY))
                    {
                        ClearPiece(x, y);
                    }
                }
            }
        }
        
        private void ExplodeVFX(int x, int y) {
            var fx = Instantiate(explosion, transform);
            fx.transform.position = GetWorldPosition(x, y);
            Destroy(fx, 5f);
        }

        public void GameOver()
        {
            _audioManager.PlayPop();
            _gameOver = true;
        }
    }
}