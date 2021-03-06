﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Paddle : MonoBehaviour {
    // Combo Stuff
    public float comboTimer = 0.0f;
    public float minComboTimeToExtend = 0.5f;
    public float maxComboTimeToExtend = 1.0f;
    public int baseNumBallsCanSpawn = 1;
    public int combo = 0;
    public int maxCombo = 15;

    // Ball Handling
    public int numBallsCanSpawn;
    public List<GameObject> spawnedBalls;
    public GameObject haggleBallPrefab;

    // how far the paddle can move relative from its top / bottom
    private float extents = 3.50f;

    // Powerups
    public PowerUpItem powerup;

    // Use this for initialization
    void Start () {
        spawnedBalls = new List<GameObject>();
        numBallsCanSpawn = baseNumBallsCanSpawn;
	}

    void UpdatePositionMouse() {
        var rigid = GetComponent<Rigidbody2D>();

        var newPos = new Vector2(rigid.position.x, God.Scale(Input.mousePosition.y, 0, Screen.height, -extents, extents));

        rigid.MovePosition(newPos);
    }

    public void Reset() {
        CancelCombo();

        foreach (GameObject ball in spawnedBalls) {
            Destroy(ball);
        }

        numBallsCanSpawn = baseNumBallsCanSpawn;
    }

    public void BallGone(GameObject ball) {
        spawnedBalls.Remove(ball);
    }

    public void SpawnBall() {
        if( numBallsCanSpawn > 0 ) {
            var pos = transform.position;
            pos.x += 1.0f;
            var ball = (GameObject)Instantiate(haggleBallPrefab, pos, Quaternion.identity);
            ball.GetComponent<HaggleBall>().whoMadeMe = gameObject;
            spawnedBalls.Add(ball);
        }
    }

    // Combo Stuff
    public void CancelCombo() {
        comboTimer = 0.0f;
        combo = 0;
    }

    public void BumpCombo() {
        combo++;
        comboTimer = God.Scale(Mathf.Min(combo, maxCombo), 0, maxCombo, maxComboTimeToExtend, minComboTimeToExtend);
    }

    public float GetComboRatio(bool limit = true) {
        var comboRatio = (float)combo / (float)maxCombo;
        if (limit) {
            comboRatio = Mathf.Min(comboRatio, 1.0f);
        }
        return comboRatio;
    }

    public void DoAbility() {
        if( God.haggleLogic.theRoundState == RoundStates.WaitForPlayerStarted ) {
            SpawnBall();
            God.haggleLogic.OnWaitForPlayerFinish();
        } else if( powerup ) {
            powerup.DoAction();
        }
    }

    public void UnDoAbility() {
        if (powerup) {
            powerup.UnDoAction();
        }
    }

    // Update is called once per frame
    void Update () {
        UpdatePositionMouse();

        if (Input.GetMouseButtonDown(0)) {
            DoAbility();
        } else if (Input.GetMouseButtonUp(0)) {
            UnDoAbility();
        }

        if ( God.haggleLogic.IsRoundActive()) {
            // Combo logic.
            if (comboTimer > 0) {
                comboTimer -= Time.deltaTime;
            } else {
                CancelCombo();
            }
        }

        // Update status.
        //var s = string.Format("Combo: {0:D2}/{1:D2} {2:F3}; Balls: {3:D2}/{4:D2};", 
        //    combo, maxCombo, comboTimer, numBallsCanSpawn, spawnedBalls.Count
        //);
        //God.haggleLogic.statusText.text = s;
    }

    void OnTriggerEnter2D(Collider2D coll) {
        var g = coll.gameObject;
        if ( g.tag == "Powerup" ) {
            var p = g.GetComponent<PowerUpItem>();
            if( p ) {
                if (powerup) {
                    powerup.Suicide();
                }
                p.Attach(gameObject);
            }
        }
    }
}
