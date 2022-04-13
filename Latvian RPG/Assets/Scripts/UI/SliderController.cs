using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SliderController : MonoBehaviour
{
    [SerializeField]
    string sliderValuePrefix;
    [SerializeField]
    Text sliderValueText;
    [SerializeField]
    AudioType sliderType;
    Slider slider;
    
    [SerializeField]
    AudioManager audioManager;

    private void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        OnSliderChanged(slider.value);
    }

    public void OnSliderChanged(float value)
    {
        sliderValueText.text = sliderValuePrefix + value.ToString() + "%";

        audioManager.SetVolume(value, sliderType);
    }
}
