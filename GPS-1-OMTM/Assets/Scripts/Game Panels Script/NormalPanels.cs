using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NormalPanels : MonoBehaviour
{
    public GameObject settingsPanel;
    public TutorialPhase tutorialPhase;
    public GameObject objectivePanel;
    public GameObject instruction4A;
    public GameObject skipTutorialButton;
    public Troop dps;
    public AudioManager audioManager;


    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        if (tutorialPhase != null && tutorialPhase.tutorialOn)
        {
            skipTutorialButton.SetActive(false);
            tutorialPhase.KillDummy();
            dps.Ultimate_DPS_End();
        }
        FindObjectOfType<AudioManager>().Play("button");
    }

    public void CloseSettingsPanel()
    {
        //FindObjectOfType<AudioManager>().Play("button");
        if (tutorialPhase != null && tutorialPhase.tutorialOn == true)
        {
            instruction4A.SetActive(false);
            objectivePanel.SetActive(true);
            settingsPanel.SetActive(false);
            Time.timeScale = 0f;
            //StartCoroutine(TutorialDelay());
            AudioManager.Instance.Play("button");
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }

    /*
    IEnumerator TutorialDelay()
    {
        settingsPanel.SetActive(false);

        yield return new WaitForSeconds(2f);

        objectivePanel.SetActive(true);
        //Time.timeScale = 0.0f;
    }
    */
}
