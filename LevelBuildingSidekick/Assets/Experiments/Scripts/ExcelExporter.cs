using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ExcelExporter
{
    public static void ExportToExcel(List<Tuple<string,List<string>>> dataColumns, string fielname)
    {
        string filePath = Application.dataPath + "/Experiments/Logs/" + fielname + ".csv";
        // Create a new Excel document
        StringBuilder sb = new StringBuilder();
        StreamWriter writer = new StreamWriter(filePath);

        // Add column headers
        for (int i = 0; i < dataColumns.Count; i++)
        {
            List<string> column = dataColumns[i].Item2;
            sb.Append(dataColumns[i].Item1 + ",");
        }
        sb.Length--; // Remove the trailing comma
        sb.AppendLine();

        // Determine the maximum number of rows among the columns
        int maxRows = 0;
        foreach (var column in dataColumns)
        {
            if (column.Item2.Count > maxRows)
                maxRows = column.Item2.Count;
        }

        // Add data rows
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < dataColumns.Count; col++)
            {
                List<string> column = dataColumns[col].Item2;
                if (row < column.Count)
                    sb.Append($"{column[row]},");
                else
                    sb.Append(",");
            }
            sb.Length--; // Remove the trailing comma
            sb.AppendLine();
        }

        // Save the document
        writer.Write(sb.ToString());
        writer.Close();
        //File.WriteAllText(filePath, sb.ToString());

        Debug.Log("Data exported to Excel successfully: " + filePath);
    }
}