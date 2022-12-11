using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyText : MonoBehaviour
{
    private TextMesh textMesh;
    private string text;
    
    QuestionsManagement questionManagement;
    Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        text = textMesh.text;
        //add this object on questions management
        questionManagement = FindObjectOfType<QuestionsManagement>();
        enemy =GetComponentInParent<Enemy>();
        enemy.OnDeath += CheckIdDeadhasCorrectAnswer;
        questionManagement.AddEnemyText(this);
    }

    void CheckIdDeadhasCorrectAnswer()
    {
        if (text == questionManagement.GetCurrentQuestion().correctAnswer) {
            print("This was the enemy with the correct answer");
        }
    }

    public string TextOfEnemy {
        get { 
            return text; 
        }
        set { 
            text = value;
            textMesh.text = text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
