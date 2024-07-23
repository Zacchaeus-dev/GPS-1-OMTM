using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject panelToShow; // The panel to show when hovering
    public TutorialPhase tutorialPhase;
    public bool isDPS;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tutorialPhase.tutorialOn && isDPS == false)
        {
            return;
        }

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(false);
        }
    }
}
