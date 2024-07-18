using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAnimationsManager : MonoBehaviour
{
    public Animator TroopAnimator;

    public void TroopIdle1()
    {
        TroopAnimator.SetBool("walking (1)", false);
        TroopAnimator.SetBool("climbing (1)", false);
        TroopAnimator.SetBool("ulti (1)", false);
        TroopAnimator.SetBool("attack (1)", false);
    }
    
    public void TroopWalkOn1()
    {
        TroopAnimator.SetBool("walking (1)", true);
        TroopAnimator.SetBool("attack (1)", false);
    }       
    
    public void TroopWalkOff1()
    {
        TroopAnimator.SetBool("walking (1)", false);    }        
    
    public void TroopClimbOn1()
    {
        TroopAnimator.SetBool("climbing (1)", true);
        TroopAnimator.SetBool("attack (1)", false);
    }      
    
    public void TroopClimbOff1()
    {
        TroopAnimator.SetBool("climbing (1)", false);
    }        
    
    public void TroopFallOn1()
    {
        TroopAnimator.SetBool("falling (1)", true);
        TroopAnimator.SetBool("attack (1)", false);
    }      
    
    public void TroopFallOff1()
    {
        TroopAnimator.SetBool("falling (1)", false);
    }    
    
    public void TroopAttack1()
    {
        TroopAnimator.SetBool("walking (1)", false);
        TroopAnimator.SetBool("climbing (1)", false);
        TroopAnimator.SetBool("ulti (1)", false);
        TroopAnimator.SetBool("attack (1)", true);
    }    
    
    public void TroopUlti1()
    {
        TroopAnimator.SetBool("walking (1)", false);
        TroopAnimator.SetBool("climbing (1)", false);
        TroopAnimator.SetBool("ulti (1)", true);
        TroopAnimator.SetBool("attack (1)", false);
    }

    public void TroopOnWeapon1()
    {
        TroopAnimator.SetBool("2nd Weapon", false);
    }    
    
    public void TroopOnWeapon2()
    {
        TroopAnimator.SetBool("2nd Weapon", true);
    }
}
