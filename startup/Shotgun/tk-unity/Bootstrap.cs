﻿//#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class Bootstrap
{
    // Do not use single-quotes in the passed string
    public static void RunPythonCodeOnClient(string pythonCodeToExecute)
    {
        string serverCode = string.Format(
@"
import sys
server_path = 'D:/unityProjects/rpycPrototype/PythonScripts'

if server_path not in sys.path:
  sys.path.append(server_path)

import server
server.run_python_code_on_client('{0}')
", pythonCodeToExecute);

        UnityEngine.Debug.Log(string.Format("Running Python: {0}", pythonCodeToExecute));
        PythonRunner.RunString(serverCode);
        UnityEngine.Debug.Log("  Done running Python on Client");
    }

    // Do not use single-quotes in the passed string
    public static void RunPythonFileOnClient(string pythonCodeToExecute)
    {
        string serverCode = string.Format(
@"
import sys
server_path = 'D:/unityProjects/rpycPrototype/PythonScripts'

if server_path not in sys.path:
  sys.path.append(server_path)

import server
server.run_python_file_on_client('{0}')
", pythonCodeToExecute);

        UnityEngine.Debug.Log(string.Format("Running Python: {0}", pythonCodeToExecute));
        PythonRunner.RunString(serverCode);
        UnityEngine.Debug.Log("  Done running Python on Client");
    }

    public static void CallBootstrap()
    {
        string bootstrapScript = System.Environment.GetEnvironmentVariable("SHOTGUN_UNITY_BOOTSTRAP_LOCATION");
        bootstrapScript = bootstrapScript.Replace(@"\", "/");
        UnityEngine.Debug.Log(string.Format("Invoking Shotgun Toolkit bootstrap script on client ({0})",bootstrapScript));
        RunPythonFileOnClient(bootstrapScript);

        //string contents = File.ReadAllText(System.Environment.GetEnvironmentVariable("SHOTGUN_UNITY_BOOTSTRAP_LOCATION"));
        //RunPythonCodeOnClient(contents);
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void OnReload()
    {
        CallBootstrap();
    }

#if DEBUG

#if UNITY_EDITOR_OSX
    const string ProjectRoot = "/Volumes/shotgun_s_drive/imgspc";
#else
    const string ProjectRoot = "S:/imgspc";
#endif

    [MenuItem("Shotgun/Debug/Bootstrap Engine")]
    public static void CallBootstrapEngine()
    {
        CallBootstrap();
    }

    [MenuItem("Shotgun/Debug/Manually Start Engine")]
    public static void CallStartEngine()
    {
        // TODO (fix) : PythonException: TankError : Missing required script user in config '/Users/inwoods/Library/Caches/Shotgun/imaginaryspaces/p120c45.basic.desktop/cfg/config/core/shotgun.yml'
        string pyScript = @"
import sgtk
tk = sgtk.sgtk_from_path('{0}')
ctx = tk.context_empty()
engine = sgtk.platform.start_engine('tk-unity', tk, ctx)
        ";

        pyScript = string.Format(pyScript, ProjectRoot);

        PythonRunner.RunString(pyScript);
    }

    [MenuItem("Shotgun/Debug/Restart Engine")]
    public static void CallRestartEngine()
    {
        string pyScript = @"
import sgtk
sgtk.platform.restart()
        ";

        PythonRunner.RunString(pyScript);
    }

    [MenuItem("Shotgun/Debug/Print Engine Envs")]
    public static void CallPrintEnv()
    {
        string[] envs = { "SHOTGUN_UNITY_BOOTSTRAP_LOCATION", "BOOTSTRAP_SG_ON_UNITY_STARTUP", };

        foreach (string env in envs)
        {
            UnityEngine.Debug.Log(env + ": " + System.Environment.GetEnvironmentVariable(env));
        }
    }
#endif

}
