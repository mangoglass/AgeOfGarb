using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Instructions:
 *
 *          progressBar.MaxValue = 20f;
 *
 *          progressBar.CurrentValue = newValue;
 */

public class ProgressBar : MonoBehaviour
{
    private UnityEngine.UI.Slider slider;
    private Transform camera;

    private float maxValue;
    public float MaxValue {
        get {
            return maxValue;
        }
        set {
            maxValue = value;
            slider.value = currentValue / maxValue;
        }
    }

    private float currentValue;
    public float CurrentValue {
        get {
            return currentValue;
        }
        set {
            currentValue = value;
            slider.value = currentValue / maxValue;
        }
    }

    private void Start() {
        camera = Camera.main.transform;
    }

    private void Awake() {
        slider = gameObject.GetComponent<UnityEngine.UI.Slider>();
        currentValue = 0;
        maxValue = 20;
    }

    private void Update() {
        transform.LookAt(2f * transform.position - camera.position);
    }
}
