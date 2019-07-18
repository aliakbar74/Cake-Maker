using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerButton : MonoBehaviour, IPointerDownHandler {
    private Texture2D _texture2D;

    public void SelectSticker() {
        if (_texture2D == null)
            _texture2D = GetComponent<Image>().sprite.ToTexture2D();
        
        GameController.Instance.ChangeSticker(_texture2D);
    }

    public void OnPointerDown(PointerEventData eventData) {
        SelectSticker();
    }
}