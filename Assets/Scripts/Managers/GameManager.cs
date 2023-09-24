using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List<WordSO> WordList;

    private WordSO currentWord;

    [SerializeField]
    private TextMeshProUGUI statusTextField;

    [SerializeField]
    private TextMeshProUGUI meaningTextField;

    public AudioClip soundField;
    public Image pictureField;

    private List<GameObject> lettersToGuess = new List<GameObject>();

    [SerializeField]
    private Transform buttonParent;

    [SerializeField]
    private Transform displayClueParent;

    [SerializeField]
    private Button letterButtonPrefab;

    [SerializeField]
    private TextMeshProUGUI letterPrefab;

    public AudioSource thisAud;


    private int hiddenLetters = 0;

    public int difficulty; // will check the difficulty
    public string currentWordString; // will check what is the current word



    bool canPlaySound = true;


    private void Start()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty");

        if (SceneManager.GetActiveScene().name.Equals("Game"))
            InitGame();

        thisAud = GetComponent<AudioSource>();
    }
  
    public WordSO GetCurrentWord()
    {
        return currentWord;
    }

    private void ClearCompleted()
    {
        foreach (WordSO word in WordList)
        {
            word.IsComplete = false;
        }
    }

    private void ClearTextFields()
    {
        statusTextField.text = "";
        meaningTextField.text = "";

        foreach (GameObject go in lettersToGuess)
        {
            Destroy(go);
        }

        foreach(Transform t in buttonParent.transform)
        {
            Destroy(t.gameObject);
        }
        lettersToGuess.Clear();
    }

    private void DisplayWord()
    {
        ClearTextFields();
        pictureField.sprite = null;
        pictureField.gameObject.SetActive(false);

        foreach (WordSO word in WordList)
        {
            if (!word.IsComplete)
            {
                Debug.Log("Set current word to: " + word.GetWord);  
                currentWord = word;

                SetClueText(currentWord.GetWord);

                return;
            }
        }

        currentWord = null;
        FinishGame();
    }

    private void SetClueText(string wordRef)
    {
        currentWordString = wordRef;
        int minHiddenLetters = 0;
        if(difficulty == 0)
        {
            minHiddenLetters = Mathf.Max(1, wordRef.Length / 3);
        }
        else if (difficulty == 1)
        {
            minHiddenLetters = Mathf.Max(1, wordRef.Length / 2);
        }
        else if(difficulty == 2)
        {
            minHiddenLetters = wordRef.Length;
        }
        hiddenLetters = 0;

        int index = -1;

        for (int i = 0; i < wordRef.Length; i++)
        {

            index++;
            int randomChance = 0; 
            if(difficulty == 0)
            {
                randomChance = Random.Range(0, 2);
            }
            else if (difficulty == 1)
            {
                randomChance = Random.Range(0, 10);
            }
            else if(difficulty == 2)
            {
                randomChance = Random.Range(0, 100);
            }
            string l = "";
            TextMeshProUGUI go;
            l = wordRef[i].ToString();

            if (randomChance == 0 || hiddenLetters >= minHiddenLetters)
            {
                go = Instantiate(letterPrefab);
                go.transform.parent = displayClueParent;

                go.GetComponent<TextMeshProUGUI>().text = wordRef[i].ToString();
            }
            else
            {
                go = Instantiate(letterPrefab);
                go.transform.parent = displayClueParent;

                go.GetComponent<TextMeshProUGUI>().text = "_";

                InstantiateButton(l, index);

                AddRandomLetter();

                
                hiddenLetters++;
            }

            foreach(Transform t in buttonParent)
            {
                t.SetSiblingIndex(Random.Range(0,3));
            }

            lettersToGuess.Add(go.gameObject);

        }

    }

    private void AddRandomLetter()
    {
        int chance = Random.Range(0, 2);

        char randomLetter = (char)Random.Range(97, 113);

        if (chance == 0)
        {
            Button randomButton = Instantiate(letterButtonPrefab);
            randomButton.transform.parent = buttonParent;

            randomButton.GetComponentInChildren<TextMeshProUGUI>().text = randomLetter.ToString();
            // randomButton.onClick.AddListener(WrongButton);
            randomButton.onClick.AddListener(()=> ClickButton(randomButton));
        }
    }

    private void InstantiateButton(string letter, int index)
    {
        Button go = Instantiate(letterButtonPrefab);
        go.transform.parent = buttonParent;
        go.GetComponentInChildren<TextMeshProUGUI>().text = letter;
        go.GetComponent<ClueContainer>().indexToChange = index;
        go.GetComponent<ClueContainer>().letter = letter;

        // go.onClick.AddListener(() => OnButtonClick(go));
        go.onClick.AddListener(() => ClickButton(go));

    }

    // public void OnButtonClick(Button buttonToClick)
    // {
    //     ClueContainer container = buttonToClick.GetComponent<ClueContainer>();

    //     displayClueParent.GetChild(container.indexToChange).GetComponent<TextMeshProUGUI>().text = container.letter;

    //     hiddenLetters--;

    //     buttonToClick.gameObject.SetActive(false);
    //     Destroy(buttonToClick);
    // }


    public void ClickButton(Button b)
    {
        TextMeshProUGUI t = b.GetComponentInChildren<TextMeshProUGUI>();
        for(int i = 0; i < currentWordString.Length; i++)
        {
            if(currentWordString[i] == t.text[0])
            {
                if(displayClueParent.GetChild(i).GetComponent<TextMeshProUGUI>().text == "_")
                {
                    displayClueParent.GetChild(i).GetComponent<TextMeshProUGUI>().text = t.text;
                    hiddenLetters--;
                    b.gameObject.SetActive(false);
                    WrongButton(false);
                    return;
                }
            }
            if(i == currentWordString.Length-1)
            {
                WrongButton(true);
            }
        }
    }

    public void CheckWord()
    {
        if (hiddenLetters == 0)
        {
            currentWord.IsComplete = true;
            DisplayWord();
        }
    }

    public void WrongButton(bool isTrue)
    {
        statusTextField.text = isTrue ? "Please try again!" : "";
    }

    public void FinishGame()
    {
        SceneManager.LoadScene("EndScreen");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void InitGame()
    {
        ClearCompleted();
        Shuffle(WordList);
        DisplayWord();
    }

    public void Shuffle(List<WordSO> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
    
    public void ShuffleCharacters(List<string> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RevealClue()
    {
        meaningTextField.text = currentWord.GetMeaning;
    }

    public void RevealPicture()
    {
        pictureField.gameObject.SetActive(true);
        pictureField.sprite = currentWord.GetPicture;
    }

    public void RevealSound()
    {
        if(!canPlaySound) return;
        canPlaySound = false;

        soundField = currentWord.GetSound;
        thisAud.PlayOneShot(soundField);

        Invoke("CanPlaySound",1f);
    }

    void CanPlaySound()
    {
        canPlaySound = true;
    }


}
