using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    //FlashLight
    public bool runFLTimer;
    public float FLTimer;

    public bool runEndTimer;
    public float EndTimer;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (runFLTimer)
            FLTimer -= Time.deltaTime;
        if (runEndTimer)
            EndTimer -= Time.deltaTime;

        if (FLTimer < 0)
            FLEmpty();

        if (EndTimer < 0)
            End();
	}

    void FLEmpty()
    {
        runFLTimer = false;
    }


    void End()
    {
        runEndTimer = false;
        
    }
}
