using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAnimationsManager : MonoBehaviour
{
    public Animator TroopAnimator;

    public void TroopIdle()
    {
        TroopAnimator.SetBool("walking", false);
        TroopAnimator.SetBool("climbing", false);
        TroopAnimator.SetBool("ulti", false);
        TroopAnimator.SetBool("attack", false);
    }    
    
    public void TroopIdleOn()
    {
        TroopAnimator.SetBool("idle", true);
    }    
    
    public void TroopIdleOff()
    {
        TroopAnimator.SetBool("idle", false);
    }
    
    public void TroopWalkOn()
    {
        TroopAnimator.SetBool("walking", true);
        TroopAnimator.SetBool("attack", false);
    }       
    
    public void TroopWalkOff()
    {
        TroopAnimator.SetBool("walking", false);    
    }        
    
    public void TroopClimbOn()
    {
        TroopAnimator.SetBool("climbing", true);
        TroopAnimator.SetBool("attack", false);
        //TroopAnimator.SetBool("walking", false);
    }      
    
    public void TroopClimbOff()
    {
        TroopAnimator.SetBool("climbing", false);
    }        
    
    public void TroopFallOn()
    {
        TroopAnimator.SetBool("falling", true);
        TroopAnimator.SetBool("attack", false);
        //TroopAnimator.SetBool("walking", false);
    }      
    
    public void TroopFallOff()
    {
        TroopAnimator.SetBool("falling", false);
    }    
    
    public void TroopAttackOn()
    {
/*        TroopAnimator.SetBool("walking", false);
        TroopAnimator.SetBool("climbing", false);
        TroopAnimator.SetBool("ulti", false);*/
        TroopAnimator.SetBool("attack", true);
    }        
    
    public void TroopAttackOff()
    {
        TroopAnimator.SetBool("attack", false);
    }    
    
    public void TroopUltiOn()
    {
        TroopAnimator.SetBool("walking", false);
        TroopAnimator.SetBool("climbing", false);
        TroopAnimator.SetBool("ulti", true);
        TroopAnimator.SetBool("attack", false);
    }    
    public void TroopUltiOff()
    {
        TroopAnimator.SetBool("ulti", false);
        TroopAnimator.SetBool("walking", false);
        TroopAnimator.SetBool("climbing", false);
        TroopAnimator.SetBool("attack", false);
    }

    public void TroopOnWeapon1()
    {
        TroopAnimator.SetBool("2nd Weapon", false);
    }    
    
    public void TroopOnWeapon2()
    {
        TroopAnimator.SetBool("2nd Weapon", true);
    }    
    
    public void TroopDies()
    {
        TroopAnimator.SetBool("death", true);
    }

    public void TroopRespawn()
    {
        TroopAnimator.SetBool("death", false);
    }
}
