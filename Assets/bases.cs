using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class bases : MonoBehaviour {
    private static int _moduleIdCounter = 1;
    private bool _isSolved, _lightsOn;
    private int _moduleId;
    public KMAudio newAudio;
    public KMBombInfo info;
    public KMBombModule module;

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        module.OnActivate += Activate;
    }

    private void Activate()
    {
        Init();
        _lightsOn = true;
    }

    private void Init() {

    }

    public string IntToString(int a, int radix)
    {
        var chars = "0123456789ABCDEF".ToCharArray();
        var str = new char[32]; // maximum number of chars in any base
        var i = str.Length;
        bool isNegative = (a < 0);
        if (a <= 0) // handles 0 and int.MinValue special cases
        {
            str[--i] = chars[-(a % radix)];
            a = -(a / radix);
        }

        while (a != 0)
        {
            str[--i] = chars[a % radix];
            a /= radix;
        }

        if (isNegative)
        {
            str[--i] = '-';
        }

        return new string(str, i, str.Length - i);
    }
}
