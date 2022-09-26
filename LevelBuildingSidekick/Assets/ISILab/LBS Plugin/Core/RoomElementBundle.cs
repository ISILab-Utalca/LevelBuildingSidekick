using LBS;
using LBS.Generator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ISILab/LBS plugin/Element bundle", fileName = "New bundle")]
public class RoomElementBundle : ScriptableObject
{
    //private static readonly string[] defaultCategories = { "Walls", "Floors", "Doors", "Skies" };
    [SerializeField] private List<ItemCategory> categories = new List<ItemCategory>();

    public List<ItemCategory> GetCategories()
    {
        return new List<ItemCategory>(categories);
    }

    public ItemCategory GetCategory(string category)
    {
        return categories.Find(c => c.category == category);
    }

    public void AddCategory(ItemCategory bundle)
    {
        for (int i = 0; i < categories.Count; i++)
        {
            if (categories[i].category == bundle.category)
            {
                categories[i].items.AddRange(bundle.items);
                return;
            }
        }
        categories.Add(bundle);
    }

    public RoomElementBundle()
    {
        string[] defaultCategories = { "Walls", "Floors", "Doors", "Skies" };
        foreach (var cat in defaultCategories)
        {
            var c = new ItemCategory(cat);

            if (cat == "Walls" || cat == "Doors")
                c.pivotType = PivotType.Edge;

            categories.Add(c);
        }
    }

    public static RoomElementBundle Combine(List<RoomElementBundle> bundles)
    {
        var toReturn = new RoomElementBundle();
        foreach (var b in bundles)
        {
            foreach (var category in b.categories)
            {
                toReturn.AddCategory(category);
            }
        }
        return toReturn;
    }
}
