using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace NOX
{
    public class ItemSlot : MonoBehaviour
    {

        [SerializeField] public Image itemImage;
        [SerializeField] public Image itemAmountBG;
        [SerializeField] public TextMeshProUGUI itemAmountBGText;
        [SerializeField] public int itemAmount;
        [SerializeField] public Item currentItem;

        [Header("RakeUI Variables")]
        [SerializeField] Transform rakeUIParent;
        [SerializeField] Transform rakeUIMeleeParent;
        [SerializeField] Transform rakeUIHelmetParent;
        [SerializeField] Animator rakeUIAnim;

        [Header("Rake Variables")]
        [SerializeField] Transform rakeParent;
        [SerializeField] Transform rakeMeleeParent;
        [SerializeField] Transform rakeHelmetParent;
        [SerializeField] Animator rakeAnim;

        [SerializeField] ScrollRect scrollRect;


        ItemToolTip itemToolTip;
        GameDataBase gameDataBase;
        ItemSlotParent itemSlotParent;
        RectTransform itemSlotParentTransform;
        DragSlot dragSlot;
        InventoryPanel inventoryPanel;
        PlayerController player;
        
        [SerializeField] EquipmentSlot[] equipmentSlots;


        private void Start()
        {
            IdentifyCaches();
            DisplayCurrentItem();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                dragSlot.isDragging = false;
                scrollRect.vertical = true;
                dragSlot.transform.position = new Vector3(800, 650, -116.8982f);
            }
        }

        private void OnMouseOver()
        {
            DisplayToolTip();

            if (Input.GetMouseButtonDown(1))
            {
                EquipItem();
                StartCoroutine(ResetAnimRebind());
                inventoryPanel.DisplayItems();
                DisplayCurrentItem();
            }


            if (Input.GetMouseButtonDown(0) && currentItem)
            {
                IntiateDrag();
                scrollRect.vertical = false;
            }
        }

        private void OnMouseEnter()
        {
            dragSlot.isOnTopItemSlot = true;
        }

        private void OnMouseExit()
        {
            ResetToolTip();
            dragSlot.isOnTopItemSlot = false;
        }

        private void IdentifyCaches()
        {
            itemToolTip = FindObjectOfType<ItemToolTip>();
            gameDataBase = FindObjectOfType<GameDataBase>();
            itemSlotParent = FindObjectOfType<ItemSlotParent>();
            inventoryPanel = FindObjectOfType<InventoryPanel>();
            dragSlot = FindObjectOfType<DragSlot>();
            equipmentSlots = FindObjectsOfType<EquipmentSlot>();

            itemSlotParentTransform = itemSlotParent.GetComponent<RectTransform>();
            scrollRect = GetComponentInParent<ScrollRect>();

            rakeUIParent = FindObjectOfType<PrepRakeParent>().transform;
            rakeUIAnim = rakeUIParent.GetComponent<Animator>();
            rakeUIHelmetParent = FindObjectOfType<PrepHelmetParent>().transform;
            rakeUIMeleeParent = FindObjectOfType<PrepMeleeParent>().transform;

            rakeParent = FindObjectOfType<RakeParent>().transform;
            rakeAnim = rakeParent.GetComponent<Animator>();
            rakeHelmetParent = FindObjectOfType<HelmetParent>().transform;
            rakeMeleeParent = FindObjectOfType<MeleeParent>().transform;

            player = FindObjectOfType<PlayerController>();

 
        }

        public void DisplayCurrentItem()
        {
            if (currentItem)
            {
                itemImage.gameObject.SetActive(true);
                itemAmountBG.gameObject.SetActive(true);

                itemImage.sprite = currentItem.itemSprite;

                var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentItem.itemID && o.itemCategory == currentItem.category);
                itemAmountBGText.text = foundInventoryItem.itemCount.ToString();
            }
            else
            {
                itemImage.gameObject.SetActive(false);
                itemAmountBG.gameObject.SetActive(false);
            }    
        }

        private void DisplayToolTip()
        {
            if(currentItem)
            {

                #region Reposition Tooltip
                if (Input.mousePosition.y > 510)
                {
                    itemSlotParentTransform.pivot = new Vector2(itemSlotParentTransform.pivot.x, 1);
                    itemToolTip.transform.position = new Vector3(transform.position.x + 1.30f, transform.position.y - 0.27f, transform.position.z);
                }
                else
                {
                    itemSlotParentTransform.pivot = new Vector2(itemSlotParentTransform.pivot.x, 0.5f);
                    itemToolTip.transform.position = new Vector3(transform.position.x + 1.30f, transform.position.y, transform.position.z);
                }
                
                #endregion

                #region Display Item Tooltip Texts
                //1: Display Item Name
                itemToolTip.itemNameText.text = currentItem.itemName;
                itemToolTip.itemNameText.SetAllDirty();

                //2: Display Item Rarity
                if (currentItem.rarity == ItemRarity.common)
                {
                    itemToolTip.itemRarirtyText.text = "Common";
                    itemToolTip.itemRarirtyText.color = gameDataBase.commonColor;
                    itemToolTip.itemNameText.color = gameDataBase.commonColor;
                }
                if (currentItem.rarity == ItemRarity.uncommon)
                {
                    itemToolTip.itemRarirtyText.text = "Uncommon";
                    itemToolTip.itemRarirtyText.color = gameDataBase.uncommonColor;
                    itemToolTip.itemNameText.color = gameDataBase.uncommonColor;
                }
                if (currentItem.rarity == ItemRarity.rare)
                {
                    itemToolTip.itemRarirtyText.text = "Rare";
                    itemToolTip.itemRarirtyText.color = gameDataBase.rareColor;
                    itemToolTip.itemNameText.color = gameDataBase.rareColor;
                }
                if (currentItem.rarity == ItemRarity.epic)
                {
                    itemToolTip.itemRarirtyText.text = "Epic";
                    itemToolTip.itemRarirtyText.color = gameDataBase.epicColor;
                    itemToolTip.itemNameText.color = gameDataBase.epicColor;
                }
                if (currentItem.rarity == ItemRarity.legendary)
                {
                    itemToolTip.itemRarirtyText.text = "Legendary";
                    itemToolTip.itemRarirtyText.color = gameDataBase.legendaryColor;
                    itemToolTip.itemNameText.color = gameDataBase.legendaryColor;
                }
                itemToolTip.itemRarirtyText.SetAllDirty();

                //Display Item Category
                if (currentItem.category == ItemCategory.amulet)
                {
                    itemToolTip.itemTypeText.text = "Amulet";
                }
                if (currentItem.category == ItemCategory.helmet)
                {
                    itemToolTip.itemTypeText.text = "Helmet";
                }
                if (currentItem.category == ItemCategory.chest)
                {
                    itemToolTip.itemTypeText.text = "Chest Armor";
                }
                if (currentItem.category == ItemCategory.pants)
                {
                    itemToolTip.itemTypeText.text = "Leg Armor";
                }
                if (currentItem.category == ItemCategory.boots)
                {
                    itemToolTip.itemTypeText.text = "Boots";
                }
                if (currentItem.category == ItemCategory.melee)
                {
                    itemToolTip.itemTypeText.text = "Melee Weapon";
                }
                if (currentItem.category == ItemCategory.ranged)
                {
                    itemToolTip.itemTypeText.text = "Ranged Weapon";
                }
                if (currentItem.category == ItemCategory.ammo)
                {
                    itemToolTip.itemTypeText.text = "Ammo";
                }
                if (currentItem.category == ItemCategory.misc)
                {
                    itemToolTip.itemTypeText.text = "Misc";
                }
                if (currentItem.category == ItemCategory.quest)
                {
                    itemToolTip.itemTypeText.text = "Quest Item";
                }

                itemToolTip.itemTypeText.SetAllDirty();

                //Display Stat Text
                if(currentItem.powerBonus > 0 && currentItem.staminaBonus == 0 && currentItem.agilityBonus == 0)
                {
                    itemToolTip.statText.text = "Power: " + currentItem.powerBonus;
                }
                else if(currentItem.powerBonus > 0 && currentItem.staminaBonus > 0 && currentItem.agilityBonus == 0)
                {
                    itemToolTip.statText.text = "Power: " + currentItem.powerBonus + " " + "Stamina: " + currentItem.staminaBonus;
                }
                else if (currentItem.powerBonus > 0 && currentItem.staminaBonus == 0 && currentItem.agilityBonus > 0)
                {
                    itemToolTip.statText.text = "Power: " + currentItem.powerBonus + " " + "Agility: " + currentItem.agilityBonus;
                }
                else if(currentItem.powerBonus > 0 && currentItem.staminaBonus > 0 && currentItem.agilityBonus > 0)
                {
                    itemToolTip.statText.text = "Power: " + currentItem.powerBonus + " " + "Stamina: " + currentItem.staminaBonus + " " + "Agility: " + currentItem.agilityBonus;
                }
                else if(currentItem.powerBonus == 0 && currentItem.staminaBonus > 0 && currentItem.agilityBonus == 0)
                {
                    itemToolTip.statText.text = "Stamina: " + currentItem.staminaBonus;
                }
                else if (currentItem.powerBonus == 0 && currentItem.staminaBonus > 0 && currentItem.agilityBonus > 0)
                {
                    itemToolTip.statText.text = "Stamina: " + currentItem.staminaBonus + " " + "Agility: " + currentItem.agilityBonus;
                }
                else if (currentItem.powerBonus == 0 && currentItem.staminaBonus == 0 && currentItem.agilityBonus > 0)
                {
                    itemToolTip.statText.text = "Agility: " + currentItem.agilityBonus;
                }


                itemToolTip.statText.SetAllDirty();

                //Display Item Info
                itemToolTip.itemInfoText.text = currentItem.itemInfo;

                itemToolTip.itemInfoText.SetAllDirty();

                #endregion

            }
        }

        private void ResetToolTip()
        {
            itemToolTip.transform.localPosition = new Vector3(-508, 1500, 0);
        }

        private void EquipItem()
        {

            if (currentItem != null)
            {
                //Amulet Slot
                if (currentItem.category == ItemCategory.amulet)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentItem.itemID && o.itemCategory == currentItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentItem.itemID, itemCategory = currentItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        ApplyStatBonuses();

                        //Remove item from inventory data
                        if (foundInventoryItem.itemCount > 1)
                        {
                            foundInventoryItem.itemCount -= 1;
                        }
                        else
                        {
                            SaveData.Instance().inventoryItems.Remove(foundInventoryItem);
                        }

                        //Add to equipment data

                        //Arrange equipment slot
                        var amuletEquipmentSlot = equipmentSlots.FirstOrDefault(o => o.itemCategory == ItemCategory.amulet);

                        if (amuletEquipmentSlot.equippedItem == null)
                        {
                            amuletEquipmentSlot.equippedItem = currentItem;
                            amuletEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                        else
                        {
                            var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault
                                (o => o.itemID == amuletEquipmentSlot.equippedItem.itemID && o.itemCategory == amuletEquipmentSlot.equippedItem.category);

                            if (foundEquippedItemInInventory != null) //if there is already a item of same kind in the inventory
                            {
                                foundEquippedItemInInventory.itemCount += 1;
                            }
                            else
                            {
                                SaveData.Instance().inventoryItems.Add(new InventorySaveData
                                { itemID = amuletEquipmentSlot.equippedItem.itemID, itemCategory = amuletEquipmentSlot.equippedItem.category, itemCount = 1 });
                            }

                            var foundEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault
                                (o => o.itemID == amuletEquipmentSlot.equippedItem.itemID && o.itemCategory == amuletEquipmentSlot.equippedItem.category);

                            player.currentPower -= amuletEquipmentSlot.equippedItem.powerBonus;
                            player.currentStamina -= amuletEquipmentSlot.equippedItem.staminaBonus;
                            player.currentAgility -= amuletEquipmentSlot.equippedItem.agilityBonus;

                            SaveData.Instance().equippedItems.Remove(foundEquippedItem);

                            player.ShowStats();

                            amuletEquipmentSlot.equippedItem = currentItem;
                            amuletEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Helmet Slot
                if (currentItem.category == ItemCategory.helmet)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentItem.itemID && o.itemCategory == currentItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentItem.itemID, itemCategory = currentItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        //Add 3D Object
                        var foundHelmet3DObject = Instantiate(currentItem.threeDObject, rakeUIHelmetParent);
                        var foundHelmet3DO = Instantiate(currentItem.threeDObject, rakeHelmetParent);
                        ApplyStatBonuses();

                        //Remove item from inventory data
                        if (foundInventoryItem.itemCount > 1)
                        {
                            foundInventoryItem.itemCount -= 1;
                        }
                        else
                        {
                            SaveData.Instance().inventoryItems.Remove(foundInventoryItem);
                        }

                        //Add to equipment data

                        //Arrange equipment slot
                        var helmetEquipmentSlot = equipmentSlots.FirstOrDefault(o => o.itemCategory == ItemCategory.helmet);

                        if (helmetEquipmentSlot.equippedItem == null)
                        {
                            helmetEquipmentSlot.equippedItem = currentItem;
                            helmetEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                        else
                        {
                            var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault
                                (o => o.itemID == helmetEquipmentSlot.equippedItem.itemID && o.itemCategory == helmetEquipmentSlot.equippedItem.category);

                            if (foundEquippedItemInInventory != null) //if there is already a item of same kind in the inventory
                            {
                                foundEquippedItemInInventory.itemCount += 1;
                            }
                            else
                            {
                                SaveData.Instance().inventoryItems.Add(new InventorySaveData
                                { itemID = helmetEquipmentSlot.equippedItem.itemID, itemCategory = helmetEquipmentSlot.equippedItem.category, itemCount = 1 });
                            }

                            var foundEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault
                                (o => o.itemID == helmetEquipmentSlot.equippedItem.itemID && o.itemCategory == helmetEquipmentSlot.equippedItem.category);

                            player.currentPower -= helmetEquipmentSlot.equippedItem.powerBonus;
                            player.currentStamina -= helmetEquipmentSlot.equippedItem.staminaBonus;
                            player.currentAgility -= helmetEquipmentSlot.equippedItem.agilityBonus;

                            SaveData.Instance().equippedItems.Remove(foundEquippedItem);

                            if (foundEquippedItem.itemCategory == ItemCategory.helmet)
                            {
                                Destroy(rakeUIHelmetParent.GetChild(0).gameObject);
                                Destroy(rakeHelmetParent.GetChild(0).gameObject);
                            }

                            player.ShowStats();

                            helmetEquipmentSlot.equippedItem = currentItem;
                            helmetEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Chest Slot
                if (currentItem.category == ItemCategory.chest)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentItem.itemID && o.itemCategory == currentItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData {itemID = currentItem.itemID, itemCategory = currentItem.category, itemCount = 1});

                    if (foundInventoryItem != null)
                    {
                        // ////Add 3D Object
                        //UIRake
                        var foundChest3DObject = Instantiate(currentItem.threeDObject, rakeUIParent);
                        foundChest3DObject.name = "Chest";
                        //Rake
                        var foundChest3DO = Instantiate(currentItem.threeDObject, rakeParent);
                        foundChest3DO.name = "Chest";

                        ApplyStatBonuses();

                        // ////Add 3D Object

                        //Remove item from inventory data
                        if (foundInventoryItem.itemCount > 1)
                        {
                            foundInventoryItem.itemCount -= 1;
                        }
                        else
                        {
                            SaveData.Instance().inventoryItems.Remove(foundInventoryItem);
                        }

                        //Add to equipment data
                        
                        //Arrange equipment slot
                        var chestEquipmentSlot = equipmentSlots.FirstOrDefault(o => o.itemCategory == ItemCategory.chest);

                        if(chestEquipmentSlot.equippedItem == null)
                        {
                            chestEquipmentSlot.equippedItem = currentItem;
                            chestEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                        else
                        {
                            rakeUIAnim.Rebind();
                            var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault
                                (o => o.itemID == chestEquipmentSlot.equippedItem.itemID && o.itemCategory == chestEquipmentSlot.equippedItem.category);

                            if (foundEquippedItemInInventory != null) //if there is already a item of same kind in the inventory
                            {
                                foundEquippedItemInInventory.itemCount += 1;
                            }
                            else
                            {
                                SaveData.Instance().inventoryItems.Add(new InventorySaveData
                                {itemID = chestEquipmentSlot.equippedItem.itemID, itemCategory = chestEquipmentSlot.equippedItem.category, itemCount = 1});
                            }

                            var foundEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault
                                (o => o.itemID == chestEquipmentSlot.equippedItem.itemID && o.itemCategory == chestEquipmentSlot.equippedItem.category);

                            player.currentPower -= chestEquipmentSlot.equippedItem.powerBonus;
                            player.currentStamina -= chestEquipmentSlot.equippedItem.staminaBonus;
                            player.currentAgility -= chestEquipmentSlot.equippedItem.agilityBonus;

                            SaveData.Instance().equippedItems.Remove(foundEquippedItem);

                            if (foundEquippedItem.itemCategory == ItemCategory.chest)
                            {
                                Clear3DObject("Chest");
                                Clear3DO("Chest");
                            }


                            player.ShowStats();

                            chestEquipmentSlot.equippedItem = currentItem;
                            chestEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Pants Slot
                if (currentItem.category == ItemCategory.pants)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentItem.itemID && o.itemCategory == currentItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentItem.itemID, itemCategory = currentItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        // ////Add 3D Object
                        //UIRake
                        var foundPants3DObject = Instantiate(currentItem.threeDObject, rakeUIParent);
                        foundPants3DObject.name = "Pants";
                        //Rake
                        var foundPants3DO = Instantiate(currentItem.threeDObject, rakeParent);
                        foundPants3DO.name = "Pants";

                        ApplyStatBonuses();

                        // ////Add 3D Object

                        //Remove item from inventory data
                        if (foundInventoryItem.itemCount > 1)
                        {
                            foundInventoryItem.itemCount -= 1;
                        }
                        else
                        {
                            SaveData.Instance().inventoryItems.Remove(foundInventoryItem);
                        }

                        //Add to equipment data

                        //Arrange equipment slot
                        var pantsEquipmentSlot = equipmentSlots.FirstOrDefault(o => o.itemCategory == ItemCategory.pants);

                        if (pantsEquipmentSlot.equippedItem == null)
                        {
                            pantsEquipmentSlot.equippedItem = currentItem;
                            pantsEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                        else
                        {
                            rakeUIAnim.Rebind();
                            var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault
                                (o => o.itemID == pantsEquipmentSlot.equippedItem.itemID && o.itemCategory == pantsEquipmentSlot.equippedItem.category);

                            if (foundEquippedItemInInventory != null) //if there is already a item of same kind in the inventory
                            {
                                foundEquippedItemInInventory.itemCount += 1;
                            }
                            else
                            {
                                SaveData.Instance().inventoryItems.Add(new InventorySaveData
                                { itemID = pantsEquipmentSlot.equippedItem.itemID, itemCategory = pantsEquipmentSlot.equippedItem.category, itemCount = 1 });
                            }

                            var foundEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault
                                (o => o.itemID == pantsEquipmentSlot.equippedItem.itemID && o.itemCategory == pantsEquipmentSlot.equippedItem.category);

                            player.currentPower -= pantsEquipmentSlot.equippedItem.powerBonus;
                            player.currentStamina -= pantsEquipmentSlot.equippedItem.staminaBonus;
                            player.currentAgility -= pantsEquipmentSlot.equippedItem.agilityBonus;

                            SaveData.Instance().equippedItems.Remove(foundEquippedItem);

                            if (foundEquippedItem.itemCategory == ItemCategory.pants)
                            {
                                Clear3DObject("Pants");
                                Clear3DO("Pants");
                            }


                            player.ShowStats();

                            pantsEquipmentSlot.equippedItem = currentItem;
                            pantsEquipmentSlot.DisplayEquippedItem();

                            return;
                        }
                    }
                }

                //Boots Slot
                if (currentItem.category == ItemCategory.boots)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentItem.itemID && o.itemCategory == currentItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentItem.itemID, itemCategory = currentItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        // ////Add 3D Object
                        //UIRake
                        var foundBoots3DObject = Instantiate(currentItem.threeDObject, rakeUIParent);
                        foundBoots3DObject.name = "Boots";
                        //Rake
                        var foundBoots3DO = Instantiate(currentItem.threeDObject, rakeParent);
                        foundBoots3DO.name = "Boots";

                        ApplyStatBonuses();

                        // ////Add 3D Object

                        //Remove item from inventory data
                        if (foundInventoryItem.itemCount > 1)
                        {
                            foundInventoryItem.itemCount -= 1;
                        }
                        else
                        {
                            SaveData.Instance().inventoryItems.Remove(foundInventoryItem);
                        }

                        //Add to equipment data

                        //Arrange equipment slot
                        var bootsEquipmentSlot = equipmentSlots.FirstOrDefault(o => o.itemCategory == ItemCategory.boots);

                        if (bootsEquipmentSlot.equippedItem == null)
                        {
                            bootsEquipmentSlot.equippedItem = currentItem;
                            bootsEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                        else
                        {
                            rakeUIAnim.Rebind();
                            var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault
                                (o => o.itemID == bootsEquipmentSlot.equippedItem.itemID && o.itemCategory == bootsEquipmentSlot.equippedItem.category);

                            if (foundEquippedItemInInventory != null) //if there is already a item of same kind in the inventory
                            {
                                foundEquippedItemInInventory.itemCount += 1;
                            }
                            else
                            {
                                SaveData.Instance().inventoryItems.Add(new InventorySaveData
                                { itemID = bootsEquipmentSlot.equippedItem.itemID, itemCategory = bootsEquipmentSlot.equippedItem.category, itemCount = 1 });
                            }

                            var foundEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault
                                (o => o.itemID == bootsEquipmentSlot.equippedItem.itemID && o.itemCategory == bootsEquipmentSlot.equippedItem.category);

                            player.currentPower -= bootsEquipmentSlot.equippedItem.powerBonus;
                            player.currentStamina -= bootsEquipmentSlot.equippedItem.staminaBonus;
                            player.currentAgility -= bootsEquipmentSlot.equippedItem.agilityBonus;

                            SaveData.Instance().equippedItems.Remove(foundEquippedItem);

                            if (foundEquippedItem.itemCategory == ItemCategory.boots)
                            {
                                Clear3DObject("Boots");
                                Clear3DO("Boots");
                            }

                            player.ShowStats();

                            bootsEquipmentSlot.equippedItem = currentItem;
                            bootsEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Melee Slot
                if (currentItem.category == ItemCategory.melee)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentItem.itemID && o.itemCategory == currentItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentItem.itemID, itemCategory = currentItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        //Add 3D Object
                        var foundMelee3DObject = Instantiate(currentItem.threeDObject, rakeUIMeleeParent);
                        var foundMelee3DO = Instantiate(currentItem.threeDObject, rakeMeleeParent);

                        ApplyStatBonuses();

                        //Remove item from inventory data
                        if (foundInventoryItem.itemCount > 1)
                        {
                            foundInventoryItem.itemCount -= 1;
                        }
                        else
                        {
                            SaveData.Instance().inventoryItems.Remove(foundInventoryItem);
                        }

                        //Add to equipment data

                        //Arrange equipment slot
                        var meleeEquipmentSlot = equipmentSlots.FirstOrDefault(o => o.itemCategory == ItemCategory.melee);

                        if (meleeEquipmentSlot.equippedItem == null)
                        {
                            meleeEquipmentSlot.equippedItem = currentItem;
                            meleeEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                        else
                        {
                            var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault
                                (o => o.itemID == meleeEquipmentSlot.equippedItem.itemID && o.itemCategory == meleeEquipmentSlot.equippedItem.category);

                            if (foundEquippedItemInInventory != null) //if there is already a item of same kind in the inventory
                            {
                                foundEquippedItemInInventory.itemCount += 1;
                            }
                            else
                            {
                                SaveData.Instance().inventoryItems.Add(new InventorySaveData
                                { itemID = meleeEquipmentSlot.equippedItem.itemID, itemCategory = meleeEquipmentSlot.equippedItem.category, itemCount = 1 });
                            }

                            var foundEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault
                                (o => o.itemID == meleeEquipmentSlot.equippedItem.itemID && o.itemCategory == meleeEquipmentSlot.equippedItem.category);

                            player.currentPower -= meleeEquipmentSlot.equippedItem.powerBonus;
                            player.currentStamina -= meleeEquipmentSlot.equippedItem.staminaBonus;
                            player.currentAgility -= meleeEquipmentSlot.equippedItem.agilityBonus;

                            SaveData.Instance().equippedItems.Remove(foundEquippedItem);

                            if (foundEquippedItem.itemCategory == ItemCategory.melee)
                            {
                                Destroy(rakeUIMeleeParent.GetChild(0).gameObject);
                                Destroy(rakeMeleeParent.GetChild(0).gameObject);
                            }

                            player.ShowStats();

                            meleeEquipmentSlot.equippedItem = currentItem;
                            meleeEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }

                }

                //Ranged Slot
                /*if (currentItem.category == ItemCategory.ranged)
                {

                }*/
            }
        }

        private void IntiateDrag()
        {
            dragSlot.isDragging = true;
            dragSlot.currentDragItem = currentItem;
            dragSlot.itemImage.sprite = currentItem.itemSprite;
        }

        private void Clear3DObject(string type)
        {

            var threeDObjects = rakeUIParent.GetComponentsInChildren<ThreeDObject>();

            var foundThree3DObject = threeDObjects.FirstOrDefault(o => o.name == type);
            Destroy(foundThree3DObject.gameObject);
        }

        private void Clear3DO(string type)
        {

            var threeDO = rakeParent.GetComponentsInChildren<ThreeDObject>();

            var foundThree3DObject = threeDO.FirstOrDefault(o => o.name == type);
            Destroy(foundThree3DObject.gameObject);
        }

        private void ApplyStatBonuses()
        {
            player.currentPower += currentItem.powerBonus;
            player.currentStamina += currentItem.staminaBonus;
            player.currentAgility += currentItem.agilityBonus;

            player.ShowStats();
        }


        IEnumerator ResetAnimRebind()
        {
            yield return new WaitForSeconds(0.01f);
            rakeUIAnim.Rebind();
            rakeAnim.Rebind();
        }


    }
}


