using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

namespace NOX
{
    public class EquipmentSlot : MonoBehaviour
    {
        [SerializeField] public Image itemImage;
        [SerializeField] public Item equippedItem;
        [SerializeField] Transform rakeUIParent;
        [SerializeField] Transform rakeUIMeleeParent;
        [SerializeField] Transform rakeUIHelmetParent;
        [SerializeField] Animator rakeUIAnim;

        [Header("Rake Variables")]
        [SerializeField] Transform rakeParent;
        [SerializeField] Transform rakeMeleeParent;
        [SerializeField] Transform rakeHelmetParent;
        [SerializeField] Animator rakeAnim;

        RectTransform equipmentSlotTransform;

        ItemToolTip itemToolTip;
        InventoryPanel inventoryPanel;
        GameDataBase gameDataBase;
        DragSlot dragSlot;
        PlayerController player;

        [Header("Item Slot Type")]
        [SerializeField] public ItemCategory itemCategory;

        void Start()
        {
            IdentifyCaches();
            InitialEquipmentUnload();
            InitialEquipment();
            DisplayEquippedItem();
        }

        private void OnMouseOver()
        {
            DisplayToolTip();

            if (Input.GetMouseButtonDown(1))
            {
                UnEquipItem();
            }

            if (Input.GetMouseButtonDown(0) && equippedItem)
            {
                IntiateDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(ResetAnimRebind());
            }
        }

        private void OnMouseEnter()
        {
            dragSlot.onTopEquipmentSlot = GetComponent<EquipmentSlot>();
        }

        private void OnMouseExit()
        {
            ResetToolTip();
            dragSlot.onTopEquipmentSlot = null;
        }


        public void DisplayEquippedItem()
        {
            if (equippedItem)
            {
                itemImage.gameObject.SetActive(true);

                itemImage.sprite = equippedItem.itemSprite;
            }
            else
            {
                itemImage.gameObject.SetActive(false);
            }

        }

        private void IdentifyCaches()
        {
            itemToolTip = FindObjectOfType<ItemToolTip>();
            gameDataBase = FindObjectOfType<GameDataBase>();
            inventoryPanel = FindObjectOfType<InventoryPanel>();
            equipmentSlotTransform = GetComponent<RectTransform>();
            dragSlot = FindObjectOfType<DragSlot>();

            rakeUIParent = FindObjectOfType<PrepRakeParent>().transform;
            rakeUIHelmetParent = FindObjectOfType<PrepHelmetParent>().transform;
            rakeUIMeleeParent = FindObjectOfType<PrepMeleeParent>().transform;
            rakeUIAnim = rakeUIParent.GetComponent<Animator>();

            rakeParent = FindObjectOfType<RakeParent>().transform;
            rakeAnim = rakeParent.GetComponent<Animator>();
            rakeHelmetParent = FindObjectOfType<HelmetParent>().transform;
            rakeMeleeParent = FindObjectOfType<MeleeParent>().transform;

            player = FindObjectOfType<PlayerController>();
        }

        private void DisplayToolTip()
        {
            if (equippedItem)
            {

                #region Reposition Tooltip
                if (Input.mousePosition.y > 510)
                {
                    itemToolTip.transform.position = new Vector3(transform.position.x + 1.58f, transform.position.y - 0.02f, transform.position.z);
                }
                else
                {
                    itemToolTip.transform.position = new Vector3(transform.position.x + 1.58f, transform.position.y - 0.07f, transform.position.z);
                }

                #endregion

                #region Display Item Tooltip Texts
                //1: Display Item Name
                itemToolTip.itemNameText.text = equippedItem.itemName;
                itemToolTip.itemNameText.SetAllDirty();

                //2: Display Item Rarity
                if (equippedItem.rarity == ItemRarity.common)
                {
                    itemToolTip.itemRarirtyText.text = "Common";
                    itemToolTip.itemRarirtyText.color = gameDataBase.commonColor;
                    itemToolTip.itemNameText.color = gameDataBase.commonColor;
                }
                if (equippedItem.rarity == ItemRarity.uncommon)
                {
                    itemToolTip.itemRarirtyText.text = "Uncommon";
                    itemToolTip.itemRarirtyText.color = gameDataBase.uncommonColor;
                    itemToolTip.itemNameText.color = gameDataBase.uncommonColor;
                }
                if (equippedItem.rarity == ItemRarity.rare)
                {
                    itemToolTip.itemRarirtyText.text = "Rare";
                    itemToolTip.itemRarirtyText.color = gameDataBase.rareColor;
                    itemToolTip.itemNameText.color = gameDataBase.rareColor;
                }
                if (equippedItem.rarity == ItemRarity.epic)
                {
                    itemToolTip.itemRarirtyText.text = "Epic";
                    itemToolTip.itemRarirtyText.color = gameDataBase.epicColor;
                    itemToolTip.itemNameText.color = gameDataBase.epicColor;
                }
                if (equippedItem.rarity == ItemRarity.legendary)
                {
                    itemToolTip.itemRarirtyText.text = "Legendary";
                    itemToolTip.itemRarirtyText.color = gameDataBase.legendaryColor;
                    itemToolTip.itemNameText.color = gameDataBase.legendaryColor;
                }
                itemToolTip.itemRarirtyText.SetAllDirty();

                //Display Item Category
                if (equippedItem.category == ItemCategory.amulet)
                {
                    itemToolTip.itemTypeText.text = "Amulet";
                }
                if (equippedItem.category == ItemCategory.helmet)
                {
                    itemToolTip.itemTypeText.text = "Helmet";
                }
                if (equippedItem.category == ItemCategory.chest)
                {
                    itemToolTip.itemTypeText.text = "Chest Armor";
                }
                if (equippedItem.category == ItemCategory.pants)
                {
                    itemToolTip.itemTypeText.text = "Leg Armor";
                }
                if (equippedItem.category == ItemCategory.boots)
                {
                    itemToolTip.itemTypeText.text = "Boots";
                }
                if (equippedItem.category == ItemCategory.melee)
                {
                    itemToolTip.itemTypeText.text = "Melee Weapon";
                }
                if (equippedItem.category == ItemCategory.ranged)
                {
                    itemToolTip.itemTypeText.text = "Ranged Weapon";
                }
                if (equippedItem.category == ItemCategory.ammo)
                {
                    itemToolTip.itemTypeText.text = "Ammo";
                }
                if (equippedItem.category == ItemCategory.misc)
                {
                    itemToolTip.itemTypeText.text = "Misc";
                }
                if (equippedItem.category == ItemCategory.quest)
                {
                    itemToolTip.itemTypeText.text = "Quest Item";
                }

                itemToolTip.itemTypeText.SetAllDirty();

                //Display Item Info
                itemToolTip.itemInfoText.text = equippedItem.itemInfo;

                itemToolTip.itemInfoText.SetAllDirty();

                #endregion

            }
        }

        private void ResetToolTip()
        {
            itemToolTip.transform.localPosition = new Vector3(-508, 1500, 0);
        }

        public void UnEquipItem()
        {
            if (equippedItem != null)
            {
                var foundEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault(o => o.itemID == equippedItem.itemID && o.itemCategory == equippedItem.category);
                SaveData.Instance().equippedItems.Remove(foundEquippedItem);

                var foundEquippedItemInInventory = SaveData.Instance().inventoryItems.FirstOrDefault(o => o.itemID == equippedItem.itemID && o.itemCategory == equippedItem.category);
                if (foundEquippedItemInInventory != null)
                {
                    foundEquippedItemInInventory.itemCount += 1;
                }
                else
                {
                    SaveData.Instance().inventoryItems.Add(new InventorySaveData { itemID = equippedItem.itemID, itemCategory = equippedItem.category, itemCount = 1 });
                }

                //Remove 3D Object
                //Chest
                if(foundEquippedItem.itemCategory == ItemCategory.chest)
                {
                    Clear3DObject("Chest");
                    Clear3DO("Chest");
                }
                //Pants
                if (foundEquippedItem.itemCategory == ItemCategory.pants)
                {
                    Clear3DObject("Pants");
                    Clear3DO("Pants");
                }
                //Boots
                if (foundEquippedItem.itemCategory == ItemCategory.boots)
                {
                    Clear3DObject("Boots");
                    Clear3DO("Boots");
                }
                //Melee
                if(foundEquippedItem.itemCategory == ItemCategory.melee)
                {
                    Destroy(rakeUIMeleeParent.GetChild(0).gameObject);
                }
                //Helmet
                if (foundEquippedItem.itemCategory == ItemCategory.helmet)
                {
                    Destroy(rakeUIHelmetParent.GetChild(0).gameObject);
                    Destroy(rakeHelmetParent.GetChild(0).gameObject);
                }

                player.currentPower -= equippedItem.powerBonus;
                player.currentStamina -= equippedItem.staminaBonus;
                player.currentAgility -= equippedItem.agilityBonus;

                player.ShowStats();

                equippedItem = null;

                inventoryPanel.DisplayItems();
                DisplayEquippedItem();
            }
        }


        private void IntiateDrag()
        {
            dragSlot.isDragging = true;
            dragSlot.currentDragItem = equippedItem;
            dragSlot.itemImage.sprite = equippedItem.itemSprite;
            dragSlot.currentEquipmentSlot = GetComponent<EquipmentSlot>();
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

        private void InitialEquipment()
        {
            var currentEquippedItem = SaveData.Instance().equippedItems.FirstOrDefault(o => o.itemCategory == itemCategory);

            if (currentEquippedItem != null)
            {
                var foundItem = gameDataBase.allItems.FirstOrDefault(o => o.category == currentEquippedItem.itemCategory && o.itemID == currentEquippedItem.itemID);

                equippedItem = foundItem;

                //UI Rake
                var foundMelee3DObject = Instantiate(equippedItem.threeDObject, rakeUIMeleeParent);
                //Rake
                var foundMelee3DO = Instantiate(equippedItem.threeDObject, rakeMeleeParent);

            }
            
        }

        private void InitialEquipmentUnload()
        {
            equippedItem = null;
        }

        IEnumerator ResetAnimRebind()
        {
            yield return new WaitForSeconds(0.01f);
            rakeUIAnim.Rebind();
            rakeAnim.Rebind();
        }
    }

}
