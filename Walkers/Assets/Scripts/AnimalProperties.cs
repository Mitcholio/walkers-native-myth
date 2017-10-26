using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Animals/NewAnimal", menuName = "New Animal", order = 1)]
public class AnimalProperties : ScriptableObject
{
    public string title;
    public GameObject model;
    public string text;
    public AudioClip clip;
    public Sprite sprite;

    public float moveSpeed;
    public float size;
}