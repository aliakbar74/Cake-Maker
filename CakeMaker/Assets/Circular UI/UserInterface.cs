using System.Collections;
using System.Collections.Generic;
using Circular_UI;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    [Header("Gerenal")] public Camera UICamera;
    public GameObject BackgroundPanel, CircleMenuElementPrefab;
    public bool UserGradient;

    [Header("Buttons")] public Color NormalBtnColor, HighlightBtnColor;
    public Gradient HighlightBtnGradient;

    [Header("Informal center")] public Image InformalCenterBg;
//    public Text ItemName, ItemDescirption;
//    public Image ItemIcon;

    private int _currentMenuItemIndex, _previousMenuItemIndex;
    private float _calculatedMenuIndex, _currentSelectionIndex;
    private Vector3 _currentMousePosition;
    private float _currentSelectionAngle;
    private List<CircularElement> _menuElements = new List<CircularElement>();
    public List<CircularElement> mElements;

    private static UserInterface _instance;

    public static UserInterface Instance => _instance;
    public bool Active => BackgroundPanel.activeSelf;

    public List<CircularElement> MenuElements {
        get => _menuElements;
        set => _menuElements = value;
    }

    private void Start() {
        Initialize();
    }

    void Initialize() {
        MenuElements = mElements;


        var rotationalIncrementalValue = 360f / MenuElements.Count;
        var currentRotationalValue = 0f;
        var fillPercentageValue = 1f / MenuElements.Count;

        for (int i = 0; i < MenuElements.Count; i++) {
            //todo : world pos?
            var menuElementGo = Instantiate(CircleMenuElementPrefab, BackgroundPanel.transform, true);
            menuElementGo.name = i + ": " + currentRotationalValue;

            var menuBtn = menuElementGo.GetComponent<MenuButton>();

            menuBtn.RectTransform.localScale = Vector3.one;
            menuBtn.RectTransform.localPosition = Vector3.zero;
            menuBtn.RectTransform.rotation = Quaternion.Euler(0, 0, currentRotationalValue);
            currentRotationalValue += rotationalIncrementalValue;

            menuBtn.BackGroundImg.fillAmount = fillPercentageValue + 0.001f;
            MenuElements[i].ButtonBg = menuBtn.BackGroundImg;

            menuBtn.IconImage.sprite = MenuElements[i].ButtonIcon;
            menuBtn.IconRectTransform.rotation = Quaternion.identity;
        }

        BackgroundPanel.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Activate();
        }
        if (!Active) return;
        GetCurrentMenuElement();
        if (Input.GetMouseButton(0))
            Select();
        
    }

    private void GetCurrentMenuElement() {
        var rotationalIncrementalValue = 360 / MenuElements.Count;
        _currentMousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2f,
            Input.mousePosition.y - Screen.height / 2f);
        _currentSelectionAngle = 90 + rotationalIncrementalValue +
                                 Mathf.Atan2(_currentMousePosition.y, _currentMousePosition.x) * Mathf.Rad2Deg;
        _currentSelectionAngle = (_currentSelectionAngle + 360f) % 360f;

        _currentMenuItemIndex = (int) (_currentSelectionAngle / rotationalIncrementalValue);
        if (_currentMenuItemIndex != _previousMenuItemIndex) {
            MenuElements[_previousMenuItemIndex].ButtonBg.color = NormalBtnColor;
            _previousMenuItemIndex = _currentMenuItemIndex;
            MenuElements[_currentMenuItemIndex].ButtonBg.color = UserGradient
                ? HighlightBtnGradient.Evaluate(1f / MenuElements.Count * _currentMenuItemIndex)
                : InformalCenterBg.color = UserGradient
                    ? HighlightBtnGradient.Evaluate(1f / MenuElements.Count * _currentMenuItemIndex)
                    : RefreshInformation();
        }
    }

    private Color RefreshInformation() {
        return Color.white;
    }

    void Select() {
        BuildingSystem.Instace.SwitchToIndex(_currentMenuItemIndex);
        Deactivate();
    }

    private void Deactivate() {
        BackgroundPanel.SetActive(false);
    }

    void Activate() {
        if (Active) return;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        BackgroundPanel.SetActive(true);
        RefreshInformation();
    }
}

internal class BuildingSystem {
    public class Instace {
        public static void SwitchToIndex(int currentMenuItemIndex) {
        }
    }
}