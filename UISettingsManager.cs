using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsManager : MonoBehaviour
{
    public GameObject SettingsPanel;
    public Resolution[] resolutions;
    public Dropdown resolutionDropdown;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        //turn resolution into string
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            //if current resolution matches the option then save the index so dropdown value can be changed into it
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetScreenResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width,res.height,Screen.fullScreen);
    }

    public void CloseSettingsPanel()
    {
        SettingsPanel.SetActive(false);
        GetComponent<UIAnimationManager>().sideUI.SetActive(true);

    }

    public void OpenSettingsPanel()
    {
        GetComponent<UIAnimationManager>().sideUI.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void SetQualitySettings(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);          // NEEDS TO BE CHANGED TO URP Standard
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
