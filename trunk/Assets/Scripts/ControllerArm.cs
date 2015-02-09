using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerArm : MonoBehaviour {

    public Transform boneHumerus;
    public Transform boneRadius;

    public Slider sliBoneAngle;
    public Text txtBoneAngle;

    float angleCurrent;

	// Use this for initialization
	void Start () {
        sliBoneAngle.onValueChanged.AddListener(delegate { UpdateAngle(sliBoneAngle.value); });

        txtBoneAngle.text = "Degrees: " + angleCurrent.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        boneRadius.localRotation = Quaternion.Euler(angleCurrent, 90, 0);
	}

    public void UpdateAngle(float value) {
        angleCurrent = value;
        txtBoneAngle.text = "Degrees: " + angleCurrent.ToString();
    }
}
