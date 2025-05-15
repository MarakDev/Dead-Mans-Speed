using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class Card : ScriptableObject
{
    [Header("Datos")]
    [SerializeField] public Sprite _artwork;

    [HideInInspector] public string _name;

    private void Awake()
    {
        _name = this.name;
    }
}
