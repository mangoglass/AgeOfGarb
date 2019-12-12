using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Instructions:
 *
 * public class ProgressBarUser {
 *      [SerializeField]
 *      private GameObject progressBarPrefab;
 *      private ProgressBar progressBar;
 *
 *      public void Start() {
 *          GameObject newProgressBar = Instantiate(progressBarPrefab, parent.transform);
 *          newProgressBar.transform.localPosition = new Vector3(0, 5, 0);
 *          ProgressBar progressBar = newProgressBar.GetComponentInChildren<ProgressBar>();
 *          progressBar.MaxValue = 20f;
 *      }
 *
 *      public void OnProgressUpdate(float newValue) {
 *          progressBar.CurrentValue = newValue;
 *      }
 *  }
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
        maxValue = 100;
    }

    private void Update() {
        transform.LookAt(2f * transform.position - camera.position);
    }
}
