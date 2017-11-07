using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<ItemProperties> Items = new List<ItemProperties>();
    public int size;

    List<ItemProperties> NewItems = new List<ItemProperties>();

    // Use this for initialization
    void Start ()
    {
		
	}

    public bool AddItem(ItemProperties _item)
    {
        if (Items.Count < size)
        {
            Items.Add(_item);
            SortItemsList();

            NewItems.Add(_item);
            return true;
        }

        return false;
    }

    public bool RemItem(ItemProperties _item)
    {
        bool _r = Items.Remove(_item);
        SortItemsList();

        NewItems.Remove(_item);
        return _r;
    }

    void SortItemsList()
    {
        Items = Items.OrderBy(i => i.title).ToList();
        //Items = Items.OrderBy(i => i.text).ThenBy(i => i.title).ToList();
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

    public List<ItemProperties> GetNewItemList()
    {
        return NewItems;
    }

    public void RemoveNewItem(ItemProperties _item)
    {
        NewItems.Remove(_item);
    }
}
