using UnityEngine;
using UnityEngine.EventSystems;

public class CanPaintController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public void OnPointerDown(PointerEventData eventData) {
        GameController.Instance.CanPaint = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        GameController.Instance.CanPaint = false;
    }
}