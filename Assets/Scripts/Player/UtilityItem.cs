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
    
    [SerializeField] private Transform pivot;
    
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
            HookSmart(pivot);
        }
        else
        {
            HookDefault(pivot);
        }
    }

    public void HookDefault(Transform point)
    {
        IHookable target = targetChooseDefault.GetTarget(point);
       
        if (target != null)
        {
            Vector2 direction =  target.GetPosition() - (Vector2) this.transform.position;
            target.Hook(this.transform.position,  direction, afterHookDistance, pullSpeed);
        }
    }

    public void HookSmart(Transform point)
    {
        IHookable target = targetChooseSmart.GetTarget(point);
        
        if (target != null)
        {
            Vector2 direction = target.GetPosition() - (Vector2) this.transform.position;
            target.Hook(this.transform.position,  direction, afterHookDistance, pullSpeed);
        }
        else
        {
            HookDefault(point);
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
            target = targetChooseSmart.GetTarget(pivot);
        }

        if (target == null)
        {
            target = targetChooseDefault.GetTarget(pivot);
        }
        
        if (target != null && isSmart)
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
