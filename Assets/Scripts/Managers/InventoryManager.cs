using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryManager", menuName = "Inventory/Manager")]
public class InventoryManager : ScriptableObject
{
    [Serializable]
    public struct InventorySlot
    {
        public ItemData item;
        public int quantity;

        public bool IsEmpty => item == null || quantity <= 0;
    }

    [HideInInspector] public InventorySlot[] slots;

    public event Action OnInventoryChanged;

    public void Initialize(int slotCount)
    {
        slots = new InventorySlot[slotCount];
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        // Tenta stackar em slot existente
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStack)
            {
                slots[i].quantity = Mathf.Min(slots[i].quantity + amount, item.maxStack);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Procura slot vazio
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i] = new InventorySlot { item = item, quantity = amount };
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false; // Inventário cheio
    }

    public bool RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return false;
        if (slots[slotIndex].IsEmpty) return false;

        slots[slotIndex].quantity -= amount;
        if (slots[slotIndex].quantity <= 0)
            slots[slotIndex] = default;

        OnInventoryChanged?.Invoke();
        return true;
    }
}