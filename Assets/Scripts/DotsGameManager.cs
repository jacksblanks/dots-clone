using System.Collections;
using System.Collections.Generic;
using DotsClone;
using UnityEngine;

namespace DotsClone
{
    /// <summary>
    /// Singleton class to hold game state data like score, time, active theme, etc.
    /// </summary>
    public class DotsGameManager : MonoBehaviour
    {
        public static DotsGameManager SharedInstance;

        public DotsTheme CurrentTheme;
        public int DotsCleared;

        private void Awake()
        {
            if (SharedInstance == null)
                SharedInstance = this;
        }
    }
}