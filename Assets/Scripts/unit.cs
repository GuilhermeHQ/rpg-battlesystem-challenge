using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit : MonoBehaviour
{
    public string unitName;
    public string unitLevel;

    public int damage;
    public Animator anim;

    public int maxHP;
    public int currentHP;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
            return true;
        else
            return false;

    }

    public void playerAttackAnim ()
    {
        anim.SetTrigger("PlayerAttack");
    }

    public void playerDamageAnim()
    {
        anim.SetTrigger("PlayerDamage");
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }
}
