using UnityEngine;
using UnityEngine.EventSystems;

namespace DotsClone
{
    [RequireComponent(typeof(Dot))]
    public class DotsInputModule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        public Dot Dot;

        public delegate void OnDotPressedDown(Dot dot);
        public delegate void OnDotPressUp();
        public delegate void OnDotEnter(Dot dot);

        public static event OnDotPressedDown DotPressStarted;
        public static event OnDotPressUp DotPressEnded;
        public static event OnDotEnter DotEnter;

        private void Start()
        {
            Dot = GetComponent<Dot>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            DotPressStarted?.Invoke(Dot);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DotPressEnded?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DotEnter?.Invoke(Dot);
        }
    }


}
