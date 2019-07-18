using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraFallow : MonoBehaviour {
    public Transform TargetObject;

    Vector3 _destination = Vector3.zero;
    Vector3 _camVelocity = Vector3.zero;
    Vector3 _currentMousePos = Vector3.zero;
    Vector3 _prevoiusMousePos = Vector3.zero;


    public PositionSetting PosSetting = new PositionSetting();
    public OrbitSetting OrbitSetting = new OrbitSetting();
    public InputSetting CameraInput = new InputSetting();

    public bool OrbitInput;

    public float Zoominput;


    [SerializeField] private float X, Y;

//    public float yOrbitInput;
    // Use this for initialization
    void Start() {
        GetTarget(TargetObject);
        if (TargetObject)
            MoveToTarget();
    }

    // Update is called once per frame
    void Update() {
        GetInput();
        if (PosSetting.Allowzoom)
            ZoomOnTarget();
    }

    private void FixedUpdate() {
        if (TargetObject) {
            MoveToTarget();
            OrbitAroundTarget();
            LookAtTarget();
        }
    }

    void GetInput() {
        OrbitInput = Input.GetButton(CameraInput.MouseOrbit);
        Zoominput = Input.GetAxisRaw(CameraInput.MouseScroll);
    }

    void GetTarget(Transform t) {
        if (t == null)
            TargetObject = GameController.Instance.Cake.transform;
    }

    void MoveToTarget() {
        _destination = TargetObject.position;


        _destination += Quaternion.Euler(Y, X, 0) * -Vector3.forward * PosSetting.DistanceFromTarget;

        if (PosSetting.SmoothFallow) {
            transform.position =
                Vector3.SmoothDamp(transform.position, _destination, ref _camVelocity, PosSetting.Smooth);
        }
        else
            transform.position = _destination;
    }

    void ZoomOnTarget() {
        PosSetting.NewDistance += PosSetting.ZoomStep * Zoominput;
        PosSetting.NewDistance = Mathf.Clamp(PosSetting.NewDistance, PosSetting.MinZoom, PosSetting.MaxZoom);
        PosSetting.DistanceFromTarget = Mathf.Lerp(PosSetting.DistanceFromTarget, PosSetting.NewDistance,
            PosSetting.ZoomSmooth * Time.deltaTime);
    }

    void LookAtTarget() {
        transform.rotation = Quaternion.LookRotation(TargetObject.position - transform.position);
    }

    void OrbitAroundTarget() {
        if (!OrbitInput) return;

        X += Input.GetAxis("Mouse X") * OrbitSetting.YOrbitSmooth;
        Y += Input.GetAxis("Mouse Y") * OrbitSetting.YOrbitSmooth;
        Y = ClampAngle(Y, OrbitSetting.YOrbitMin, OrbitSetting.YOrbitMax);
    }

    private float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}

[Serializable]
public class PositionSetting {
    public float DistanceFromTarget = -10;
    public bool Allowzoom = true;
    public float MinZoom = -30;
    public float MaxZoom = -60;
    public float ZoomSmooth = 100f;
    public float ZoomStep = 20;
    public bool SmoothFallow = true;
    public float Smooth = .05f;
    public float NewDistance = -40;
}

[Serializable]
public class OrbitSetting {
    public bool AllowOrbit = true;
    public float YRoatation = -180;
    public float XRotation = -60;

    public float YOrbitSmooth = 2;

    public float YOrbitMin;

    public float YOrbitMax;
    //public float smooth = .5f;
}

[Serializable]
public class InputSetting {
    public string MouseOrbit = "Mouse Orbit";
    public string MouseScroll = "Mouse ScrollWheel";
}