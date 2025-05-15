using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Hourglass : MonoBehaviour
{
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("UI_Hourglass_Idle");
        StopHourglass();
    }

    public void ContinueHourglass()
    {
        _animator.speed = 1;
    }

    public void StopHourglass()
    {
        _animator.speed = 0;
    }

    public void RestartAnimation()
    {
        _animator.Play("");
        _animator.Play("UI_Hourglass_Idle");
    }
}
