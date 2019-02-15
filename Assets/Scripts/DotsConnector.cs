using UnityEngine;

namespace DotsClone
{
    /// <summary>
    /// Class to control connector, which is either attached to two dots or following the pointer if pointer is held down
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class DotsConnector : MonoBehaviour
    {
        public LineRenderer LineRenderer;
        private bool _isConnected;

        public void StartConnection(Dot dot)
        {
            LineRenderer.SetPosition(0, dot.transform.position);
            LineRenderer.sharedMaterial.color = dot.DotColor;
            SetLineToPointerPosition();
            LineRenderer.enabled = true;
        }

        public void ConnectToDot(Dot dot)
        {
            _isConnected = true;
            LineRenderer.SetPosition(1, dot.transform.position);
        }

        public void Disconnect()
        {
            _isConnected = false;
        }

        private void Update()
        {
            if (!_isConnected)
                SetLineToPointerPosition();
        }

        private void SetLineToPointerPosition()
        {
            Vector2 screenPosition;
            if (Input.touchSupported && Input.touchCount > 0)
            {
                screenPosition = Input.GetTouch(0).position;
            }
            else
            {
                screenPosition = Input.mousePosition;
            }
            var targetPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, 10);
            LineRenderer.SetPosition(1, targetPosition);
        }
    }
}
