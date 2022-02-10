using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CompassBehaviour : MonoBehaviour
{
    public Text text;
    private bool startTracking = false;

    void Start() {
        Input.compass.enabled = true;
        Input.location.Start();
        StartCoroutine(Compass());
    }

    void Update() {
        if (startTracking) {
            // Rotamos la brújula
            transform.rotation = Quaternion.Euler(0, 0, -Input.compass.trueHeading);
            // Mostramos los grados y el cardinal
            text.text = ((int)Input.compass.trueHeading).ToString() + "º " +  DegreesToCardinalDetailed(Input.compass.trueHeading);
        }
    }

    IEnumerator Compass() {
        yield return new WaitForSeconds(1f);
        startTracking = Input.compass.enabled;
    }

    private string DegreesToCardinalDetailed(double degrees) {
        string[] cardinals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        return cardinals[(int)Math.Round(((double)degrees * 10 % 3600) / 225)];
    }
}
