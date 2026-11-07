using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UtilityItem: MonoBehaviour
{
    [SerializeField] private TargeChooseUtility targetChooseDefault;
    [SerializeField] private TargeChooseUtility targetChooseSmart;
    
    [SerializeField] private GameObject chosenTargetSprite;
    
    [SerializeField] private float afterHookDistance;
    [SerializeField] private float pullSpeed;
    
    public float swingTime = 0.2f;
    public float pullTime = 0.3f;

    private HashSet<IHookable> hookables = new HashSet<IHookable>();

    public void UpdateLookupDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void Hook(bool isSmart)
    {
        if (isSmart)
        {
            HookSmart();
        }
        else
        {
            HookDefault();
        }
    }

    public void HookDefault()
    {
        IHookable target = targetChooseDefault.GetTarget();
       
        if (target != null)
        {
            Vector2 direction =  target.GetPosition() - (Vector2) this.transform.position;
            target.Hook(this.transform.position,  direction, afterHookDistance, pullSpeed);
        }
    }

    public void HookSmart()
    {
        IHookable target = targetChooseSmart.GetTarget();
        
        if (target != null)
        {
            Vector2 direction = target.GetPosition() - (Vector2) this.transform.position;
            target.Hook(this.transform.position,  direction, afterHookDistance, pullSpeed);
        }
        else
        {
            HookDefault();
        }
    }

    public void ShowChosenTargetSprite()
    {
        chosenTargetSprite.SetActive(true);
    }

    public void UpdateChosenTargetSpritePosition(bool isSmart)
    {
        IHookable target = null;
        if (isSmart)
        {
            target = targetChooseSmart.GetTarget();
        }

        if (target == null)
        {
            target = targetChooseDefault.GetTarget();
        }
        
        if (target != null)
        {
            ShowChosenTargetSprite();
            chosenTargetSprite.transform.position = target.GetPosition() + new Vector2(0, 1.5f);
            //chosenTargetSprite.transform.position = target.GetPosition() + target.GetOffset();
        }
        else
        {
            HideChosenTargetSprite();
        }
    }

    public void HideChosenTargetSprite()
    {
        chosenTargetSprite.SetActive(false);
    }
    
}
