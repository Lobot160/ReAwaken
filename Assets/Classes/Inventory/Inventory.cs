using System.Collections.Generic;
using System;
using System.Collections;
using System.Text.Json;

//Stores inventory items for the player. 
//Each item is represented by a string (the item name) and an integer (the quantity of that item).
public class Inventory : IEnumerable<KeyValuePair<string, int>>
{
    const string itemDataFilePath = "Items/itemDat.json";

    public string ItemType { get;}
    public int Capacity { get; private set; } // -1 is no limit
    public int EnergyLimit { get; private set; } // -1 is no limit
    public int CurrentEnergy { get; private set; } = 0;

    public Dictionary<string, int> Items { get; }
    public int Count => Items.Count;

    public event Action<string, int> OnItemAdded;
    public event Action<string, int> OverCapacity;
    public event Action<string, int> OverEnergyLimit;

    public void AddItem(string itemName, int quantity = 1)
    {
        if (Capacity != -1 && Count + quantity > Capacity)
        {
            OverCapacity?.Invoke(itemName, quantity);
            return;
        }

        int energyCost = GetEnergyCost(itemName) * quantity;
        if (EnergyLimit != -1 && CurrentEnergy + energyCost > EnergyLimit)
        {
            OverEnergyLimit?.Invoke(itemName, quantity);
            return;
        }

        if (Items.ContainsKey(itemName))
        {
            Items[itemName] += quantity;
        }
        else
        {
            Items[itemName] = quantity;
        }

        OnItemAdded?.Invoke(itemName, quantity);
    }

    public int GetEnergyCost(string itemName)
    {
        return GetEnergyCost(ItemType, itemName);
    }

    public static int GetEnergyCost(string itemType, string itemName)
    {
        return JsonDocument.Parse(System.IO.File.ReadAllText(itemDataFilePath))
            .RootElement
            .GetProperty(itemType)
            .GetProperty(itemName)
            .GetProperty("energyCost")
            .GetInt32();
    }

    public static JsonElement GetStats(string itemType, string itemName)
    {
        return JsonDocument.Parse(System.IO.File.ReadAllText(itemDataFilePath))
            .RootElement
            .GetProperty(itemType)
            .GetProperty(itemName)
            .GetProperty("values");

    }

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Inventory()
    {
        Capacity = -1; // No capacity limit
        EnergyLimit = -1; // No energy limit
        Items = new Dictionary<string, int>();
    }

    public Inventory(string itemType, int capacity = -1, int energyLimit = -1)
    {
        
        if (capacity < -1)
        {
            throw new ArgumentException("Capacity cannot be negative.");
        }
        else if (capacity == -1)
        {
            Items = new Dictionary<string, int>();
        }
        else
        {
            Items = new Dictionary<string, int>(capacity);
        }

        if (energyLimit < -1)
        {
            throw new ArgumentException("Energy limit cannot be negative.");
        }

        ItemType = itemType;
        Capacity = capacity;
        EnergyLimit = energyLimit;

        
    }

}