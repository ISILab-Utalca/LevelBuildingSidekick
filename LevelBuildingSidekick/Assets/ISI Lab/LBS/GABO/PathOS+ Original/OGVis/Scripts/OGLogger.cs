using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/*
OGLogger.cs
OGLogger (c) Ominous Games 2018-19 Atiya 2021
*/

public class OGLogger : MonoBehaviour 
{
    //Expected log row lengths.
    public const int POSLOG_L = 8;
    public const int INTLOG_L = 6;

    //Output and timers.
    private FileStream logStream;
    private StreamWriter logOutput;
    private string filename;
    private float sampleTimer = 0.0f;

    private static OGLogManager mgr;

    //temporary solution
    private PathOSAgent agent;

    private void Awake()
    {
        if (null == mgr)
            mgr = OGLogManager.instance;

        agent = GetComponent<PathOSAgent>();
    }

    private void Start()
    {
        sampleTimer = mgr.sampleTime;
    }

    private void Update()
	{
        //Sample position/orientation/health
        if (sampleTimer >= mgr.sampleTime)
        {
            sampleTimer -= mgr.sampleTime;
            LogPosition();
        }

        sampleTimer += Time.deltaTime;
    }

    public void InitStream(string filename)
    {
        this.filename = filename;

        logStream = new FileStream(filename, 
            FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        logOutput = new StreamWriter(logStream);

        logOutput.AutoFlush = true;
    }

    public void DisposeStream()
    {
        logOutput.Dispose();
        logStream.Dispose();
    }

    //Called from manager to write custom data into log file.
    public void WriteHeader(string header)
    {
        string line = OGLogManager.LogItemType.HEADER + "," +
            header;

        WriteLogLine(line);
    }

    //Called from manager for custom GameObject interactions.
    public void LogInteraction(string objectName, Transform location)
    {
        objectName = Regex.Replace(objectName, ",", string.Empty);

        string line = OGLogManager.LogItemType.INTERACTION + "," +
            mgr.gameTimer + "," +
            objectName + "," +
            location.position.x + "," +
            location.position.y + "," +
            location.position.z + "," +
            agent.GetHealth();

        WriteLogLine(line);
    }

    //Transform logging.
    private void LogPosition()
    {
        string line = OGLogManager.LogItemType.POSITION + "," +
            mgr.gameTimer + "," +
            transform.position.x + "," +
            transform.position.y + "," +
            transform.position.z + "," +
            transform.rotation.eulerAngles.x + "," +
            transform.rotation.eulerAngles.y + "," +
            transform.rotation.eulerAngles.z + "," +
            agent.GetHealth();

        WriteLogLine(line);
    }

    public void WriteLogLine(string line)
    {
        if (null == logOutput)
            return;

        try
        {
            logOutput.WriteLine(line);
        }
        catch(System.Exception e)
        {
            NPDebug.LogWarning("Exception raised while writing logfile: " +
                e.Message);

            NPDebug.LogError("Unable to write to logfile " + filename + "\n" + 
                "Try restarting the simulation.\n" +
                "If this error persists, try running Unity as an administrator and/or " +
                "whitelisting your logging directory in your antivirus software.");

            logOutput.Dispose();
        }
    }
}
