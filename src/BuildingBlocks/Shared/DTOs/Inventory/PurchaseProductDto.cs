using Shared.Enums.Inventory;

namespace Shared.DTOs.Inventory;

public record PurchaseProductDto
{
    public EDocumentType DocumentType => EDocumentType.Purchase;

    private string _itemNo { get; set; }

    public int Quantity { get; set; }

    public string GetItemNo()
    {
        return _itemNo;
    }

    public void SetItemNo(string itemNo)
    {
        _itemNo = itemNo;
    }
}