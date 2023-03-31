using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOX
{
    public class GameDataBase : MonoBehaviour
    {
        [Header("Items")]
        [SerializeField] public List<Item> allItems;
        [SerializeField] public Color commonColor;
        [SerializeField] public Color uncommonColor;
        [SerializeField] public Color rareColor;
        [SerializeField] public Color epicColor;
        [SerializeField] public Color legendaryColor;

        [Header("Enemy")]
        [SerializeField] public Color detectionGreen;
        [SerializeField] public Color detectionRed;
    }

}

