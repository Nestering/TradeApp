using UnityEngine;
using System.Collections.Generic;

public class MenuAnimatorController : MonoBehaviour
{
    [SerializeField] private List<AnimationGroup> animationGroups;
    private int currentIndex = 0;

    public MenuManage menuManage;
    public void NextAnimation()
    {
        if (currentIndex >= animationGroups.Count+1)
        {
            menuManage.gameObject.SetActive(true);
            menuManage.SelectNumberBaseMenu(0);
            gameObject.SetActive(false);
            return;
        }
        if (animationGroups.Count == 0) return;

        SetTriggerForAll(currentIndex, "Next",false);
        currentIndex = currentIndex + 1; 
        SetTriggerForAll(currentIndex, "Start",true);
        
    }
    public void Skip()
    {
        menuManage.gameObject.SetActive(true);
        menuManage.SelectNumberBaseMenu(0);
        gameObject.SetActive(false);
    }
    private void SetTriggerForAll(int index, string trigger,bool enabled)
    {
        foreach (AnimationGroup animationGroup in animationGroups)
        {
            if (animationGroup.animators[index] != null)
            {
                if (enabled)
                {
                    animationGroup.animators[index].gameObject.SetActive(true);
                }
                animationGroup.animators[index].SetTrigger(trigger);
            }
            
        }
    }
}

[System.Serializable]
public class AnimationGroup
{
    public string name;
    public List<Animator> animators = new List<Animator>();
}
