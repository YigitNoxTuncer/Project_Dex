using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace NOX
{
    public class InventoryPanel : MonoBehaviour
    {
        // /////////////////////////// STORED VARIABLES

        //[SerializeField] Transform itemSlotsParent;
        [SerializeField] List<ItemSlot> itemSlots;
        List<InventorySaveData> sortedItemsData;

        [Header("Save Data")]
        [SerializeField] List<InventorySaveData> invItems;
        [SerializeField] List<InventorySaveData> equippedItems;

        GameDataBase gameDataBase;

        // /////////////////////////// STORED VARIABLES

        private void Start()
        {
            gameDataBase = FindObjectOfType<GameDataBase>();
            DisplayItems();
        }

        private void Update()
        {
            TMP();
        }

        public void DisplayItems()
        {
            SortItems();

            foreach (ItemSlot itemSlot in itemSlots)
            {
                itemSlot.currentItem = null;
                itemSlot.DisplayCurrentItem();
            }

            for (int i = 0; i < sortedItemsData.Count; i++)
            {
                var foundItem = gameDataBase.allItems.FirstOrDefault(o => o.itemID == sortedItemsData[i].itemID && o.category == sortedItemsData[i].itemCategory);
                itemSlots[i].currentItem = foundItem;
                itemSlots[i].DisplayCurrentItem();
            }
        }

        private void SortItems()
        {
            sortedItemsData = new List<InventorySaveData>();

            var allHelmets = new List<Item>();

            foreach (Item item in gameDataBase.allItems)
            {
                if (item.category == ItemCategory.helmet)
                {
                    allHelmets.Add(item);
                }
            }

            var allChestArmors = new List<Item>();

            foreach (Item item in gameDataBase.allItems)
            {
                if (item.category == ItemCategory.chest)
                {
                    allChestArmors.Add(item);
                }
            }

            var allPants = new List<Item>();

            foreach (Item item in gameDataBase.allItems)
            {
                if (item.category == ItemCategory.pants)
                {
                    allPants.Add(item);
                }
            }

            var allBoots = new List<Item>();

            foreach (Item item in gameDataBase.allItems)
            {
                if (item.category == ItemCategory.boots)
                {
                    allBoots.Add(item);
                }
            }

            var allMeleeWeapons = new List<Item>();

            foreach (Item item in gameDataBase.allItems)
            {
                if (item.category == ItemCategory.melee)
                {
                    allMeleeWeapons.Add(item);
                }
            }

            var allAmulets = new List<Item>();

            foreach (Item item in gameDataBase.allItems)
            {
                if (item.category == ItemCategory.amulet)
                {
                    allAmulets.Add(item);
                }
            }

            for (int i = 0; i < allHelmets.Count + 1; i++)
            {
                var foundHelmetData = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == i && o.itemCategory == ItemCategory.helmet);

                if (foundHelmetData != null)
                {
                    sortedItemsData.Add(foundHelmetData);
                }
            }

            for (int i = 0; i < allChestArmors.Count + 1; i++)
            {
                var foundChestArmorData = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == i && o.itemCategory == ItemCategory.chest);

                if (foundChestArmorData != null)
                {
                    sortedItemsData.Add(foundChestArmorData);
                }
            }

            for (int i = 0; i < allPants.Count + 1; i++)
            {
                var foundPantsData = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == i && o.itemCategory == ItemCategory.pants);

                if (foundPantsData != null)
                {
                    sortedItemsData.Add(foundPantsData);
                }
            }

            for (int i = 0; i < allBoots.Count + 1; i++)
            {
                var foundBootsData = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == i && o.itemCategory == ItemCategory.boots);

                if (foundBootsData != null)
                {
                    sortedItemsData.Add(foundBootsData);
                }
            }


            for (int i = 0; i < allMeleeWeapons.Count + 1; i++)
            {
                var foundMeleeWeaponData = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == i && o.itemCategory == ItemCategory.melee);

                if (foundMeleeWeaponData != null)
                {
                    sortedItemsData.Add(foundMeleeWeaponData);
                }
            }

            for (int i = 0; i < allAmulets.Count + 1; i++)
            {
                var foundAmuletData = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == i && o.itemCategory == ItemCategory.amulet);

                if (foundAmuletData != null)
                {
                    sortedItemsData.Add(foundAmuletData);
                }
            }

        }

        private void TMP()
        {
            invItems = new List<InventorySaveData>();
            equippedItems = new List<InventorySaveData>();

            invItems = SaveData.Instance().inventoryItems;
            equippedItems = SaveData.Instance().equippedItems;
        }
    }
}

