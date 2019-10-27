using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class WriteToFile : MonoBehaviour
{
    private string m_DirectoryPath = "ExperimentResults";
    private string m_FilenameTime;
    private string m_FilenameDistance;
    private string m_WholePathTime;

    private void Start()
    {
        m_FilenameTime = "Experiment " + DateTime.Now.ToString("yy-MM-dd-hh.mm", CultureInfo.CreateSpecificCulture("en-US")) + " Time.txt";
        m_WholePathTime = m_DirectoryPath + "/" + m_FilenameTime;
        m_FilenameDistance = m_DirectoryPath + "/Experiment " + DateTime.Now.ToString("yy-MM-dd-hh.mm", CultureInfo.CreateSpecificCulture("en-US"));
    }


    public void WriteAllDataToFile(LocomotionManager.LocomotionTechinique techinique)
    {
        string filenameDistanceRoom1 = m_FilenameDistance + "Room_1_" + techinique + "Distance.csv";
        string filenameDistanceRoom2 = m_FilenameDistance + "Room_2_" + techinique + "Distance.csv";
        string filenameDistanceRoom3 = m_FilenameDistance + "Room_3_" + techinique + "Distance.csv";

        WriteDataToFile(PepareTimeDataForWiriteTofile(techinique), m_WholePathTime);
        WriteDataToFile(DistanceManager.m_DistanceManager.GetDistanceDataRoom1(), filenameDistanceRoom1);
        WriteDataToFile(DistanceManager.m_DistanceManager.GetDistanceDataRoom2(), filenameDistanceRoom2);
        WriteDataToFile(DistanceManager.m_DistanceManager.GetDistanceDataRoom3(), filenameDistanceRoom3);
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

    private void WriteDataToFile(string data, string path)
    {
        if (!System.IO.Directory.Exists(m_DirectoryPath))
        {
            System.IO.Directory.CreateDirectory(m_DirectoryPath);
        }

        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fileStream);
            long endPoint = fileStream.Length;
            fileStream.Seek(endPoint, SeekOrigin.Begin);
            sw.WriteLine(data);
            sw.Flush();
        }
    }
}
