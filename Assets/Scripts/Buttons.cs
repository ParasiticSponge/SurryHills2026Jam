using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum TYPE
    {
        Play,
        Settings,
        Quit,
        Pause,
        Restart,
        Home
    }

    public TYPE type;

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.Play:
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                //animator.Play("pop");
                //Actions.MenuBeginSound.Invoke();
                Actions.Begin.Invoke();
                break;
            case TYPE.Settings:
                //StartCoroutine(SettingsPop());
                break;
            case TYPE.Quit:
                Application.Quit();
                break;
            case TYPE.Pause:
                Actions.pauseGame.Invoke();
                break;
            case TYPE.Restart:
                Actions.restartGame.Invoke();
                break;
            case TYPE.Home:
                SceneManager.LoadScene(0);
                break;
            //case TYPE.BOXOPTION1:
            //    Actions.TextBoxColour(0);
            //    break;
            //case TYPE.BOXOPTION2:
            //    Actions.TextBoxColour(1);
            //    break;
            //case TYPE.BOXOPTION3:
            //    Actions.TextBoxColour(2);
            //    break;
            //case TYPE.BOXOPTION4:
            //    Actions.TextBoxColour(3);
            //    break;
            //case TYPE.MENU:
            //    Actions.Settings.Invoke(false);
            //    break;
            //case TYPE.SFX_TEST:
            //    Actions.MenuBeginSound.Invoke();
            //    break;
            //case TYPE.CROSS:
            //    Actions.Toggles.Invoke(MenuManager_2.crossAssist, (value => MenuManager_2.crossAssist = value));
            //    break;
            //case TYPE.WIGGLE:
            //    Actions.Toggles.Invoke(MenuManager_2.wiggleCross, (value => MenuManager_2.wiggleCross = value));
            //    break;
            //case TYPE.DEV:
            //    StartCoroutine(DevPop());
            //    break;
            //case TYPE.MENUFromDev:
            //    Actions.Dev.Invoke(false);
            //    break;
            //case TYPE.GOTOMENU:
            //    SceneManager.LoadScene(0);
            //    break;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("hovering");
        if (type != TYPE.Pause || type != TYPE.Restart || type != TYPE.Home) Actions.Hover.Invoke(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (type != TYPE.Pause || type != TYPE.Restart || type != TYPE.Home) Actions.HoverExit.Invoke(gameObject);
    }
}
