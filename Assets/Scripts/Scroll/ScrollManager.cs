using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    public List<Sprite> BallSprites;
    public GameObject ballPrefab; // Ԥ���壬����������
    public BoxCollider2D spawnArea; // BoxCollider2D����

    public float angleRange = 30f;
    public float minSpeed = 1f; // ��С�ٶ�
    public float maxSpeed = 5f; // ����ٶ�
    public float minAngularSpeed = -360f; // ��С���ٶ�
    public float maxAngularSpeed = 360f; // �����ٶ�

    
    public int catcount = 1;
    public float splitTime = 0.4f;
    public Text scoreBoard;
    public int score;
    private int ballIndex;
    public void setBallIndex(Food food){
        switch(food.foodType){
            case FoodType.Type1:
            ballIndex = 0;
            break;
            case FoodType.Type2:
            ballIndex = 1;
            break;
            case FoodType.Type3:
            ballIndex = 2;
            break;
            case FoodType.Type4:
            ballIndex = 3;
            break;
            case FoodType.Type5:
            ballIndex = 4;
            break;
            case FoodType.Type6:
            ballIndex = 5;
            break;
            case FoodType.Type7:
            ballIndex = 6;
            break;
            case FoodType.Type8:
            ballIndex = 7;
            break;
            case FoodType.Type9:
            ballIndex = 8;
            break;
            default:
            break;
        }
    }
    public IEnumerator CatBottom()
    {
        //Animator anim = GetComponent<Animator>();
        //anim.Play("bottle2");
        
     
        for (int i = 0; i < catcount; i++)
        {
            SpawnBall(ballIndex);
            score += 1;
            scoreBoard.text = score.ToString();
            PlayerPrefs.SetString("score", scoreBoard.text);
            yield return new WaitForSeconds(splitTime);
        }

        yield return new WaitForSeconds(2f);
        //GetComponent<Animator>().Play("bottle");
    }
    
    void SpawnBall(int _index)
    {
        // ��ȡBoxCollider2D�ı߽�
        Bounds bounds = spawnArea.bounds;

        // ��BoxCollider2D��Χ���������һ��λ��
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        Vector2 randomPosition = new Vector2(randomX, randomY);

        // ������
        GameObject ball = Instantiate(ballPrefab, randomPosition, Quaternion.identity);
        ball.GetComponentInChildren<SpriteRenderer>().sprite = BallSprites[_index];
        // ��ȡ���Rigidbody2D���
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

        // ����һ�����µ�����ٶ�
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        float randomAngle = Random.Range(-angleRange, angleRange);
        float angleInRadians = randomAngle * Mathf.Deg2Rad;

        // �����ٶȵ�x��y����
        Vector2 velocity = new Vector2(Mathf.Sin(angleInRadians), -Mathf.Cos(angleInRadians)) * randomSpeed;
        rb.velocity = velocity;
        
        // ����һ�������ת���ٶ�
        float randomAngularSpeed = Random.Range(minAngularSpeed, maxAngularSpeed);
        rb.angularVelocity = randomAngularSpeed;
    }
}
