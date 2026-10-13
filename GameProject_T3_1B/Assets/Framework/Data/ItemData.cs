using UnityEngine;

namespace GameFramework.Data
{
    public enum ItemType { Consumable, Equipment, Material, Quest, Currency }

    [CreateAssetMenu(menuName = "GameFramework/Item Data", fileName = "Item_")]
    public class ItemData : ScriptableObject
    {
        [Header("아이템 데이터")]
        [Tooltip("아이템 추가할때는 ItemData 만든 후 꼭 Assets 폴더에 바로 있는 ItemDatabase에 추가할 것.")]
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public ItemType type;
        public int maxStack = 99;
        //public int sellPrice;
    }
}
