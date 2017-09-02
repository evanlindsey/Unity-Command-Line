using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Command : MonoBehaviour
{
    // Change Restricted Commands/Reply in Editor
    public List<string> restrictedCommands;
    public string restrictedReply = "Unable to Comply";

    private Directory dir;


    private void Awake()
    {
        dir = GetComponent<Directory>();
    }

    public string Execute(string args)
    {
        // Handle Restricted Commands
        if (restrictedCommands.Contains(args))
            return restrictedReply + "\n";
        // Send CD Commands to 'ChangeDirectory'
        else if (args.Substring(0, 2).ToLower() == "cd" && args.Length > 2)
            return ChangeDirectory(args.Substring(3));
        else
        {
            try
            {
                // Use cmd.exe as a Background Process (w/ Args) at Assigned Directory
                Process p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.WorkingDirectory = dir.Folder;
                p.StartInfo.Arguments = "/C " + args;
                p.Start();

                // Read Output
                string output = p.StandardError.ReadToEnd();
                if (output.Length == 0)
                    output = p.StandardOutput.ReadToEnd();

                // Close Process / Return Output
                p.WaitForExit();
                return output;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }

    public string ChangeDirectory(string path)
    {
        // Store Current Directory ('null' on Launch)
        string temp = dir.Folder;
        if (temp != null)
            dir.Change(path);
            
        // Execute to Get Output
        string res = Execute("cd");
        if (res.IndexOf("Error") != -1)
        {
            // Directory Error, Use Original
            dir.Change(temp);
            return res + "\n";
        }
        else
        {
            // Remove '\r\n'
            res = res.Remove(res.Length - 2);

            // If Launch, Assign 'Folder' Var
            if (temp == null)
                dir.Folder = res;

            // Update Prompt
            dir.Prompt = res;
            return "";
        }
    }
}
