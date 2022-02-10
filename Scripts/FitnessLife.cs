using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessLife : MonoBehaviour
{
    // Panels
    public GameObject firstScreen;
    public GameObject secondScreen;
    
    // Text
    public Text distanceWalked;
    public Text steps;
    public Text calories;
    public Text phrase;
    private int it = 0;
    private string[] phrases = {"Today I will do what others won't so tomorrow I can do what others can't", "t's amazing what you can do when you try", "There ar no shortcut to any place worth going", "Excellence is not a singular act but a habit. You are what you do repeatedly. Shaquille O'Neal", "Small daily improvements are the key to staggering long-term results", "Winners forget they're in a rece, they just love to run", "Success isn't given. It's earned. On the track, on the field, in the gym. With blood, sweat, and the occasionar tear.", "There are plenty of difficult obstacles in your path. Don't allow yourself to become one of them."};

    // Distance
    private float distance = 0;
    private float oldDistance = 0;
    private const float earthRadious = 6378;
    private float lastLatitud;
    private float lastLongitud;

    IEnumerator Start() {
        if (Input.location.isEnabledByUser) {
            Input.location.Start();
            int waitTime = 15;
            while(Input.location.status == LocationServiceStatus.Initializing && waitTime > 0) {
                distanceWalked.text = "0";
                steps.text = "0";
                calories.text = "0";
                yield return new WaitForSeconds(1);
                waitTime--;
            }
            if (Input.location.status == LocationServiceStatus.Failed || waitTime == 0) {
                phrase.text = "Error. Status Failed";
                yield break;
            } else {
                lastLatitud = Input.location.lastData.latitude;
                lastLongitud = Input.location.lastData.longitude;
            }
        } else {
            phrase.text = "Error. GPS is not enabled by user";
        }
    }

    public void StartApp() {
        firstScreen.SetActive(false);
        secondScreen.SetActive(true);
    }

    public void ReturnToMenu() {
        firstScreen.SetActive(true);
        secondScreen.SetActive(false);
    }

    public void Restart() {
        distanceWalked.text = "0";
        steps.text = "0";
        calories.text = "0";
        it = 0;
        distance = 0;
        oldDistance = 0;
        phrase.text = "Today I will do what others won't so tomorrow I can do what others can't";
    }

    public void Exit() {
        Input.location.Stop();
        Application.Quit();
    }

    void Update() {
        if (it >= phrases.Length) it = 0;
        if ((distance - oldDistance) > 100) {
            it++;
            phrase.text = phrases[it]; 
        }
        if (Input.location.status == LocationServiceStatus.Running) {
            float deltaDistance = Haversine(lastLatitud, lastLongitud);
            if (deltaDistance > 0f) {
                distance = deltaDistance * 1000;
                distanceWalked.text = distance.ToString();
                int numberOfSteps = (int)(distance * 7000 / 10000);
                steps.text = (numberOfSteps).ToString();
                calories.text = (numberOfSteps * .4f).ToString();
                oldDistance = distance;
            }
        }
    }

    private float Haversine(float lastLatitud, float lastLongitud) {
        float newLatitud = Input.location.lastData.latitude;
        float newLongitud = Input.location.lastData.longitude;
        float deltaLatitud = (newLatitud - lastLatitud) * Mathf.Deg2Rad;
        float deltaLongitud = (newLongitud - lastLongitud) * Mathf.Deg2Rad;
        float a = Mathf.Pow(Mathf.Sin(deltaLatitud / 2), 2) + Mathf.Cos(lastLatitud) * Mathf.Cos(newLatitud) * Mathf.Pow(Mathf.Sin(deltaLongitud / 2), 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        lastLatitud = newLatitud; 
        lastLongitud = newLongitud;
        return distance = earthRadious * c;
    }
}
