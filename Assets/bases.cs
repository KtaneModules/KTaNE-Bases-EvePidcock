using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KMHelper;
using UnityEngine;
using Random = UnityEngine.Random;

public class bases : MonoBehaviour {
    private static int _moduleIdCounter = 1;
    private bool _isSolved, _lightsOn;
    private int _moduleId;
    public KMAudio newAudio;
    public KMBombInfo info;
    public KMBombModule module;

    public KMSelectable[] numbers;
    public KMSelectable submit;

    public TextMesh num1Text, num2Text, answerText, opText;
    private int num1, num2, answer, base1, base2, baseAns, op;
    private String currentInput;

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        module.OnActivate += Activate;
    }

    private void Awake() {
        for (int i = 0; i < 10; i++)
        {
            var j = i;
            numbers[i].OnInteract += delegate {
                handleKeyPress(j);
                return false;
            };
        }
        submit.OnInteract += delegate {
            handleSubmit();
            return false;
        };
    }

    private void handleKeyPress(int key) {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, numbers[key].transform);
        if (!_lightsOn || _isSolved) return;

        currentInput += key.ToString();

        UpdateDisplay();
    }

    private void handleSubmit() {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submit.transform);
        if (!_lightsOn || _isSolved) return;

        if (currentInput == IntToString(answer, baseAns)) {
            module.HandlePass();
            _isSolved = true;
            newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, submit.transform);
            StartCoroutine(EndAnimation());
        } else {
            module.HandleStrike();
            currentInput = "";
            UpdateDisplay();
            
        }

    }

    private void Activate() {
        Init();
        _lightsOn = true;
    }

    private void Init() {
        answerText.text = "";
        setupNumBase();
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        answerText.text = currentInput;
    }

    private void setupNumBase() {
    TryAgain:
        num1 = Random.Range(1, 100);
        num2 = Random.Range(1, 100);
        op = Random.Range(1, 5);
        if (op == 4 && num1 % num2 != 0) {
            goto TryAgain;
        }
        var operators = new Dictionary<int, string> {
            {1, "+"},
            {2, "-"},
            {3, "x"},
            {4, "/"}
        };
        opText.text = operators[op];
        var one = info.GetOnIndicators().Count() + info.GetSerialNumberNumbers().Last() + (info.IsPortPresent(KMBombInfoExtensions.KnownPortType.DVI) ? 3 : 0);
        var two = info.GetOffIndicators().Count() + info.GetSerialNumberNumbers().First() + (info.IsPortPresent(KMBombInfoExtensions.KnownPortType.Serial) ? 6 : 0);
        var ansB = info.GetPortPlateCount() + (info.IsIndicatorPresent(KMBombInfoExtensions.KnownIndicatorLabel.IND) ? 2 : 0);

        if (one != 10) {
            one = one % 7;
            one += 2;
        }
        if (two != 10)
        {
            two = two % 7;
            two += 2;
        }
        if (ansB != 10)
        {
            ansB = ansB % 7;
            ansB += 2;
        }

        base1 = one;
        base2 = two;
        baseAns = ansB;

        switch (op) {
            case 1:
                answer = num1 + num2;
                break;
            case 2:
                answer = num1 - num2;
                break;
            case 3:
                answer = num1 * num2;
                break;
            case 4:
                answer = num1 / num2;
                break;
            default:
                break;
        }

        if (answer < 0) {
            answer *= -1;
        }

        Debug.LogFormat("[Bases #{0}] The first number displayed is {1} in base {2}, which is shown as {3}.", _moduleId, num1, base1, IntToString(num1, base1));
        Debug.LogFormat("[Bases #{0}] The second number displayed is {1} in base {2}, which is shown as {3}.", _moduleId, num2, base2, IntToString(num2, base2));
        Debug.LogFormat("[Bases #{0}] The operator between the numbers is {1}.", _moduleId, operators[op]);
        Debug.LogFormat("[Bases #{0}] The answer is {1} in base {2}, which should be inputted as {3}.", _moduleId, answer, baseAns, IntToString(answer, baseAns));

        num1Text.text = IntToString(num1, base1);
        num2Text.text = IntToString(num2, base2);
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

    private IEnumerator EndAnimation() {
        var one = true;
        var two = true;
        var ans = true;
        if (base1 == 10) {
            one = false;
        }
        if (base2 == 10)
        {
            two = false;
        }
        if (baseAns == 10)
        {
            ans = false;
        }

        var ansMap = currentInput.ToCharArray();
        var ansMap10 = answer.ToString().ToCharArray();

        var num1Map = num1Text.text.ToCharArray();
        var num1Map10 = num1.ToString().ToCharArray();

        var num2Map = num2Text.text.ToCharArray();
        var num2Map10 = num2.ToString().ToCharArray();

        if (ans) {
            for (int j = currentInput.Length - 1; j >= 0; j--) {
                if (j < ansMap10.Length) {
                    ansMap[j] = ansMap10[j];
                } else {
                    ansMap[j] = ' ';
                }

                currentInput = new string(ansMap);
                UpdateDisplay();
                yield return new WaitForSeconds(0.2f);
            }

            currentInput = answer.ToString();
            UpdateDisplay();
        }

        if (two)
        {
            for (int j = num2Map.Length - 1; j >= 0; j--)
            {
                if (j < num2Map10.Length)
                {
                    num2Map[j] = num2Map10[j];
                }
                else
                {
                    num2Map[j] = ' ';
                }

                string num2temp = new string(num2Map);
                num2Text.text = num2temp;
                yield return new WaitForSeconds(0.2f);
            }

            num2Text.text = num2.ToString();
        }

        if (one) {
            for (int j = num1Map.Length - 1; j >= 0; j--) {
                if (j < num1Map10.Length) {
                    num1Map[j] = num1Map10[j];
                } else {
                    num1Map[j] = ' ';
                }

                string num1temp = new string(num1Map);
                num1Text.text = num1temp;
                yield return new WaitForSeconds(0.2f);
            }

            num1Text.text = num1.ToString();
        }

        

    }

#pragma warning disable 414
    private string TwitchHelpMessage = "Use '!{0} 482' to submit 482 as your answer.";
#pragma warning restore 414

    protected KMSelectable[] ProcessTwitchCommand(string input)
    {
        var buttons = new Dictionary<char, KMSelectable> {
            {'0', numbers[0]},
            {'1', numbers[1]},
            {'2', numbers[2]},
            {'3', numbers[3]},
            {'4', numbers[4]},
            {'5', numbers[5]},
            {'6', numbers[6]},
            {'7', numbers[7]},
            {'8', numbers[8]},
            {'9', numbers[9]}
        };

        var split = input.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 1 && Regex.IsMatch(split[0], @"^[0-9]+$"))
        {
            return split[0].Select(c => buttons[c]).Concat(new[] { submit }).ToArray();
        }

        return null;
    }
}
