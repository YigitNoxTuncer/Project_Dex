using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace NOX
{
    public class DragSlot : MonoBehaviour
    {
        [SerializeField] public Image itemImage;
        [SerializeField] public Item currentDragItem;

        [SerializeField] public bool isDragging;
        [SerializeField] public bool isOnTopItemSlot;

        [Header("Drag Allignment Variables")]
        [SerializeField] float widthUnit;
        [SerializeField] float heightUnit;
        [SerializeField] float offSetX;
        [SerializeField] float offSetY;
        [SerializeField] float offSetZ;

        [Space]
        [SerializeField] EquipmentSlot[] equipmentSlots;

        [SerializeField] Transform rakeUIParent;
        [SerializeField] Transform rakeUIMeleeParent;
        [SerializeField] Transform rakeUIHelmetParent;
        [SerializeField] Animator rakeUIAnim;

        [Header("Rake Variables")]
        [SerializeField] Transform rakeParent;
        [SerializeField] Transform rakeMeleeParent;
        [SerializeField] Transform rakeHelmetParent;
        [SerializeField] Animator rakeAnim;
        [SerializeField] PlayerController player;

        [SerializeField] public EquipmentSlot onTopEquipmentSlot;
        [SerializeField] public EquipmentSlot currentEquipmentSlot;

        InventoryPanel inventoryPanel;

        private void Update()
        {
            Allign();
            DropDragSlot();
        }

        private void Start()
        {
            IdentifyCaches();
        }

        private void IdentifyCaches()
        {
            equipmentSlots = FindObjectsOfType<EquipmentSlot>();
            inventoryPanel = FindObjectOfType<InventoryPanel>();

            rakeUIParent = FindObjectOfType<PrepRakeParent>().transform;
            rakeUIMeleeParent = FindObjectOfType<PrepMeleeParent>().transform;
            rakeUIHelmetParent = FindObjectOfType<PrepHelmetParent>().transform;
            rakeUIAnim = rakeUIParent.GetComponent<Animator>();

            rakeParent = FindObjectOfType<RakeParent>().transform;
            rakeAnim = rakeParent.GetComponent<Animator>();
            rakeHelmetParent = FindObjectOfType<HelmetParent>().transform;
            rakeMeleeParent = FindObjectOfType<MeleeParent>().transform;
            player = FindObjectOfType<PlayerController>();
        }

        public Vector3 AllignWithMouse()
        {
            float mousePosFixX = Input.mousePosition.x / (Screen.width * widthUnit);
            float mousePosFixY = Input.mousePosition.y / (Screen.height * heightUnit);

            Vector3 mousePos = new Vector3(mousePosFixX + offSetX, mousePosFixY + offSetY, offSetZ);
            return mousePos;
        }

        private void Allign()
        {
            if (isDragging)
            {
                transform.position = AllignWithMouse();
            }
        }

        public void EquipItem()
        {

            if (currentDragItem != null)
            {
                //Amulet Slot
                if (currentDragItem.category == ItemCategory.amulet)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentDragItem.itemID && o.itemCategory == currentDragItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentDragItem.itemID, itemCategory = currentDragItem.category, itemCount = 1 });

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
                            amuletEquipmentSlot.equippedItem = currentDragItem;
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

                            amuletEquipmentSlot.equippedItem = currentDragItem;
                            amuletEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Helmet Slot
                if (currentDragItem.category == ItemCategory.helmet)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentDragItem.itemID && o.itemCategory == currentDragItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentDragItem.itemID, itemCategory = currentDragItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        //Add 3D Object
                        var foundHelmet3DObject = Instantiate(currentDragItem.threeDObject, rakeUIHelmetParent);
                        var foundHelmet3DO = Instantiate(currentDragItem.threeDObject, rakeHelmetParent);

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
                            helmetEquipmentSlot.equippedItem = currentDragItem;
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

                            helmetEquipmentSlot.equippedItem = currentDragItem;
                            helmetEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Chest Slot
                if (currentDragItem.category == ItemCategory.chest)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentDragItem.itemID && o.itemCategory == currentDragItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentDragItem.itemID, itemCategory = currentDragItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        // ////Add 3D Object
                        //UIRake
                        var foundChest3DObject = Instantiate(currentDragItem.threeDObject, rakeUIParent);
                        foundChest3DObject.name = "Chest";
                        //Rake
                        var foundChest3DO = Instantiate(currentDragItem.threeDObject, rakeParent);
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

                        if (chestEquipmentSlot.equippedItem == null)
                        {
                            chestEquipmentSlot.equippedItem = currentDragItem;
                            chestEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                        else
                        {
                            var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault
                                (o => o.itemID == chestEquipmentSlot.equippedItem.itemID && o.itemCategory == chestEquipmentSlot.equippedItem.category);

                            if (foundEquippedItemInInventory != null) //if there is already a item of same kind in the inventory
                            {
                                foundEquippedItemInInventory.itemCount += 1;
                            }
                            else
                            {
                                SaveData.Instance().inventoryItems.Add(new InventorySaveData
                                { itemID = chestEquipmentSlot.equippedItem.itemID, itemCategory = chestEquipmentSlot.equippedItem.category, itemCount = 1 });
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

                            chestEquipmentSlot.equippedItem = currentDragItem;
                            chestEquipmentSlot.DisplayEquippedItem();
                            return;
                        }

                    }
                }

                //Pants Slot
                if (currentDragItem.category == ItemCategory.pants)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentDragItem.itemID && o.itemCategory == currentDragItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentDragItem.itemID, itemCategory = currentDragItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        // ////Add 3D Object
                        //UIRake
                        var foundPants3DObject = Instantiate(currentDragItem.threeDObject, rakeUIParent);
                        foundPants3DObject.name = "Pants";
                        //Rake
                        var foundPants3DO = Instantiate(currentDragItem.threeDObject, rakeParent);
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
                            pantsEquipmentSlot.equippedItem = currentDragItem;
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

                            pantsEquipmentSlot.equippedItem = currentDragItem;
                            pantsEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Boots Slot
                if (currentDragItem.category == ItemCategory.boots)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentDragItem.itemID && o.itemCategory == currentDragItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentDragItem.itemID, itemCategory = currentDragItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        // ////Add 3D Object
                        //UIRake
                        var foundBoots3DObject = Instantiate(currentDragItem.threeDObject, rakeUIParent);
                        foundBoots3DObject.name = "Boots";
                        //Rake
                        var foundBoots3DO = Instantiate(currentDragItem.threeDObject, rakeParent);
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
                            bootsEquipmentSlot.equippedItem = currentDragItem;
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

                            bootsEquipmentSlot.equippedItem = currentDragItem;
                            bootsEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }
                }

                //Melee Slot
                if (currentDragItem.category == ItemCategory.melee)
                {
                    var foundInventoryItem = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == currentDragItem.itemID && o.itemCategory == currentDragItem.category);
                    SaveData.Instance().equippedItems.Add(new InventorySaveData { itemID = currentDragItem.itemID, itemCategory = currentDragItem.category, itemCount = 1 });

                    if (foundInventoryItem != null)
                    {
                        //Add 3D Object
                        var foundMelee3DObject = Instantiate(currentDragItem.threeDObject, rakeUIMeleeParent);
                        var foundMelee3DO = Instantiate(currentDragItem.threeDObject, rakeMeleeParent);

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
                            meleeEquipmentSlot.equippedItem = currentDragItem;
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

                            meleeEquipmentSlot.equippedItem = currentDragItem;
                            meleeEquipmentSlot.DisplayEquippedItem();
                            return;
                        }
                    }

                }

                //Ranged Slot
                /*
                if (currentDragItem.category == ItemCategory.ranged)
                {

                }*/

            }

        }

        private void DropDragSlot()
        {
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                if (onTopEquipmentSlot != null && currentDragItem.category == onTopEquipmentSlot.itemCategory)
                {
                    //Debug.Log("Equipped");
                    EquipItem();
                    inventoryPanel.DisplayItems();
                }
                else if (isOnTopItemSlot && currentEquipmentSlot != null)
                {
                    if(currentDragItem == currentEquipmentSlot.equippedItem)
                    {
                        currentEquipmentSlot.UnEquipItem();
                    }
                }
            }
            else
                return;
        }

        private void Clear3DObject(string type)
        {

            var threeDObjects = rakeUIParent.GetComponentsInChildren<ThreeDObject>();

            var foundThree3DObject = threeDObjects.FirstOrDefault(o => o.name == type);
            Debug.Log(foundThree3DObject);
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
            player.currentPower += currentDragItem.powerBonus;
            player.currentStamina += currentDragItem.staminaBonus;
            player.currentAgility += currentDragItem.agilityBonus;

            player.ShowStats();
        }
    }
}

