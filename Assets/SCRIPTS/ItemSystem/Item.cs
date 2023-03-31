using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NOX
{
    public class Item : MonoBehaviour
    {
        [SerializeField] public string itemName;
        [SerializeField] public Sprite itemSprite;
        [SerializeField] public int itemID;
        [SerializeField] public ItemCategory category;
        [SerializeField] public ItemRarity rarity;
        [TextArea(9, 7)] public string itemInfo;
        [SerializeField] public int itemCount = 1;
        [Space]
        [SerializeField] public ThreeDObject threeDObject;

        [Header("Stat Bonuses")]
        [SerializeField] public float powerBonus;
        [SerializeField] public float agilityBonus;
        [SerializeField] public float staminaBonus;
        
    }
}
