using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class MenuManage : MonoBehaviour
{
    public int numberBaseMenu;
    public int numberAddMenu;
    public bool isActiveBaseMenu;
    public List<GameObject> ListMenu;
    public List<GameObject> ListAddMenu;
    public List<Animator> ListAnimButton;
    public List<bool> ListAnimButtonEnabled;
    public Animator animatorBackground;

    public bool isActiveNextMenu;

    public void EnabledMenu()
    {
        for (int i = 0; i < ListMenu.Count; i++)
        {
            ListMenu[i].SetActive(false);
        }
        for (int i = 0; i < ListAddMenu.Count; i++)
        {
            ListAddMenu[i].SetActive(false);
        }
        if (isActiveBaseMenu)
        {
            for (int i = 0; i < ListMenu.Count; i++)
            {
                ListAnimButtonEnabled[i] = false;
            }
            ListMenu[numberBaseMenu].SetActive(true);
            ListAnimButtonEnabled[numberBaseMenu] = true;
            isActiveNextMenu = true;
        }
        else
        {
            ListAddMenu[numberAddMenu].SetActive(true);
            isActiveNextMenu = true;
        }
    }
    public void SelectNumberBaseMenu(int selectNumber)
    {
        if (isActiveNextMenu && AddTradeOperation.Instance.IsFillInput)
        {
            isActiveBaseMenu = true;
            isActiveNextMenu = false;
            numberBaseMenu = selectNumber;
            animatorBackground.SetTrigger("Next");
            for (int i = 0; i < ListAnimButton.Count; i++)
            {
                if (ListAnimButtonEnabled[i])
                {
                    ListAnimButton[i].SetTrigger("Back");
                }
            }
            ListAnimButton[selectNumber].SetTrigger("Start");
        }
        else
        {
            AddTradeOperation.Instance.IsFillInput = true;
        }
    }

    public void SelectNumberAddMenu(int selectNumber)
    {
        if (isActiveNextMenu)
        {
            isActiveBaseMenu = false;
            isActiveNextMenu = false;
            numberAddMenu = selectNumber;
            animatorBackground.SetTrigger("Next");
        }
    }

}
