using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    private const float MAX_SPEED_ANGLE = -20;
    private const float ZERO_SPEED_ANGLE = 210;
    private Transform needleTransform;
    private Transform speedLabelTemplateTransform;
    private float speedMax;
    public CarController carController;
    private float speed;

    private void Awake(){
        needleTransform = GameObject.Find ("Needle").transform;
        speedLabelTemplateTransform = GameObject.Find ("SpeedLabelTemplate").transform;
        speedLabelTemplateTransform.gameObject.SetActive(false);

        speedMax = 1f;

        CrateSpeedLabels();
    }

    private void Update(){

        speed = carController.acceleration;

        //needleTransform.eulerAngles = new Vector3(0, 0, GetSpeedRotation());

        
       float desiredRotation = GetSpeedRotation();

        needleTransform.localRotation = Quaternion.Euler(0f, 0f, desiredRotation);


    }

    private void CrateSpeedLabels(){

        int labelAmount = 6;

        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        for(int i = 0; i<=labelAmount; i++){
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float labelSpeedNormalised = (float)i / labelAmount;
            float speedLabelAngle = ZERO_SPEED_ANGLE - labelSpeedNormalised * totalAngleSize;
            speedLabelTransform.eulerAngles = new Vector3(speedLabelTemplateTransform.eulerAngles.x,speedLabelTemplateTransform.eulerAngles.y,speedLabelAngle);

            Transform speedLabelChild = speedLabelTransform.Find("SpeedLabel");
            if (speedLabelChild != null) {
                speedLabelChild.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(labelSpeedNormalised * 120).ToString();
                speedLabelChild.eulerAngles = new Vector3(0f, 0f, 30f);
                speedLabelTransform.gameObject.SetActive(true);
            } else {
                Debug.LogError("SpeedLabel not found under speedLabelTransform");
            }
        }

        needleTransform.SetAsLastSibling();
    }

    private float GetSpeedRotation(){

        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalised  = speed / speedMax;

        return ZERO_SPEED_ANGLE - speedNormalised * totalAngleSize;
    }




}
