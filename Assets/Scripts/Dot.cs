using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DotsClone {

    public enum DotType
    {
        DotTypeA,
        DotTypeB,
        DotTypeC,
        DotTypeD,
        DotTypeE,
        DotTypeF
    }    

    /// <summary>
    /// Manager class for single dot data and operations
    /// </summary>
    public class Dot : MonoBehaviour
    {
        public SpriteRenderer MainSpriteRenderer;
        public SpriteRenderer AnimationSpriteRenderer;

        public DotType CurrentDotType;
        public Vector2 DotCoordinates;
        public bool IsConnected;
        public bool IsCleared;

        public Color DotColor => DotsGameManager.SharedInstance.CurrentTheme.GetColorForDotType(CurrentDotType);
        
        private void Awake()
        {
            InitializeNewDot();
        }

        private void InitializeNewDot()
        {
            var dotColor = DotColor;
            MainSpriteRenderer.color =  dotColor;
            AnimationSpriteRenderer.color = dotColor;
        }

        public void PlayDotConnectedAnimation()
        {
            AnimationSpriteRenderer.transform.localScale = Vector3.one;
            AnimationSpriteRenderer.color = DotColor;
            AnimationSpriteRenderer.gameObject.SetActive(true);

            AnimationSpriteRenderer.DOFade(0, 0.5f).SetEase(Ease.OutCubic);
            AnimationSpriteRenderer.transform.DOScale(1.5f, 0.3f).SetEase(Ease.InOutQuint);
        }

        #if UNITY_EDITOR
        private void Update()
        {
            name = $"Dot: {DotCoordinates.x},{DotCoordinates.y}";
        }
        #endif

        // Uses the usually slow and avoidable, but at least only once.
        private static readonly List<DotType> _dotTypeList = Enum.GetValues(typeof(DotType)).Cast<DotType>().ToList();

        // Using Unity built in random function, probably will want to switch this out 
        // for a custom random function to control difficulty, make it more fun, have seeds etc. 
        public static DotType GetRandomDotType()
        {
            return (DotType) Random.Range(0, _dotTypeList.Count);
        }

        public static DotType GetRandomDotTypeExceptOne(DotType typeToExclude)
        {
            _dotTypeList.Remove(typeToExclude);
            var dotToReturn = _dotTypeList[Random.Range(0, 5)];
            _dotTypeList.Add(typeToExclude);
            return dotToReturn;
        }


        public static bool NeighboringDotCheck(Dot dot1, Dot dot2)
        {
            var diff = dot1.DotCoordinates - dot2.DotCoordinates;
            return diff.magnitude <= 1 && (int)diff.x != (int)diff.y && dot1.CurrentDotType == dot2.CurrentDotType;
        }

    }
}