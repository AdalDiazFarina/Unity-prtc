using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Windows.Speech;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public GameObject enemies;
    public GameObject player;
    public NavMeshAgent agent;


    // Enemy
    public float enemyHealth = 120;
    public float damage = 20;
    public GameObject enemy;

    // SpeechRecognition
    private string[] mKeywords = {"forward", "back", "right", "left", "stop", "Look an enemy", 
    "attack one", "attack two", "attack enemy", "combo one"};
    private KeywordRecognizer mRecognizer;
    private string repeat; 

    // Canvas
    public Text text;

    // Movement
    private float x, z, distanceToWalkPoint;
    public float speed;
    private bool isMov = false;
    private Vector3 walkPoint;

    // Chase enemy
    private Vector3 target;

    void Start() {
        mRecognizer = new KeywordRecognizer(mKeywords);
        mRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        mRecognizer.Start();
        walkPoint = player.transform.position;
        target = player.transform.position;
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args) {
        if (text.text != "repeat") repeat = text.text; 
        text.text = args.text;
        Stop();
        switch(args.text) {
            case "forward":
                isMov = true;
                anim.SetBool("run", true);
                x = 0;
                z = 10;
                break;
            case "back":
                isMov = true;
                anim.SetBool("back", true);
                x = 0;
                z = -10;
                break;
            case "right":
                isMov = true;
                anim.SetBool("right", true);
                x = 10;
                z = 0;
                break;
            case "left":
                isMov = true;
                anim.SetBool("left", true);
                x = -10;
                z = 0;
                break;
            case "stop":
                isMov = false;
                break;
            case "attack one":
                isMov = false;
                anim.SetBool("attackOne", true);
                break;
            case "attack two":
                isMov = false;
                anim.SetBool("attackTwo", true);
                break;
            case "combo one":
                isMov = false;
                anim.SetBool("comboOne", true);
                break;
            case "Look an enemy":
                isMov = false;
                SearchATarget();
                break;
        }
        if (isMov) walkPoint = new Vector3(player.transform.position.x + x, 0, player.transform.position.z + z);
        if (args.text == "forward" || args.text == "right" || args.text == "left") target = walkPoint;
        if (args.text == "attack enemy") {
            isMov = true;
            anim.SetBool("run", true);
            walkPoint = target;
        }
    }

    void Update() {
        if (isMov) Movement();
        if (enemyHealth <= 0) {
            Destroy(enemy);
            enemy = null;
        }
    }

    void Movement() {
        agent.SetDestination(walkPoint);
        player.transform.LookAt(target);
        distanceToWalkPoint = (player.transform.position - walkPoint).magnitude;
        if (distanceToWalkPoint < 2f) {
            target.z = target.z + 1; 
            Stop();
            agent.isStopped = true;
        } else {
            agent.isStopped = false;
        }
    }

    void Stop() {
        anim.SetBool("run", false);
        anim.SetBool("back", false);
        anim.SetBool("right", false);
        anim.SetBool("left", false);
        anim.SetBool("attackOne", false);
        anim.SetBool("attackTwo", false);
        anim.SetBool("comboOne", false);
    }

    void SearchATarget() {
        enemyHealth = 120;
        enemy = enemies.transform.GetChild(0).gameObject;
        target = enemies.transform.GetChild(0).transform.position;
        float minDistance = (player.transform.position - enemies.transform.GetChild(0).transform.position).magnitude;
        float distance;
        for (int i = 1; i < enemies.transform.childCount; i++) {
            distance = (player.transform.position - enemies.transform.GetChild(i).transform.position).magnitude;
            if (minDistance > distance) {
                minDistance = distance;
                target = enemies.transform.GetChild(i).transform.position;
                enemy = enemies.transform.GetChild(i).gameObject;
            }
        }

        target.z = target.z - 1;
        player.transform.LookAt(target);
    }

    public void Attack() {
        Debug.Log((player.transform.position - enemy.transform.position).magnitude);
        if ((player.transform.position - enemy.transform.position).magnitude < 3f) {
            enemy.GetComponent<Renderer>().material.color = Color.red;
            enemyHealth -= damage;
        }
    }

    public void ChangeColor() {
        enemy.GetComponent<Renderer>().material.color = Color.green;
    }
}
