﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Dio.TriviaGame.Global;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Dio.TriviaGame.Pack
{
    public class PackData : MonoBehaviour
    {
        [SerializeField] private TMP_Text amountCoin;
        [SerializeField] private Button _packButtonPrefab;
        [SerializeField] private Transform _packParent;
        private List<int> price = new List<int> {0,100,100,100};
        PackType[] packs;

        private void Awake()
        {
            GetPackList();
            InitPackList();
            UpdateCoinText();
        }
        private void OnEnable()
        {
            EventManager.StartListening("SetCointext", UpdateCoinText);
        }
        private void OnDisable()
        {
            EventManager.StopListening("SetCointext", UpdateCoinText);
        }

        void GetPackList()
        {
            packs = (PackType[])Enum.GetValues(typeof(PackType));
        }
        void InitPackList()
        {
            for (int i = 0; i < packs.Length; i++)
            {
                Button packButton = Instantiate(_packButtonPrefab, _packParent);

                PackType packType = packs[i];
                packButton.name = "Level Pack " + packType.ToString();
                packButton.GetComponentInChildren<TMP_Text>().text = "Level Pack " + packType.ToString();
                packButton.GetComponent<PackObject>().pricePack = price[i];

                packButton.onClick.RemoveAllListeners();
                packButton.onClick.AddListener(() => OnClickPack(packType, packButton));
            }
        }
        void OnClickPack(PackType pack,Button button)
        {
            PackDatabase.databaseInstance.packType = pack;
            PackDatabase.databaseInstance.packName = pack.ToString();
            PackDatabase.databaseInstance.GetPackList();

            SceneManager.LoadScene("Level");
        }
        private void UpdateCoinText()
        {
            amountCoin.text = Currency.currencyInstance.amountCoin.ToString();
        }
    }
    public enum PackType
    {
        A,
        B,
        C,
        D
    }
}