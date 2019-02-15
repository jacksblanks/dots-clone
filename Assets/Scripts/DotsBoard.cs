using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

namespace DotsClone {
    /// <summary>
    /// Manager Class and functions for exposing Dots Board - definitely the most tricky part
    /// and optimizable. Could also build out further with tools to allow game designers to
    /// easily modify and test.
    /// </summary>
    public class DotsBoard : MonoBehaviour
    {
        public const float DOT_SPACING = 1.0f;
        public static Vector2 DOT_BOARD_START_POS = new Vector2(0, 0);
        public const float Y_START_OFFSET = 10;
        [SerializeField]
        public int DotGridSizeVertical = 6;
        [SerializeField]
        public int DotGridSizeHorizontal = 10;
        [SerializeField]
        public GameObject DotPrefab;

        private Dot[,] _activeDots; 

        private void Start()
        {
            _activeDots = new Dot[DotGridSizeHorizontal, DotGridSizeVertical];

            DotsConnectionManager.DotsCleared += ReplaceDots;
            DotsConnectionManager.SquareCompleted += RemoveAllDotsForDotType;

            InitializeDotsBoard();
        }

        private void InitializeDotsBoard()
        {
            for (var horizontalIndex = 0; horizontalIndex < DotGridSizeHorizontal; horizontalIndex++)
                for (var verticalIndex = 0; verticalIndex < DotGridSizeVertical; verticalIndex++)
                    SpawnNewDot(horizontalIndex, verticalIndex, Dot.GetRandomDotType());
        }

        private Dot GetNearestDotInColumn(int horizontalIndex, int startingVerticalIndex)
        {
            for (var verticalIndex = startingVerticalIndex; verticalIndex < DotGridSizeVertical; verticalIndex++)
            {
                if (_activeDots[horizontalIndex, verticalIndex] == null || _activeDots[horizontalIndex, verticalIndex].IsCleared) continue;
                return _activeDots[horizontalIndex, verticalIndex];
            }
            return null;
        }

        private void RemoveAllDotsForDotType(DotType dotType)
        {
            var verticalSlicesToUpdate = new Dictionary<int, int>();
            var dotsRemovedCount = 0;
            for (var horizontalIndex = 0; horizontalIndex < DotGridSizeHorizontal; horizontalIndex++)
            {
                for (var columns = 0; columns < DotGridSizeVertical; columns++)
                {
                    var dot = _activeDots[horizontalIndex, columns];
                    if (dot.CurrentDotType != dotType) continue;
                    dotsRemovedCount++;

                    var rowOfRemovedDot = (int) dot.DotCoordinates.x;
                    if (!verticalSlicesToUpdate.ContainsKey(rowOfRemovedDot))
                        verticalSlicesToUpdate[rowOfRemovedDot] = 0;
                    
                    verticalSlicesToUpdate[rowOfRemovedDot] += 1;
                    
                    dot.IsCleared = true;
                    Destroy(dot.gameObject);
                }
            }

            ReplaceDots(verticalSlicesToUpdate, dotType);
            DotsGameManager.SharedInstance.DotsCleared += dotsRemovedCount;
        }

        private void ReplaceDots(Dictionary<int,int> numOfDotsClearedForEachVerticalSlice, DotType typeToExempt)
        {
            foreach (var rows in numOfDotsClearedForEachVerticalSlice.Keys)
            {
                for (var columns = 0; columns < DotGridSizeVertical; columns++)
                {
                    if (_activeDots[rows, columns] != null && !_activeDots[rows, columns].IsCleared) continue;

                    var nextDot = GetNearestDotInColumn(rows, columns);
                    if (nextDot == null) break;

                    _activeDots[(int) nextDot.DotCoordinates.x, (int) nextDot.DotCoordinates.y] = null;
                    _activeDots[rows, columns] = nextDot;
                    nextDot.DotCoordinates = new Vector2(rows, columns);
                   
                    nextDot.transform.DOMoveY(GetWorldSpacePositionForDot(nextDot).y, 0.25f).SetEase(Ease.InOutQuint);
                }

                for (var verticalIndex = DotGridSizeVertical - numOfDotsClearedForEachVerticalSlice[rows]; verticalIndex < DotGridSizeVertical; verticalIndex++)
                {
                    SpawnNewDot(rows, verticalIndex, Dot.GetRandomDotTypeExceptOne(typeToExempt));
                }
            }
        }

        private void ReplaceDots(Dictionary<int,int> numOfDotsClearedForEachVerticalSlice)
        {
            foreach (var rows in numOfDotsClearedForEachVerticalSlice.Keys)
            {
                for (var columns = 0; columns < DotGridSizeVertical; columns++)
                {
                    if (_activeDots[rows, columns] != null && !_activeDots[rows, columns].IsCleared) continue;

                    var nextDot = GetNearestDotInColumn(rows, columns);
                    if (nextDot == null) break;

                    _activeDots[(int) nextDot.DotCoordinates.x, (int) nextDot.DotCoordinates.y] = null;
                    _activeDots[rows, columns] = nextDot;
                    nextDot.DotCoordinates = new Vector2(rows, columns);
                   
                    nextDot.transform.DOMoveY(GetWorldSpacePositionForDot(nextDot).y, 0.25f).SetEase(Ease.InOutQuint);
                }

                for (var verticalIndex = DotGridSizeVertical - numOfDotsClearedForEachVerticalSlice[rows]; verticalIndex < DotGridSizeVertical; verticalIndex++)
                {
                    SpawnNewDot(rows, verticalIndex, Dot.GetRandomDotType());
                }
            }
        }

        private void SpawnNewDot(int horizontalIndex, int verticalIndex, DotType type)
        {
            // Should use Object Pool instead of instantiating 
            var dot = Instantiate(DotPrefab, transform).GetComponent<Dot>();
            _activeDots[horizontalIndex, verticalIndex] = dot;
            dot.DotCoordinates = new Vector2(horizontalIndex, verticalIndex);
            
            var worldPos = GetWorldSpacePositionForDot(dot);
            dot.transform.position = worldPos + Vector2.up * Y_START_OFFSET;
            dot.CurrentDotType = type;
            dot.gameObject.SetActive(true);

            dot.transform.DOMoveY(worldPos.y, 0.5f + 0.05f * verticalIndex).SetEase(Ease.InOutQuint);
        }

        public static Vector2 GetWorldSpacePositionForDot(Dot dot)
        {
            return DOT_BOARD_START_POS + DOT_SPACING * dot.DotCoordinates;
        }
    }

}
