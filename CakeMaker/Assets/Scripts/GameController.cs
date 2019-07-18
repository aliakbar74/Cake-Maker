using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enums;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable CompareOfFloatsByEqualityOperator

public class GameController : MonoBehaviour {
    public static GameController Instance;

    public GameObject Cake, Vessel, Under;

    public GameObject[] Cakes;
    public Texture2D[] BrushShape;
    public Texture2D[] Stickers;

    [Header("Brush")] public P3D_ClickToPaint ClickToPaint;
    public ColorPicker ColorPicker;

    [Header("UI")] public Slider BrushSizeSlider;
    public GameObject SelectPnl, CakeSizePnl, BrushShapePnl, BrushSettingsPnl, CakeShapePnl, CakeFlavorPnl, StickerPnl;
    public Image ActiveStickerHighlight;
    public Vector2 BrushSettingPos, StickerPnlPos;

    public float MoveUIDelay;

    private P3D_Brush _brush;
    private bool _isBrushSettingOn, _isStickerPnlOn, _isBrush;
    private static readonly int Main = Shader.PropertyToID("_MainTex");
    public bool CanPaint { get; set; }
    private CakeSize _cakeSize;
    private SidePanels _sidePanels;

    private void Awake() {
        Instance = this;
        ChangeCakeShape(0);
    }

    private void Start() {
        SetBrushDefault();
    }

    public void Clear() {
        ChangeCakeTextureColor(Color.white);
    }

    private void SetBrushDefault() {
        _brush = ClickToPaint.Brush;
        var value = BrushSizeSlider.value;
        var size = new Vector2(value, value);
        _brush.Size = size;
        _brush.Color = ColorPicker.CurrentColor;
        _isBrush = true;
        SetBrush();
    }

    public void ChangeCakeSize(int index) {
        switch (index) {
            case 0:
                _cakeSize = CakeSize.Small;
                break;
            case 1:
                _cakeSize = CakeSize.Medium;
                break;
            case 2:
                _cakeSize = CakeSize.Big;

                break;
            default:
                Debug.Log("Index Out Of Rage - Cake Shape Size");
                break;
        }

        CheckCakeSize();
        ClosePnls();
    }

    private void CheckCakeSize() {
        switch (_cakeSize) {
            case CakeSize.Small:
                Cake.transform.localScale = Vector3.one;
                Vessel.transform.localScale = Vector3.one;
                Under.transform.localScale = Vector3.one;
                break;
            case CakeSize.Medium:
                Cake.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                Vessel.transform.localScale = new Vector3(1.5f, 1, 1.5f);
                Under.transform.localScale = new Vector3(1.3f, 1, 1);
                break;
            case CakeSize.Big:
                Cake.transform.localScale = new Vector3(2, 2, 2);
                Vessel.transform.localScale = new Vector3(2, 1, 2);
                Under.transform.localScale = new Vector3(1.64f, 1, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangeCakeColor(Color color) {
        ChangeCakeTextureColor(color);
        ClosePnls();
    }

    public void ChangeCakeTextureColor(Color color) {
        Texture2D texture2D;
        if (Cake.GetComponent<MeshRenderer>() != null)
            texture2D = Cake.GetComponent<MeshRenderer>().material.GetTexture(Main) as Texture2D;
        else
            texture2D = Cake.GetComponentInChildren<MeshRenderer>().material.GetTexture(Main) as Texture2D;
        P3D_Helper.ClearTexture(texture2D, color);
    }

    public void ChangeCakeShape(int index) {
        foreach (var cake in Cakes) {
            cake.SetActive(false);
        }

        Cake = Cakes[index];
        Cake.SetActive(true);
        CheckCakeSize();
        ClosePnls();
    }

    public void ChangeSticker(int index) {
        _brush.Shape = Stickers[index];
        ColorPicker.CurrentColor = Color.white;
    }

    public void ChangeSticker(Texture2D texture2) {
        _brush.Shape = texture2;
        ColorPicker.CurrentColor = Color.white;
        OpenSidePnl(0);
        _isBrush = false;
    }

    public void ChangeBrushShape(int index) {
        _brush.Shape = BrushShape[index];
        ClosePnls();
        _isBrush = true;
    }


    public void ChangeBrushSize() {
        if (_brush == null) return;

        var value = BrushSizeSlider.value;
        _brush.Size.x = value;
        _brush.Size.y = value;
        SetBrush();
    }

    public void ChangeBrushColor() {
        if (_brush == null || !_isBrush) return;
        _brush.Color = ColorPicker.CurrentColor;
        SetBrush();
    }


    private void SetBrush() {
        if (_brush == null) return;
        ClickToPaint.Brush = _brush;
    }

    public void OpenCakeSizePnl() {
        OpenPanels(SelectPanelType.CakeSize);
    }

    public void OpenBrushShapePnl() {
        OpenPanels(SelectPanelType.BrushShape);
    }

    public void OpenCakeShapePnl() {
        OpenPanels(SelectPanelType.CakeShape);
    }

    public void OpenCakeFlavor() {
        OpenPanels(SelectPanelType.CakeFlavor);
    }

    public void OpenSidePnl(int index) {
        if (index == 0)
            _sidePanels = _sidePanels != SidePanels.StickerPnl ? SidePanels.StickerPnl : SidePanels.None;
        else if (index == 1) 
            _sidePanels = _sidePanels != SidePanels.ColorPalatePnl ? SidePanels.ColorPalatePnl : SidePanels.None;

        ActiveStickerHighlight.enabled = (int)_sidePanels==1;

        switch (_sidePanels) {
            case SidePanels.None:
                BrushSettingsPnl.transform.DOLocalMoveX(BrushSettingPos.y, MoveUIDelay);
                StickerPnl.transform.DOLocalMoveX(StickerPnlPos.y, MoveUIDelay);
                break;
            case SidePanels.StickerPnl:
                BrushSettingsPnl.transform.DOLocalMoveX(BrushSettingPos.y, MoveUIDelay);
                StickerPnl.transform.DOLocalMoveX(StickerPnlPos.x, MoveUIDelay);
                break;
            case SidePanels.ColorPalatePnl:
                BrushSettingsPnl.transform.DOLocalMoveX(BrushSettingPos.x, MoveUIDelay);
                StickerPnl.transform.DOLocalMoveX(StickerPnlPos.y, MoveUIDelay);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ClosePnls() {
        SelectPnl.SetActive(false);
        CakeSizePnl.SetActive(false);
        BrushShapePnl.SetActive(false);
        CakeShapePnl.SetActive(false);
        CakeFlavorPnl.SetActive(false);
    }

    public void OpenPanels(SelectPanelType type) {
        SelectPnl.SetActive(true);
        switch (type) {
            case SelectPanelType.CakeSize:
                CakeSizePnl.SetActive(true);
                break;
            case SelectPanelType.BrushShape:
                BrushShapePnl.SetActive(true);
                break;
            case SelectPanelType.BrushSettings:
                BrushSettingsPnl.transform.DOMoveX(BrushSettingPos.x, MoveUIDelay);
                break;
            case SelectPanelType.CakeShape:
                CakeShapePnl.SetActive(true);
                break;
            case SelectPanelType.CakeFlavor:
                CakeFlavorPnl.SetActive(true);
                break;
            default:
                Debug.Log("Invalid type!");
                break;
        }
    }
}