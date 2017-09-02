using UnityEngine;

public class Directory : MonoBehaviour
{
    private string folder, prompt;
    public string Folder { get { return folder; } set { folder = value; } }
    public string Prompt { get { return prompt; } set { prompt = value; } }


    public void Change(string path)
    {
        // Current Directory
        string temp = folder;

        // Levels to Move Up
        int upCount = 0;
        // Is Relative or Absolute?
        bool relative = false;
        // Is Moving Down?
        bool moveDown = false;

        // Replace any Double Slashes w/ Single
        path = path.Replace("//", "/").Replace("\\\\", "\\");

        // Move Up w/ Extended Directory Appended
        string[] splitPath = new string[] { };
        if (path.IndexOf("../") != -1 || path.IndexOf("..\\") != -1)
        {
            splitPath = path.Split('/');
            upCount = splitPath.Length - 1;
            relative = true;
        }
        // Move Up One Level
        else if (path == "..")
        {
            upCount = 1;
            relative = true;
        }
        // Stay at Current Directory
        else if (path == "./" || path == ".\\" || path == ".")
        {
            path = temp;
        }
        // Move Down, Relative to Current Folder
        else if (path.IndexOf(":") == -1)
        {
            moveDown = true;
            relative = true;
        }

        // Move Through Relative Path
        if (relative)
        {
            // Move Up
            if (!moveDown)
            {
                string addDirs = "";
                for (int i = 0; i < splitPath.Length; i++)
                {
                    if (splitPath[i].Length > 2)
                    {
                        int counter = 0;
                        for (int j = i; j < splitPath.Length; j++)
                        {
                            counter++;
                            addDirs += splitPath[j] + "\\";
                            if (counter > 1)
                                upCount--;
                        }
                        break;
                    }
                }
                folder = "";
                string[] folders = new string[] { };
                folders = prompt.Split('\\');
                int steps = folders.Length - upCount;
                if (steps > 0)
                {
                    for (int i = 0; i < steps; i++)
                        folder += folders[i] + "\\";
                }
                else
                    folder = folders[0] + "\\";
                folder += addDirs;
            }
            // Move Down
            else
                folder = temp + "\\" + path;
        }
        // Absolute Path - Use Whole String
        else
            folder = path;
    }
}
