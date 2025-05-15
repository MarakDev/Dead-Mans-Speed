using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LifePoint : MonoBehaviour
{
    [SerializeField] public List<GameObject> lifePoint;
    private void Start()
    {
        foreach (GameObject lifepoint in lifePoint)
        {
            lifepoint.GetComponent<Animator>().Play("UI_LifeBall_Idle");
        }
    }

    public void ResetLife()
    {
        foreach (GameObject lifepoint in lifePoint)
        {
            Animator animator = lifepoint.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName("UI_LifeBall_Idle"))
            {
                lifepoint.GetComponent<Animator>().Play("UI_LifeBall_Regenerate");
            }
        }
    }

    public void LoseLife(int i)
    {
        lifePoint[i].GetComponent<Animator>().Play("UI_LifeBall_Hurt");
    }

    public void AddLife(int i)
    {
        lifePoint[i].GetComponent<Animator>().Play("UI_LifeBall_Regenerate");
    }
}
