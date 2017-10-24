using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Items/NewItem", menuName = "New Item", order = 1)]
public class ItemProperties : ScriptableObject
{
    public GameObject model;
    public AudioClip clip;
    public Sprite sprite;
    public string title;
}