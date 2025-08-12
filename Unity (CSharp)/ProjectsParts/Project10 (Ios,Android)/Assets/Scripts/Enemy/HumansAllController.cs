using UnityEngine;
using System.Collections;

// Контроллер по управлению генерацию пакетипами
public class HumansAllController : MonoBehaviour {

    public GameObject gameController; // Основной контроллер
    public GameObject human; // Стандартный человек
    public GameObject[] humanSpecial; // Специальный человек

    private RunnerController runner;
    private float timeLastGenerate; // Время последней генерации
    public float timeGenerate;  //Время между генерациями

    private bool firstGenerate;

    private float generateGroup; //Время для генерации
    private bool needGenerate;  // Ожидаем новую генерацию

    void Start()
    {
        runner = gameController.GetComponent<RunnerController>();
        firstGenerate = true;
    }

    void Update()
    {
        if (runner.runSpeedActual > 0 && firstGenerate)
        {
            needGenerate = true;
            firstGenerate = false;
            generateGroup = Time.time + 3f;
        }

        if (!needGenerate && Time.time > timeGenerate + timeLastGenerate)
        {
            needGenerate = true;
            firstGenerate = false;
            generateGroup = Time.time + 10f;
        }

        if (needGenerate)
        {
            //runner.EnemyIncoming(generateGroup - Time.time);
            if (generateGroup - Time.time <= 0)
            {
                needGenerate = false;
                GenerateGroup();
            }
        }
    }


    void GenerateGroup()
    {
        timeLastGenerate = Time.time;
        Instantiate(human, new Vector2(7.5f, runner.mapHeight + 4f), Quaternion.identity);
        Instantiate(human, new Vector2(7.5f, runner.mapHeight + 4f), Quaternion.identity);
        Instantiate(human, new Vector2(7.5f, runner.mapHeight + 4f), Quaternion.identity);
        Instantiate(human, new Vector2(7.5f, runner.mapHeight + 4f), Quaternion.identity);
        int rand = (int)Mathf.Round( Random.Range(0, humanSpecial.Length));
        Instantiate(humanSpecial[rand], new Vector2(7.5f, runner.mapHeight + 4f), Quaternion.identity);
        rand = (int)Mathf.Round(Random.Range(0, humanSpecial.Length));
        Instantiate(humanSpecial[rand], new Vector2(7.5f, runner.mapHeight + 4f), Quaternion.identity);

    }

}
