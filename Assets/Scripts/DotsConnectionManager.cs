using System.Collections;
using System.Collections.Generic;
using DotsClone;
using UnityEngine;

namespace DotsClone
{
    public class DotsConnectionManager : MonoBehaviour
    {
        public GameObject ConnectorPrefab;

        public delegate void OnDotsCleared(Dictionary<int, int> dotsCleared);
        public static event OnDotsCleared DotsCleared;

        public delegate void OnSquareCompleted(DotType dotType);
        public static event OnSquareCompleted SquareCompleted;

        private DotsConnector LastConnectedLine => _connectedLines[_connectedLines.Count - 1];
        private Dot LastConnectedDot => _connectedDots[_connectedLines.Count - 1];
        private List<Dot> _connectedDots;
        private List<DotsConnector> _connectedLines;

        private bool _isSquare;
        private bool _isActiveConnection;

        private void Start()
        {
            DotsInputModule.DotPressStarted += CreateConnection;
            DotsInputModule.DotEnter += AddConnection;
            DotsInputModule.DotPressEnded += ClearDots;

            _connectedDots = new List<Dot>();
            _connectedLines = new List<DotsConnector>();
        }

        private void CreateConnection(Dot dot)
        {
            _connectedDots.Add(dot);
            dot.PlayDotConnectedAnimation();

            _isSquare = dot.IsConnected;
            if (_isSquare)
                return;

            var connector = Instantiate(ConnectorPrefab).GetComponent<DotsConnector>();
            connector.StartConnection(dot);
            _connectedLines.Add(connector);
            dot.IsConnected = true;
        }

        private void AddConnection(Dot dot)
        {
            // If we don't already have a connection or we go over the last dot, then just return.
            if (_connectedDots.Count == 0 ||
                dot.DotCoordinates == _connectedDots[_connectedDots.Count - 1].DotCoordinates)
            {
                return;
            }

            // If we have more than one dot and go over the second to last dot, remove it form the active dot list
            if (_connectedDots.Count > 1 && dot.DotCoordinates ==
                _connectedDots[Mathf.Clamp(_connectedDots.Count - 2, 0, _connectedDots.Count - 1)].DotCoordinates)
            {
                _isSquare = false;

                _connectedDots[_connectedDots.Count - 1].IsConnected = false;
                _connectedDots.RemoveAt(_connectedDots.Count - 1);

                Destroy(LastConnectedLine.gameObject);
                _connectedLines.RemoveAt(_connectedLines.Count - 1);

                LastConnectedLine.Disconnect();
            }
            else if (!_isSquare && Dot.NeighboringDotCheck(dot, _connectedDots[_connectedDots.Count - 1]))
            {
                LastConnectedLine.ConnectToDot(dot);
                CreateConnection(dot);
            }
        }

        private void ClearDots()
        {
            // Key = vertical index to update 
            // Value = num of dots cleared per vertical group 
            var verticalSliceToUpdate = new Dictionary<int, int>();
            var dotsRemovedCount = 0;

            // Where we could count score if desired.
            if (_isSquare)
            {
                SquareCompleted?.Invoke(LastConnectedDot.CurrentDotType);
                _isSquare = false;
            }
            else if (_connectedDots.Count > 1)
            {
                foreach (var dot in _connectedDots)
                {
                    if (dot == null) continue;
                    dotsRemovedCount++;
                    var rowOfRemovedDot = (int) dot.DotCoordinates.x;

                    if (!verticalSliceToUpdate.ContainsKey(rowOfRemovedDot))
                        verticalSliceToUpdate[rowOfRemovedDot] = 0;
                    verticalSliceToUpdate[rowOfRemovedDot] += 1;
                    dot.IsCleared = true;
                    Destroy(dot.gameObject);
                }
            } 
            else if (_connectedDots.Count == 1)
            {
                LastConnectedDot.IsConnected = false;
            }

            foreach (var connector in _connectedLines)
            {
                if (connector != null)
                    Destroy(connector.gameObject);
            }

            _connectedDots.Clear();
            _connectedLines.Clear();

            DotsCleared?.Invoke(verticalSliceToUpdate);
            DotsGameManager.SharedInstance.DotsCleared += dotsRemovedCount;
        }
    }
}