using Inputs;
using Invector.vCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleControl : NetworkBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public float Speed = 5.0f;
    public KnightThirdPersonInput cc;

    public AudioSource liteStrike;
    public AudioSource heavyStrike;

    [SerializeField]
    private InputReader inputReader;

    void Awake()
    {

    }

    public override void OnNetworkSpawn()
    {
        inputReader.AttackEvent += Attack;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        inputReader.AttackEvent -= Attack;
        base.OnNetworkDespawn();    
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Attack()
    {
        if (IsClient && IsOwner)
        {
            if (!animator.GetBool("IsGrounded") || animator.GetBool("Died")) return;
            if (!animator.GetBool("Attacking"))
            {
                animator.SetFloat("InputHorizontal", 0f);
                animator.SetFloat("InputVertical", 0f);
                animator.SetFloat("InputMagnitude", 0f);
                setAttackingTrue();
            }
            else
            {
                if (animator.GetBool("Interruptible"))
                {
                    setAttackAgainTrue();
                }

            }
        }
    }

    #region Animator Control
    public void setAttackingFalse()
    {
        animator.SetBool("Attacking", false);
        setInterruptibleTrue();
        setAttackAgainFalse();


    }
    public void setAttackingTrue()
    {
        animator.SetBool("Attacking", true);
        setInterruptibleFalse();

    }
    public void setAttackAgainFalse()
    {
        animator.SetBool("AttackAgain", false);

    }
    public void setAttackAgainTrue()
    {
        animator.SetBool("AttackAgain", true);

    }
    public void setInterruptibleFalse()
    {
        animator.SetBool("Interruptible", false);
        setAttackAgainFalse();

    }
    public void setInterruptibleTrue()
    {
        animator.SetBool("Interruptible", true);
    }

    #endregion

    #region Audio
    public void LiteStrikeSound()
    {
        liteStrike.Play();

    }
    public void HeaveyStrikeSound()
    {
        heavyStrike.Play();
    }
    #endregion
}
