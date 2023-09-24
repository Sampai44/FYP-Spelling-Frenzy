using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Word", menuName = "Word/New Word")]
public class WordSO : ScriptableObject
{
    [SerializeField]
    private string word;

    [SerializeField]
    private string meaning;
    
    [SerializeField]
    private Sprite picture;

    [SerializeField]
    private AudioClip sound;

    [SerializeField]
    private bool isComplete;

    public string GetWord { get { return word; } }
    public string GetMeaning { get { return meaning; } }
    public Sprite GetPicture { get { return picture; } }
    public AudioClip GetSound { get { return sound; } }

    public bool IsComplete
    {
        get { return isComplete; }
        set { isComplete = value; }
    }

}
