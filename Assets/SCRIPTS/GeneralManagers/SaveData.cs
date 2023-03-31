using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace NOX
{
    [Serializable]
    public class SaveData
    {
        static SaveData instance;
        static bool isLoaded;
        // /////////////////////////// STORED VARIABLES

        public List<InventorySaveData> inventoryItems;
        public List<InventorySaveData> equippedItems;

        [Header("Player Stats")]
        [SerializeField] public float basePower;
        [SerializeField] public float baseStamina;
        [SerializeField] public float baseAgility;
        
        public float maxStaminaNumber;
        public float currentStaminaNumber;


        // /////////////////////////// STORED VARIABLES


        // /////////////////////////// CLASS CONSTRUCTOR

        public static SaveData Instance()
        {
            if (!isLoaded)
            {
                Load();
                isLoaded = true;
            }
            return instance;
        }

        public static void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.save";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, Instance());
            stream.Close();
        }

        public static void Load()
        {
            string path = Application.persistentDataPath + "/player.save";
            if (File.Exists(path))
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();

                var data = formatter.Deserialize(stream) as SaveData;
                instance = data;
            }
            else
            {
                Debug.Log("Save file was not found in " + path);
                instance = new SaveData();
                instance.InitialLoad();
            }
        }

        public static void ResetSaveData()
        {
            instance.InitialLoad();
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.sav";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, Instance());
            stream.Close();
        }

        private void InitialLoad()
        {
            SetInitials();
        }


        private void SetInitials()
        {
            inventoryItems = InitialInventory();
            equippedItems = InitialEquippedItems();
            maxStaminaNumber = 100;
            currentStaminaNumber = maxStaminaNumber;
        }

        private List<InventorySaveData> InitialInventory()
        {
            var list = new List<InventorySaveData>();

            list.Add(new InventorySaveData { itemID = 1, itemCategory = ItemCategory.melee, itemCount = 1 }); // iron dagger
            list.Add(new InventorySaveData { itemID = 1, itemCategory = ItemCategory.chest, itemCount = 1 }); // leather armor
            list.Add(new InventorySaveData { itemID = 2, itemCategory = ItemCategory.melee, itemCount = 1 }); // silver dagger
            list.Add(new InventorySaveData { itemID = 2, itemCategory = ItemCategory.chest, itemCount = 1 }); // hard leather armor
            list.Add(new InventorySaveData { itemID = 1, itemCategory = ItemCategory.boots, itemCount = 1 }); // leather boots
            list.Add(new InventorySaveData { itemID = 2, itemCategory = ItemCategory.boots, itemCount = 1 }); // hard leather boots
            list.Add(new InventorySaveData { itemID = 1, itemCategory = ItemCategory.pants, itemCount = 1 }); // leather pants
            list.Add(new InventorySaveData { itemID = 2, itemCategory = ItemCategory.pants, itemCount = 1 }); // hard leather pants
            list.Add(new InventorySaveData { itemID = 1, itemCategory = ItemCategory.helmet, itemCount = 1 }); // leather helmet
            list.Add(new InventorySaveData { itemID = 2, itemCategory = ItemCategory.helmet, itemCount = 1 }); // hard leather armor
            list.Add(new InventorySaveData { itemID = 2, itemCategory = ItemCategory.amulet, itemCount = 1 }); // stamina amulet

            return list;
        }

        private List<InventorySaveData> InitialEquippedItems()
        {
            
            var list = new List<InventorySaveData>();

            list.Add(new InventorySaveData { itemID = 1, itemCategory = ItemCategory.melee, itemCount = 1 });

            return list;
        }

    }

    [Serializable]
    public class InventorySaveData
    {
        public int itemID;
        public ItemCategory itemCategory;
        //public ItemRarity itemRarity;
        public int itemCount;
    }

}




