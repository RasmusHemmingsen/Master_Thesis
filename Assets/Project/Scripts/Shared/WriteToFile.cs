using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class WriteToFile : MonoBehaviour
{
    private string m_DirectoryPath = "ExperimentResults";
    private string m_Filename;
    private string m_WholePath;

    private void Start()
    {
        if (m_Filename == null)
        {
            m_Filename = "Experiment " + DateTime.Now.ToString("yy-MM-dd-hh.mm", CultureInfo.CreateSpecificCulture("en-US")) + ".txt";
            m_WholePath = m_DirectoryPath + "/" + m_Filename;
        }
    }


    public void WriteAllDataToFile(LocomotionManager.LocomotionTechinique techinique)
    {
        string data = PepareTimeDataForWiriteTofile(techinique);
        WriteTimeDataToFile(data);
    }

    private string PepareTimeDataForWiriteTofile(LocomotionManager.LocomotionTechinique technique)
    {
        List<float> ListOfRoom1Time = TimeManager.m_TimeManager.GetRoomTimer1();
        List<float> ListOfRoom2Time = TimeManager.m_TimeManager.GetRoomTimer2();
        List<float> ListOfRoom3Time = TimeManager.m_TimeManager.GetRoomTimer3();

        string data = technique + "\n" +
            "Time total for room 1: " + ListOfRoom1Time[0] + "\n\n" +
            "Time first button room 2: " + ListOfRoom2Time[1] + "\n" +
            "Time second button room 2: " + ListOfRoom2Time[2] + "\n" +
            "Time to open door to room 3: " + ListOfRoom2Time[3] + "\n" +
            "Time total for room 2: " + ListOfRoom2Time[0] + "\n\n" +
            "Time for grap first cube room 3: " + ListOfRoom3Time[1] + "\n" +
            "Time for deliver first cube room 3: " + ListOfRoom3Time[2] + "\n" +
            "Time for grap second cube room 3: " + ListOfRoom3Time[3] + "\n" +
            "Time for deliver second cube room 3: " + ListOfRoom3Time[4] + "\n" +
            "Time for grap third cube room 3: " + ListOfRoom3Time[5] + "\n" +
            "Time for deliver third cube room 3: " + ListOfRoom3Time[6] + "\n" +
            "Time for grap forth cube room 3: " + ListOfRoom3Time[7] + "\n" +
            "Time for deliver forth cube room 3: " + ListOfRoom3Time[8] + "\n" +
            "Time for grap fifth cube room 3: " + ListOfRoom3Time[9] + "\n" +
            "Time for deliver fifth cube room 3: " + ListOfRoom3Time[10] + "\n" +
            "Time for grap sixth cube room 3: " + ListOfRoom3Time[11] + "\n" +
            "Time for deliver sixth cube room 3: " + ListOfRoom3Time[12] + "\n" +
            "Time for grap seventh cube room 3: " + ListOfRoom3Time[13] + "\n" +
            "Time for deliver seventh cube room 3: " + ListOfRoom3Time[14] + "\n" +
            "Time for grap first eighth room 3: " + ListOfRoom3Time[15] + "\n" +
            "Time for deliver first eighth room 3: " + ListOfRoom3Time[16] + "\n" +
             "Time total for room 3: " + ListOfRoom3Time[0] + "\n\n";
        return data;
    }

    private void WriteTimeDataToFile(string data)
    {
        if (!System.IO.Directory.Exists(m_DirectoryPath))
        {
            System.IO.Directory.CreateDirectory(m_DirectoryPath);
        }

        using (FileStream fileStream = new FileStream(m_WholePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fileStream);
            long endPoint = fileStream.Length;
            fileStream.Seek(endPoint, SeekOrigin.Begin);
            sw.WriteLine(data);
            sw.Flush();
        }
    }
}
