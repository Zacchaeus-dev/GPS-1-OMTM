using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject panelToShow; // The panel to show when hovering

    public void OnPointerEnter(PointerEventData eventData)
    {
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
