using DG.Tweening;
using Events;
using Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClickableFence : ClickableBase
{
    [Flags]
    public enum RBConstraints
    {
        None = 0,
        FreezePositionX = 2,
        FreezePositionY = 4,
        FreezePositionZ = 8,
        FreezeRotationX = 16,
        FreezeRotationY = 32,
        FreezeRotationZ = 64,
        FreezePosition = 14,
        FreezeRotation = 112,
        FreezeAll = 126
    }

    [SerializeField]
    private float liftDuration = 1.0f;
    [SerializeField]
    private Ease liftEase = Ease.InOutExpo;
    [SerializeField]
    private bool restoreToStart = false;
    [SerializeField]
    private Transform startPos;
    [SerializeField]
    private float restoreDuration = 3.0f;
    [SerializeField]
    private Transform targetPos;
    [SerializeField]
    RBConstraints idleConstraints;

    Rigidbody rb;
    Tween fenceTween;
    Animator animator;
    Vector3 startPosValue;
    Vector3 targetPosValue;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rb = GetComponent<Rigidbody>();
        rb.constraints = (RigidbodyConstraints)idleConstraints;
        animator = GetComponent<Animator>();
        EventManager.Instance.Subscribe<ClickableHintEvent>(OnHintEvent);
        startPosValue = startPos.position;
        targetPosValue = targetPos.position;
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe<ClickableHintEvent>(OnHintEvent);
        base.OnNetworkDespawn();
    }

    private void OnHintEvent(ClickableHintEvent evt)
    {
        if (evt.show)
        {
            animator.SetBool("show", true);
        }
        else
        {
            animator.SetBool("show", false);
        }
    }

    public override void OnClickStart()
    {
        Debug.Log("Fence Clicked");
        OnClickStartServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnClickStartServerRpc()
    {
        Debug.Log("Fence Clicked ServerRpc");
        fenceTween?.Kill();
        rb.isKinematic = true;
        //fenceTween = transform.DOMove(useTargetPos ? targetPosValue : move, liftDuration)
        //    .SetEase(liftEase)
        //    .OnComplete(() =>
        //    {
        //        rb.constraints = RigidbodyConstraints.FreezeAll;
        //    });
        fenceTween = DOTween.To(() => transform.position, x => transform.position = x, targetPosValue, liftDuration)
            .SetEase(liftEase)
            .OnComplete(() =>
            {
                rb.constraints = (RigidbodyConstraints)idleConstraints;
            });
    }

    public override void OnClickEnd()
    {
        Debug.Log("Fence Click Ended");
        OnClickEndServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnClickEndServerRpc()
    {
        fenceTween?.Kill();
        if (restoreToStart)
        {
            rb.isKinematic = true;
            fenceTween = DOTween.To(() => transform.position, x => transform.position = x, startPosValue, restoreDuration)
                .SetEase(liftEase)
                .OnComplete(() =>
                {
                    rb.constraints = (RigidbodyConstraints)idleConstraints;
                    rb.isKinematic = false;
                });
        }
        else
        {
            rb.constraints = (RigidbodyConstraints)idleConstraints;
            rb.isKinematic = false;
        }
    }
}
