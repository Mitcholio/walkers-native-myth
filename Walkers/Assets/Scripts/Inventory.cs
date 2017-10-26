using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<ItemProperties> Items = new List<ItemProperties>();
    public int size;

	// Use this for initialization
	void Start ()
    {
		
	}

    public bool AddItem(ItemProperties _item)
    {
        if (Items.Count < size)
        {
            Items.Add(_item);
            return true;
        }

        return false;
    }

    public bool RemItem(ItemProperties _item)
    {
        return Items.Remove(_item);
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
