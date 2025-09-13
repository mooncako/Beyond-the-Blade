using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Shop Database", menuName = "ShopDatabase")]
public class ShopItemDatabaseSO : ScriptableObject
{
    public List<ShopItemSO> ShopItems = new List<ShopItemSO>();
}
