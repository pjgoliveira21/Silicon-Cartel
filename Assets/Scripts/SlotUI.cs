using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image selectionHighlight;

    public void Refresh(InventoryManager.InventorySlot slot)
    {
        bool hasItem = !slot.IsEmpty;

        iconImage.enabled = hasItem;
        quantityText.enabled = hasItem && slot.quantity > 1;

        if (hasItem)
        {
            iconImage.sprite = slot.item.icon;
            quantityText.text = slot.quantity.ToString();
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.enabled = selected;
    }
}