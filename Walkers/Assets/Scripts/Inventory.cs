using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<ItemProperties> Items = new List<ItemProperties>();
    public int size = 10;

	// Use this for initialization
	void Start ()
    {
		
	}

    public bool AddItem(ItemProperties _item)
    {
        bool _r = false;

        if (Items.Count < size)
        {
            Items.Add(_item);
            _r = true;
        }

        return _r;
    }

    public bool RemItem(ItemProperties _item)
    {
        bool _r = false;

        _r = Items.Remove(_item);

        return _r;
    }

    public ItemProperties GetItem(int _nr)
    {
        ItemProperties _item = null;
        if(_nr <= Items.Count-1)
            _item = Items[_nr];

        return _item;
    }

    public List<ItemProperties> GetItemList()
    {
        return Items;
    }
}
