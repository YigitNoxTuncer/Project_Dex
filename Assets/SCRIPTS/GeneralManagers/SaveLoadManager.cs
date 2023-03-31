using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace NOX
{
    public class SaveLoadManager : MonoBehaviour
    {


        void Start()
        {

        }

        public void ResetSave()
        {
            SaveData.ResetSaveData();
            SceneManager.LoadScene(0);
            SaveData.Save();
        }

        public void SaveGame()
        {

            SaveData.Save();
        }

        public void LoadGame()
        {
            SaveData.Load();
            SceneManager.LoadScene(0);
        }
    }

}
