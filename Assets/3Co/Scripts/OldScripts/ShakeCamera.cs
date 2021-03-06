﻿using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour {
    public float baseShakeAmount = 0.1f;
    public float baseShakeTime = 1.0f;

    public float baseShakeIncreaseFactor = 1.05f;
    public float baseShakeDecayFactor = 1.0f;
    public float baseShakeCalmFactor = 1.0f;

    public float maxShakeAmount = 5.0f;
    public float maxShakeTime = 3.0f;

	public float unfuckFactor = 0.001f;

    private float shakeAmount = 1.0f;
    private float shakeTime = 2.0f;

    private Vector3 startPos;

    void Start(){
        startPos = transform.localPosition;
    }
   
    public void Jostle(float amount = 1.0f){
        shakeAmount = Mathf.Min(shakeAmount + amount * baseShakeIncreaseFactor, maxShakeAmount);
        shakeTime = Mathf.Min(shakeTime + amount * baseShakeTime, maxShakeTime);
    }

    public void StopThat(){
        shakeTime = 0.0f;
        shakeAmount = baseShakeAmount;

        transform.localPosition = startPos;
		SetObliqueness (0, 0);
    }

    void Update(){
        if (shakeTime > 0){
            var pos = transform.localPosition;
            pos = Random.insideUnitSphere * shakeAmount;
            pos.z = startPos.z;
            //transform.localPosition = pos;
			SetObliqueness (pos.x * unfuckFactor, pos.y * unfuckFactor);

            if( shakeAmount > baseShakeAmount){
                shakeAmount -= Time.deltaTime * baseShakeCalmFactor;
            }

            shakeTime -= Time.deltaTime * baseShakeDecayFactor;
        } else {
            StopThat();
        }

    }

	void SetObliqueness(float horizObl, float vertObl) {
		Matrix4x4 mat  = Camera.main.projectionMatrix;
		mat[0, 2] = horizObl;
		mat[1, 2] = vertObl;
		Camera.main.projectionMatrix = mat;
	}
}
