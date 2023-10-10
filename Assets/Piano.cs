using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Piano : MonoBehaviour
{
    public GameObject whiteKeys;
    public GameObject blackKeys;
    public Material whiteMaterial;
    public Material blackMaterial;
    public Material rootMaterial;

    public TextMeshPro keyNameText;
    public TextMeshPro pressedKeyNameText;
    public TextMeshPro pressedKeyDoremiText;
    public KeySelector.Handedness preferredHandedness;

    List<List<Transform>> keysByNote = new List<List<Transform>>();
    readonly List<int> blackNoteIndices = new List<int>() { 1, 3, 6, 8, 10 };
    readonly List<string> notes = new List<string>() { "C", "Cs", "D", "Ds", "E", "F", "Fs", "G", "Gs", "A", "As", "B" };
    int p_currentRoot = 0;
    int currentRoot
    {
        get { return p_currentRoot; }
        set
        {
            var newValue = Mod(value, 12);

            p_currentRoot = newValue;
            for (int i = 0; i < 12; i++)
            {
                if (i == newValue)
                {
                    keysByNote[i].ForEach((key) => key.GetComponent<Renderer>().material = rootMaterial);
                }
                else if (blackNoteIndices.Contains(Mod(i - newValue, 12)))
                {
                    keysByNote[i].ForEach((key) => key.GetComponent<Renderer>().material = blackMaterial);
                }
                else
                {
                    keysByNote[i].ForEach((key) => key.GetComponent<Renderer>().material = whiteMaterial);
                }
            }
            if (keyNameText != null) keyNameText.text = ToKeyname(notes[newValue]);
        }
    }

    int Mod(int i, int divider)
    {
        var mod = i % divider;
        if (mod < 0) mod += divider;
        return mod;
    }

    string ToKeyname(string s)
    {
        return s.Replace('s', '#');
    }

    void Awake()
    {
        for (int i = 0; i < 12; i++)
        {
            keysByNote.Add(new List<Transform>());
        }
        foreach (Transform white in whiteKeys.transform)
        {
            registerKeys(white);
        }
        foreach (Transform black in blackKeys.transform)
        {
            registerKeys(black);
        }
    }

    void registerKeys(Transform key)
    {
        for (int i = 0; i < 12; i++)
        {
            if (key.name.StartsWith(notes[i] + "_"))
            {
                keysByNote[i].Add(key);
                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentRoot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            currentRoot++;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentRoot--;
        }
    }

    public void OnSliderChanged(int i, int j)
    {
        currentRoot = 12 - i;
    }
    public void OnIncrement()
    {
        currentRoot += 1;
    }
    public void OnDecrement()
    {
        currentRoot -= 1;
    }

    public void OnKeyColliderEnter(int i, KeySelector.Handedness handedness)
    {
        if (i < 0 || i > 11) return;
        if (handedness != preferredHandedness) return;
        pressedKeyNameText.text = ToKeyname(notes[i]);
    }
}
