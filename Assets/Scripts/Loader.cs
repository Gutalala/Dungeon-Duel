using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;            //GameManager prefab to instantiate.

    void Awake()
    {
        //Check if a GameManager instance exists, if not then instantiate it 
        if (GameManager.instance == null)

            //Instantiate gameManager prefab
            Instantiate(gameManager);
    }
}
