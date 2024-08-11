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

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        if (tutorialPhase != null && tutorialPhase.tutorialOn)
        {
            tutorialPhase.KillDummy();
        }
        FindObjectOfType<AudioManager>().Play("button");
    }

    public void CloseSettingsPanel()
    {
        FindObjectOfType<AudioManager>().Play("button");
        if (tutorialPhase != null && tutorialPhase.tutorialOn == true)
        {
            instruction4A.SetActive(false);
            objectivePanel.SetActive(true);
            settingsPanel.SetActive(false);
            Time.timeScale = 0f;
            //StartCoroutine(TutorialDelay());
            
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
