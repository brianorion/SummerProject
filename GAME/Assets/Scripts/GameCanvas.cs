using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public static GameObject canvas;

    private void Awake()
    {
        canvas = gameObject;
    }
}
