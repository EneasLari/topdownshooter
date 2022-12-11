using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class QuestionsManagement : MonoBehaviour
{


    public TextMeshProUGUI questiontext;
    public TextAsset questionsJson;
    public bool setRandomWrongAnswers=true;
    
    List<string> shownWrongAnswers;
    List<EnemyText> enemyTexts;
    List<Question> questions;
    Question currentquestion;

    // Start is called before the first frame update
    void Start()
    {
        enemyTexts = GetComponents<EnemyText>().ToList();
        questions = new List<Question>();
        shownWrongAnswers= new List<string>();

        questions =ParseQuestionsFromJson(questionsJson);
        SetRandomQuestion();
    }

    void SetRandomQuestion() {
        var random = new Random();
        int index=random.Next(questions.Count);
        Question getquestion = questions[index];
        questions.Remove(getquestion);
        currentquestion = getquestion;
        questiontext.text = currentquestion.question;
    }
    List<Question> ParseQuestionsFromJson(TextAsset questionsjson) {

        List<Question> newlist = JsonConvert.DeserializeObject<List<Question>>(questionsjson.text);
        return newlist;
    }

    public void AddEnemyText(EnemyText text) {
        enemyTexts.Add(text);
        if (questions != null && currentquestion!=null) {
            Random random = new Random();
            if (random.Next(100) > 50 && !currentquestion.correctisAlive)
            {
                text.TextOfEnemy = currentquestion.correctAnswer;
                currentquestion.correctisAlive = true;
            }
            else {
                if (setRandomWrongAnswers)
                {
                    string newwrong = (int.Parse(currentquestion.correctAnswer) + random.Next(-int.Parse(currentquestion.correctAnswer), 100)).ToString();
                    while (shownWrongAnswers.Contains(newwrong))
                    {
                        newwrong = (int.Parse(currentquestion.correctAnswer) + random.Next(-int.Parse(currentquestion.correctAnswer), 100)).ToString();
                    }
                    text.TextOfEnemy = newwrong;
                }
                else {
                    //todo
                    print("get non random wrong answers from somewhere");
                }
            }
            shownWrongAnswers.Add(text.TextOfEnemy);
        }
    }

    public Question GetCurrentQuestion() {
        return currentquestion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Serializable]
    public class Question {
        public string question;
        public string correctAnswer;
        public string[] wrongAnswers;
        public bool correctisAlive;
        public Question(string question, string correctanswer) {
            correctisAlive = false;
            this.question = question;
            correctAnswer = correctanswer;
        }

    }

}
