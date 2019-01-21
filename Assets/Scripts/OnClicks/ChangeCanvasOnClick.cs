using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


public class ChangeCanvasOnClick : MonoBehaviour
{
    private const string MAIN_MENU_CANVAS_NAME = "MainMenuCanvas";
    private static Menu _activeMenu;

    #region PRIVATE_METHODS
    private void loadMainMenuAsActiveMenu()
    {
        GameObject gameObject = GameObject.Find(MAIN_MENU_CANVAS_NAME);
        if (gameObject == null)
            throw new System.Exception(MAIN_MENU_CANVAS_NAME + " not found");
        _activeMenu = gameObject.GetComponent<Menu>();
    }
    #endregion

    #region PUBLIC_METHODS
    public void changeTo(Canvas canvas)
    {
        //If not already set set active menu as MainMenu
        if (_activeMenu == null)
            loadMainMenuAsActiveMenu();
        
        //Set current menu inactive
        _activeMenu.setInactive();

        //Set new menu active
        _activeMenu = canvas.GetComponent<Menu>();
        _activeMenu.setActive();
    }

    public void show(Canvas canvas)
    {
        canvas.GetComponent<Menu>().setActive();
    }

    public void hide(Canvas canvas)
    {
        canvas.GetComponent<Menu>().setInactive();
    }
    #endregion
}
