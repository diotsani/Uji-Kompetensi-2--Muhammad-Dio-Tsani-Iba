﻿using Dio.TriviaGame.Database;
using Dio.TriviaGame.Global;
using Dio.TriviaGame.Message;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dio.TriviaGame.Gameplay
{
    public class Quiz : MonoBehaviour
    {
        SaveData saveData = SaveData.saveDataInstance;
        PackDatabase packDatabase = PackDatabase.databaseInstance;
        private QuizData quizData;
        [SerializeField] private QuizScriptable quizScriptable;
        [SerializeField] private List<QuizData> quizDataList;
        [SerializeField] private string _quizID;
        [SerializeField] private string selectedNameLevel;
        [SerializeField] private int defaultIndexLevel;
        [SerializeField] private int selectedIndexLevel;
        public string getNameLevelID;

        [SerializeField] private TMP_Text questionText;
        [SerializeField] private Image hintImage;
        [SerializeField] private string correctAnswer;
        [SerializeField] private string checkAnswer;
        [SerializeField] private int coinLevel;

        [SerializeField] private List<Button> answerButtonList;

        [SerializeField] private Button answerPrefab;
        [SerializeField] private Transform answerParent;
        private int amount;

        private void OnEnable()
        {
            EventManager.StartListening("SetDataMessage", SetQuizData);
            EventManager.StartListening("StartGameMessage", NewQuiz);
        }
        private void OnDisable()
        {
            EventManager.StopListening("SetDataMessage", SetQuizData);
            EventManager.StopListening("StartGameMessage", NewQuiz);
        }
        private void Awake()
        {
            answerButtonList = new List<Button>();
            quizScriptable = PackDatabase.databaseInstance.levelPackSelected;
            selectedIndexLevel = PackDatabase.databaseInstance.levelIndex;
            selectedNameLevel = PackDatabase.databaseInstance.packName;
            _quizID = quizScriptable.quizDataID;
            SetQuizData();
        }
        void SetQuizData()
        {
            quizDataList = quizScriptable.quizData;
            quizData = quizDataList[selectedIndexLevel];
            
            getNameLevelID = selectedNameLevel + selectedIndexLevel;
            quizData.QuizLevelID = getNameLevelID;
        }
        void NewQuiz()
        {
            InitQuiz(quizData);
        }
        void InitQuiz(QuizData quiz)
        {
            questionText.text = quiz.question;
            correctAnswer = quiz.correctAnswer;
            hintImage.sprite = quiz.hintImage;
            coinLevel = quiz.coin;

            List<string> answerName = quiz.answerList;
            for (int i = 0; i < quiz.answerList.Count; i++)
            {
                if (answerButtonList.Count < quiz.answerList.Count)
                {
                    Button answerButton = Instantiate(answerPrefab, answerParent);
                    answerButtonList.Add(answerButton);
                    answerButton.name = answerName[i];
                    answerButton.GetComponent<AnswerObject>()._answerText.text = answerName[i];
                    answerButton.GetComponent<AnswerObject>().answerToCheck = answerName[i];

                    answerButton.onClick.RemoveAllListeners();
                    answerButton.onClick.AddListener(() => OnClickAnswer(answerButton));
                }
                else
                {
                    for (int j = 0; j < answerButtonList.Count; j++)
                    {
                        Button newButton = answerButtonList[j];
                        newButton.name = answerName[j];
                        newButton.GetComponent<AnswerObject>()._answerText.text = answerName[j];
                        newButton.GetComponent<AnswerObject>().answerToCheck = answerName[j];

                        newButton.onClick.RemoveAllListeners();
                        newButton.onClick.AddListener(() => OnClickAnswer(newButton));
                    }
                    
                }
            }
            selectedIndexLevel++;
            defaultIndexLevel = selectedIndexLevel - 1;
        }
        void OnClickAnswer(Button button)
        {
            var b = button.GetComponent<AnswerObject>();
            checkAnswer = b.answerToCheck;

            OnCheckAnswer();
        }
        void OnCheckAnswer()
        {
            if (correctAnswer == checkAnswer)
            {
                saveData.AddLevelIdData(getNameLevelID,coinLevel);
                EventManager.TriggerEvent("PlayerWinMessage", new PlayerWinMessage(selectedNameLevel,selectedIndexLevel));
                EventManager.TriggerEvent("StopCountdownMessage");
                CheckNextLevel();
            }
            else
                EventManager.TriggerEvent("GoToLevelMessage");
        }
        void CheckNextLevel()
        {
            if (selectedIndexLevel < quizDataList.Count)
            {
                EventManager.TriggerEvent("NextLevelMessage");
            }
            else
            {
                EventManager.TriggerEvent("GoToPackMessage");
            }
            GetQuizData();
        }
        void GetQuizData()
        {
            saveData.AddQuizIdData(_quizID);
            saveData.AddPackIdData(selectedNameLevel);

        }
    }
}