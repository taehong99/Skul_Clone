using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class PauseUI : PopUpUI
{
    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;
    List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    private int selectedIndex;

    private void Start()
    {
        texts.Add(GetUI<TextMeshProUGUI>("Return"));
        texts.Add(GetUI<TextMeshProUGUI>("Restart"));
        texts.Add(GetUI<TextMeshProUGUI>("Exit"));
        foreach(var text in texts)
        {
            text.color = unselectedColor;
        }
        selectedIndex = 0;
        texts[selectedIndex].color = selectedColor;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedIndex == texts.Count - 1)
                return;

            texts[selectedIndex].color = unselectedColor;
            texts[++selectedIndex].color = selectedColor;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedIndex == 0)
                return;

            texts[selectedIndex].color = unselectedColor;
            texts[--selectedIndex].color = selectedColor;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (selectedIndex)
            {
                case 0:
                    Return();
                    break;
                case 1:
                    Restart();
                    break;
                case 2:
                    Exit();
                    break;
                default:
                    break;
            }
        }
    }

    private void Return()
    {
        Manager.UI.Return();
    }

    private void Restart()
    {
        Manager.Scene.LoadScene("0.TitleScene");
        Manager.UI.Return();
    }

    private void Exit()
    {
        EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
