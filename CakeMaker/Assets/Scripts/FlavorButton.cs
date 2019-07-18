using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace {
    public class FlavorButton : MonoBehaviour, IPointerDownHandler {
        public Color Color;

        private void SelectFlavor() {
            GameController.Instance.ChangeCakeColor(Color);
        }

        public void OnPointerDown(PointerEventData eventData) {
            SelectFlavor();
        }
    }
}