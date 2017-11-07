using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Items/NewItem", menuName = "New Item", order = 1)]
public class ItemProperties : ScriptableObject
{
    public string title;
    public string description;
    public GameObject model;
    public string text;
    public AudioClip clip;
    public Sprite sprite;
}