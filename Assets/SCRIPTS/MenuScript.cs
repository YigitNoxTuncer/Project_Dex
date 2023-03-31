using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace NOX
{
    public class MenuScript : MonoBehaviour
    {
        int scene;
        SaveLoadManager saveLoadManager;

        private void Start()
        {
            scene = SceneManager.GetActiveScene().buildIndex;
            saveLoadManager = FindObjectOfType<SaveLoadManager>();
        }

        public void RetryGame()
        {
            StartCoroutine(RestartAfterASecond());
        }

        public void ExitGame()
        {
            StartCoroutine(ExitAfterASecond());
        }

        IEnumerator ExitAfterASecond()
        {
            yield return new WaitForSeconds(0.5f);
            Application.Quit();
        }

        IEnumerator RestartAfterASecond()
        {
            yield return new WaitForSeconds(0.5f);
            saveLoadManager.ResetSave();
        }
    }
}

