using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StartRound : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void OnEnable()
    {
        StartCoroutine(StartAnimation());
    }

    public IEnumerator StartAnimation()
    {
        animator.Play("UI_StartRound");

        yield return new WaitForSeconds(0.8f);

        this.gameObject.SetActive(false);
    }
}
