using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Engine : MonoBehaviour
{
    // Change Attributes in Editor
    public AudioClip sound;
    public int textOffset = 10;
    public float letterPause = 0.025f;
    public float messageDelay = 0.25f;

    private Text text;
    private InputField ioField;
    private RectTransform rectTransform;

    private Command cmd;
    private Directory dir;

    private int letterCount;
    private bool inputEnabled;


    private void Start()
    {
        text = GetComponent<Text>();
        ioField = GetComponent<InputField>();
        rectTransform = GetComponent<RectTransform>();

        cmd = GetComponent<Command>();
        dir = GetComponent<Directory>();

        // Assign Default Directory
        cmd.ChangeDirectory(".");

        // Clear Content
        ioField.text = "";

        // Turn Off Input to Begin
        InputOff();

        // Start the Engines
        StartCoroutine(GreetingScript());
    }

    private void Update()
    {
        // If Input is Enabled
        if (inputEnabled)
        {
            // Move Cursor to End of Text
            ioField.MoveTextEnd(true);

            // If Text has Focus, Prevent Clicks
            if (!ioField.isFocused)
                text.raycastTarget = true;
            else
                text.raycastTarget = false;
        }
    }

    private void FixedUpdate()
    {
        // If Input is Enabled
        if (inputEnabled)
        {
            // Find Input Caret
            var theCaret = GameObject.Find("Text Input Caret");

            // Move Caret Up
            if (theCaret != null)
                theCaret.transform.localPosition = new Vector3(theCaret.transform.localPosition.x, rectTransform.transform.localPosition.y, theCaret.transform.localPosition.z);
        }
        else
            // Move Screen Up
            rectTransform.transform.position = new Vector3(rectTransform.position.x, rectTransform.position.y + textOffset, rectTransform.position.z);
    }

    private void OnGUI()
    {
        // If Input is Enabled
        if (inputEnabled)
        {
            // Keys Restricted Until Typed Text Exists
            if (Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete) ||
                Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete) ||
                Input.GetKeyUp(KeyCode.Backspace) || Input.GetKeyUp(KeyCode.Delete))
            {
                if (ioField.text.Length <= letterCount)
                    Event.current.Use();
            }

            // Keys Always Restricted
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Tab) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Tab) ||
                Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.Tab))
            {
                Event.current.Use();
            }

            // Return Key Actions
            if (Input.GetKey(KeyCode.Return) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyUp(KeyCode.Return))
            {
                // If Text has Focus
                if (ioField.isFocused)
                {
                    // User Text Starts at Output End
                    string userInput = ioField.text.Substring(letterCount);

                    // If Input is Not Blank
                    if (userInput != "")
                        // Send Command
                        StartCoroutine(SendCommand(userInput));
                    else
                        Event.current.Use();
                }
            }
        }
    }

    private IEnumerator GreetingScript()
    {
        var msgs = new List<string>();
        msgs.Add(DateTime.Now.ToString("MM/dd/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss") + "\n");
        msgs.Add("Unity Command Line Interface\n");

        // For Each Line of Script
        foreach (string message in msgs)
        {
            // Delay Between Typed Lines
            yield return new WaitForSeconds(messageDelay);

            // Type Line of Script
            yield return TypeOutput(message);
        }

        // Turn on Input
        InputOn();
    }

    private IEnumerator SendCommand(string input)
    {
        // Turn Off Input
        InputOff();

        // Send Command, Get Output
        string output = cmd.Execute(input);

        // Type Output
        yield return TypeOutput("\n");
        yield return TypeOutput(output);

        // Turn on Input
        InputOn();
    }

    private IEnumerator TypeOutput(string output)
    {
        // For Each Line of Script
        foreach (char letter in output.ToCharArray())
        {
            // Add to Existing Text
            ioField.text = ioField.text + letter;

            // Play Sound
            PlaySound();

            // Delay Between Each Letter
            yield return new WaitForSeconds(letterPause);
        }
    }

    private void InputOn()
    {
        // Create Input Prompt
        ioField.text = ioField.text + dir.Prompt + ">";

        // Starting Point = Current Length
        letterCount = ioField.text.Length;

        // Enable Input
        inputEnabled = true;
        ioField.interactable = true;
        ioField.ActivateInputField();
    }

    private void InputOff()
    {
        // Disable Input
        inputEnabled = false;
        ioField.interactable = false;
        ioField.DeactivateInputField();
    }

    private void PlaySound()
    {
        // If Sound Exists
        if (sound)
        {
            // Get Audio Component
            AudioSource audio = GetComponent<AudioSource>();

            // Assign Sound to Component
            audio.clip = sound;

            // Play
            audio.PlayOneShot(sound);
        }
    }
}
