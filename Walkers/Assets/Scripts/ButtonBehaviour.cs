using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour {

    PlayerUI PUI;
    public GameObject SelectPrefab;
    public GameObject NewItemPrefab;
    public enum ButtonMode { none, inventory }
    public ButtonMode mode;
    public int ID;
    public ItemProperties item;

    Button button;
    Text text;

    GameObject Selected_Model;
    GameObject NewItem_Model;

    float curAlpha = 0.1f;
    bool alphaDir;
    float alphaTime;

	// Use this for initialization
	void Start ()
    {
        StartVars();
        AddListeners();
    }

    void StartVars()
    {
        PUI = GetComponentInParent<PlayerUI>();
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    void AddListeners()
    {
        button.onClick.AddListener(ButtonOnClick);
    }

    void ButtonOnClick()
    {
        if (mode == ButtonMode.inventory)
            PUI.OnInvButtonClick(this);


    }

    public void Select()
    {
        Selected_Model = Instantiate(SelectPrefab, transform);
        OldItem();
    }

    public void Deselect()
    {
        Destroy(Selected_Model);
    }

    public void NewItem()
    {
        NewItem_Model = Instantiate(NewItemPrefab, transform);
        StartCoroutine(PulseNewItemModel());
    }

    public void OldItem()
    {
        Destroy(NewItem_Model);
    }

    IEnumerator PulseNewItemModel()
    {
        if (NewItem_Model)
        {
            Image _image = NewItem_Model.GetComponentInChildren<Image>();
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, curAlpha);

            if (Time.time > alphaTime +2)
            {
                alphaTime = Time.time;
                alphaDir = !alphaDir;
            }

            if (alphaDir)
                curAlpha += 0.01f;
            else
                curAlpha -= 0.01f;

            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            yield break;
        }

        StartCoroutine(PulseNewItemModel());

    }
}
