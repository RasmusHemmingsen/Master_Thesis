using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class WriteToFile : MonoBehaviour
{
    private const string DirectoryPath = "ExperimentResults";
    private string _filenameTime;
    private string _filenameDistance;
    private string _wholePathTime;

    private void Start()
    {
        _filenameTime = "Experiment " + DateTime.Now.ToString("yy-MM-dd-hh.mm", CultureInfo.CreateSpecificCulture("en-US")) + " Time.txt";
        _wholePathTime = DirectoryPath + "/" + _filenameTime;
        _filenameDistance = DirectoryPath + "/Experiment " + DateTime.Now.ToString("yy-MM-dd-hh.mm", CultureInfo.CreateSpecificCulture("en-US"));
    }


    public void WriteAllDataToFile(LocomotionManager.LocomotionTechnique technique)
    {
        var filenameDistanceRoom1 = _filenameDistance + "Room_1_" + technique + "Distance.csv";
        var filenameDistanceRoom2 = _filenameDistance + "Room_2_" + technique + "Distance.csv";
        var filenameDistanceRoom3 = _filenameDistance + "Room_3_" + technique + "Distance.csv";

        WriteDataToFile(PrepareTimeDataForWriteToFile(technique), _wholePathTime);
        WriteDataToFile(DistanceManager.DistanceManagerVariable.GetDistanceDataRoom1(), filenameDistanceRoom1);
        WriteDataToFile(DistanceManager.DistanceManagerVariable.GetDistanceDataRoom2(), filenameDistanceRoom2);
        WriteDataToFile(DistanceManager.DistanceManagerVariable.GetDistanceDataRoom3(), filenameDistanceRoom3);
    }

    private string PrepareTimeDataForWriteToFile(LocomotionManager.LocomotionTechnique technique)
    {
        var ListOfRoom1Time = TimeManager.TimeManagerVariable.GetRoomTimer1();
        var ListOfRoom2Time = TimeManager.TimeManagerVariable.GetRoomTimer2();
        var ListOfRoom3Time = TimeManager.TimeManagerVariable.GetRoomTimer3();

        var data = technique + "\n" +
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
        if (!Directory.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }

        var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        var sw = new StreamWriter(fileStream);
        var endPoint = fileStream.Length;
        fileStream.Seek(endPoint, SeekOrigin.Begin);
        sw.WriteLine(data);
        sw.Flush();
    }
}
