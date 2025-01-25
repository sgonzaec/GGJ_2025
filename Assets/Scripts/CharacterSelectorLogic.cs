using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public List<GameObject> characters = new List<GameObject>();
    
    public int currentCharacter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (characters.Count > 0)
        {
            ActiveCharacter(currentCharacter);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveCharacter(int index)
    {
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
        if (index >= 0 && index < characters.Count)
        {
            characters[index].SetActive(true);
            currentCharacter = index;
        }

        PlayerPrefs.SetString("SelectedCharacter", characters[index].name);
    }

    public void SwitchToPreviousCharacter()
    {
        int previousIndex = currentCharacter - 1;
        if (previousIndex < 0)
        {
            previousIndex = characters.Count - 1;
        }
        ActiveCharacter(previousIndex);
    }

    public void SwitchToNextCharacter()
    {
        int nextIndex = (currentCharacter + 1)% characters.Count;
        ActiveCharacter(nextIndex);
    }
}
