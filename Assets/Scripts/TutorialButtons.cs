using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialButtons : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public enum TYPE
    {
        Next,
        Previous
    }
    public TYPE type;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.Next:
                TutorialPage.changeNumber.Invoke(1);
                break;
            case TYPE.Previous:
                TutorialPage.changeNumber.Invoke(-1);
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
