using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    public int id;
    public Sprite icon;
}
