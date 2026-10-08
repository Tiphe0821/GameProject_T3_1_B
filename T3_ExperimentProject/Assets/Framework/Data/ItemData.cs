using UnityEngine;

namespace GameFramework.Data
{
    public enum ItemType { Consumable, Equipment, Material, Quest, Currency }

    [CreateAssetMenu(menuName = "GameFramework/Item Data", fileName = "Item_")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public ItemType type;
        public int maxStack = 99;       // 본 프로젝트에서 수정 필요할듯
        public int sellPrice;           // 필요한가? (아닐듯)
    }
}
