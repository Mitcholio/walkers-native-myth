using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    ComponentList CL;
    PlayerController PC;
    PlayerActions PA;
    Inventory Inv;
    AudioSource audioSrc;

    public Text HoverInfoText;
    public Text curItemInfoText;
    public Text ControlsInfoText;

    public GameObject InvListingPrefab;
    public Button ListenButton;
    public Button StopListenButton;
    public Button DropButton;

    GameObject MiscGO;
    GameObject listenPanelGO;
    GameObject InventoryGO;

    Text InspectTitle;
    Text InspectDesc;
    Text InspectText;
    Slider AudioSlider;
    Text LPTitleText;
    Text LPCurTimeText;
    Text LPMaxTimeText;
    Text ATCurTimeText;
    Text ATMaxTimeText;

    bool listening = false;
    int listenButtonID;

    Camera cam;

    Transform InvPos;
    List<ButtonBehaviour> InventoryList = new List<ButtonBehaviour>();
    bool InvOpen = false;
    ButtonBehaviour SelectedButton;

    Coroutine audioSliderCor;

    // Use this for initialization
    void Start ()
    {
        StartVars();
        AddListeners();
	}

    void StartVars()
    {
        CL = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<ComponentList>();
        Inv = CL.Inv;
        PC = CL.PC;
        PA = CL.PA;
        cam = Camera.main;
        audioSrc = GetComponent<AudioSource>();
        ControlsInfoText.gameObject.SetActive(false);
        InventoryGO = GameObject.Find("Inventory");
        InvPos = GameObject.Find("InventoryParent").transform;
        InspectTitle = GameObject.Find("InspectTitle").GetComponent<Text>();
        InspectDesc = GameObject.Find("InspectDesc").GetComponent<Text>();
        InspectText = GameObject.Find("InspectText").GetComponent<Text>();
        AudioSlider = GameObject.Find("AudioSlider").GetComponent<Slider>();
        listenPanelGO = GameObject.Find("ListenPanel");
        LPTitleText = GameObject.Find("ListenPanelTitle").GetComponent<Text>();
        LPCurTimeText = GameObject.Find("ListenPanelCurTime").GetComponent<Text>();
        LPMaxTimeText = GameObject.Find("ListenPanelMaxTime").GetComponent<Text>();
        ATCurTimeText = GameObject.Find("AudioTimeCurTime").GetComponent<Text>();
        ATMaxTimeText = GameObject.Find("AudioTimeMaxTime").GetComponent<Text>();
        MiscGO = GameObject.Find("Misc");
        InventoryGO.SetActive(false);
        MiscGO.SetActive(true);
        AudioSlider.gameObject.SetActive(false);
        listenPanelGO.SetActive(false);
    }

    void AddListeners()
    {
        ListenButton.onClick.AddListener(OnListenClick);
        StopListenButton.onClick.AddListener(StopAudio);
        DropButton.onClick.AddListener(OnDropClick);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        HoverItemPopup();
        EquipInfo();
        RefreshListenPanel();
    }

    void Update()
    {
        ActControlsInfo();
        ActOpenInventory();
    }

    void ActOpenInventory()
    {
        if (!CL.IM.Inventory())
            return;

        if (InvOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    void OpenInventory()
    {
        InvOpen = true;
        InventoryGO.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CreateInvList();

        if (!listening)
        {
            int i = Inv.GetItemList().Count;
            if (i > 0)
                SelectInvButton(InventoryList[0].GetComponent<ButtonBehaviour>());
            else
                SelectInvButton(null);
        }
        else
        {
            SelectInvButton(InventoryList[listenButtonID]);
        }
    }

    void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InvOpen = false;
        InventoryGO.SetActive(false);
        DestroyInvList();
    }

    void CreateInvList()
    {
        List<ItemProperties> _list = Inv.GetItemList();
        int i = 0;
        int xPos = 0;
        int yPos = 0;
        foreach (ItemProperties _item in _list)
        {
            GameObject _listing = Instantiate(InvListingPrefab, InvPos);
            RectTransform _rt = _listing.GetComponent<RectTransform>();
            Image _image = _listing.GetComponentInChildren<Image>();
            Text _text = _listing.GetComponentInChildren<Text>();
            Button _button = _listing.GetComponent<Button>();
            ButtonBehaviour _BB = _listing.GetComponent<ButtonBehaviour>();
            AudioSource _audioSrc = _listing.GetComponent<AudioSource>();

            _BB.ID = i;
            _BB.mode = ButtonBehaviour.ButtonMode.inventory;
            _BB.item = _item;

            _rt.anchoredPosition += new Vector2(60 * xPos, -60 * yPos);
            _image.sprite = _item.sprite;

            InventoryList.Add(_BB);

            if (Inv.GetNewItemList().Contains(_item))
            {
                _BB.NewItem();
                Inv.RemoveNewItem(_item);
            }

            xPos++;

            if (xPos == 4)
            {
                yPos++;
                xPos = 0;
            }

            i++;
        }
    }

    void DestroyInvList()
    {
        foreach (ButtonBehaviour _BB in InventoryList)
        {
            Destroy(_BB.gameObject);
        }

        InventoryList = new List<ButtonBehaviour>();
    }

    public void OnInvButtonClick(ButtonBehaviour _BB)
    {
        ItemProperties _item = _BB.item;
        SelectInvButton(_BB);
    }

    void SelectInvButton(ButtonBehaviour _BB)
    {
        if(SelectedButton)
            SelectedButton.Deselect();

        if (_BB)
        {
            _BB.Select();
        }

        SelectedButton = _BB;

        RefreshInspectPanel();
    }

    void OnListenClick()
    {
        ItemProperties _item = SelectedButton.item;

        StopAudio();

        PlayAudio(_item.clip, (ulong)0.5);
    }

    void RefreshInspectPanel()
    {
        string _title = null;
        string _desc = null;
        string _text = null;
        ItemProperties _item;
        bool _active = false;
        string _curTime = 0.ToString("0.0");
        string _maxTime = null;

        if (SelectedButton)
        {
            _item = SelectedButton.item;
            _title = _item.title;
            _desc = _item.description;
            _text = _item.text;
            

            if (_item.clip)
            {
                _active = true;
                _maxTime = _item.clip.length.ToString("0.0");
            }
        }

        InspectTitle.text = _title;
        InspectDesc.text = _desc;
        InspectText.text = _text;

        ATCurTimeText.text = _curTime;
        ATMaxTimeText.text = _maxTime;

        AudioSlider.gameObject.SetActive(_active);
        ListenButton.gameObject.SetActive(_active);
        StopListenButton.gameObject.SetActive(_active);
    }

    void PlayAudio(AudioClip _clip, ulong delay)
    {
        listening = true;
        listenButtonID = SelectedButton.ID;

        audioSrc.clip = SelectedButton.item.clip;
        audioSrc.PlayDelayed(delay);

        StartAudioProgressBar();
        
        OpenListenPanel();
    }

    void StopAudio()
    {
        audioSrc.Stop();
        StopCor(audioSliderCor);
        AudioSlider.value = 0;
        ATCurTimeText.text = "0.0";
        ATMaxTimeText.text = "0.0";
        listening = false;
        CloseListenPanel();
    }

    void PauseAudio(AudioSource _src)
    {
        if (_src.isPlaying)
            _src.Pause();
        else
            _src.Play();
    }

    void StopCor(Coroutine _cor)
    {
        if (_cor != null)
            StopCoroutine(_cor);
    }

    void StartAudioProgressBar()
    {
        AudioSlider.minValue = 0;
        AudioSlider.maxValue = SelectedButton.item.clip.length;
        ATMaxTimeText.text = Math.Round(audioSrc.clip.length, 1).ToString("0.0");

        StopCor(audioSliderCor);
        audioSliderCor = StartCoroutine(RefreshAudioProgressBar());
    }

    IEnumerator RefreshAudioProgressBar()
    {
        yield return new WaitForFixedUpdate();

        float _clipLenght = SelectedButton.item.clip.length;

        do
        {
            if (InvOpen)
                if (SelectedButton == InventoryList[listenButtonID])
                {

                    AudioSlider.value = audioSrc.time;
                    ATCurTimeText.text = Math.Round(audioSrc.time, 1).ToString("0.0");
                }
                else
                {
                    AudioSlider.value = 0;
                }

            yield return new WaitForSeconds(0.05f);
        }
        while (audioSrc.time < _clipLenght && audioSrc.time > 0);

        StopAudio();
    }

    void OpenListenPanel()
    {
        listenPanelGO.SetActive(true);
        LPMaxTimeText.text = Math.Round(audioSrc.clip.length, 1).ToString("0.0");
        LPTitleText.text = InventoryList[listenButtonID].item.title;
    }

    void RefreshListenPanel()
    {
        if (!listening)
            return;
        LPCurTimeText.text = Math.Round(audioSrc.time, 1).ToString("0.0");
    }

    void CloseListenPanel()
    {
        listenPanelGO.SetActive(false);
    }

    void OnDropClick()
    {
        if (!SelectedButton)
            return;

        ItemProperties _item = SelectedButton.item;

        if (listenButtonID > SelectedButton.ID)
        {
            listenButtonID--;
        }
        else if (listenButtonID == SelectedButton.ID)
        {
            StopAudio();
            listenButtonID--;
        }

        PA.DropItem(_item);
        RefreshInv();
    }

    void RefreshInv()
    {
        DestroyInvList();
        CreateInvList();

        int i = SelectedButton.ID;
        i = Mathf.Clamp(i, 0, Inv.GetItemList().Count - 1);
        if (i >= 0)
            SelectInvButton(InventoryList[i]);
        else
            SelectInvButton(null);

    }

    void HoverItemPopup()
    {
        Item _item = null;

        Ray _ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, 2.2f, PA.GetPlayerMask()))
        {
            _item = _hit.transform.GetComponentInParent<Item>();
        }

        if (_item)
            HoverInfoText.text = _item.myItem.title;
        else
            HoverInfoText.text = "";
    }

    void EquipInfo()
    {
        if (!PA.EquipedItem)
        {
            curItemInfoText.text = "";
            return;
        }

        curItemInfoText.text = PA.EquipedItem.title;
    }

    void ActControlsInfo()
    {
        if (!CL.IM.Help())
        {
            return;
        }

        ControlsInfoText.gameObject.SetActive(!ControlsInfoText.gameObject.activeSelf);
    }

    public bool InvState()
    {
        return InvOpen;
    }
}
