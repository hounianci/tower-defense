using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class FileUtil
{

    public static List<List<int>> readFileMatrix(string path){
        List<List<int>> matrix = new List<List<int>>();
        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            // 从文件读取并显示行，直到文件的末尾 
            while ((line = sr.ReadLine()) != null)
            {
                List<int> rowContent = new List<int>();
                String[] cells = line.Trim().Split(',');
                for (int i=0; i<cells.Length; i++)
                {
                    rowContent.Add(int.Parse(cells[i]));
                }
                matrix.Add(rowContent);
            }
        }
        // return matrixTurnRight(matrix);
        return matrix;
    }
    public static string readFile(string path){
        string s = "";
        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            // 从文件读取并显示行，直到文件的末尾 
            while ((line = sr.ReadLine()) != null)
            {
                s += line;
            }
        }
        return s;
    }

    public static List<List<int>> matrixTurnRight(List<List<int>> matrix){
        List<List<int>> matrixTmp = new List<List<int>>();
        for(int i=0; i<matrix[0].Count; i++){
            matrixTmp.Add(new List<int>());
        }
        for(int i=matrix.Count-1; i>=0; i--){
            for(int j=0; j<matrix[i].Count; j++){
                matrixTmp[j].Add(matrix[i][j]);
            }
        }
        return matrixTmp;
    }
    public static List<List<int>> matrixTurnLeft(List<List<int>> matrix){
        List<List<int>> matrixTmp = new List<List<int>>();
        for(int i=0; i<matrix[0].Count; i++){
            matrixTmp.Add(new List<int>());
        }
        for(int i=0; i<matrix.Count; i++){
            for(int j=0; j<matrix[i].Count; j++){
                matrixTmp[matrix[i].Count-1-j].Add(matrix[i][j]);
            }
        }
        return matrixTmp;
    }
    //水平旋转
    public static List<List<int>> matrixTurnAround(List<List<int>> matrix){
        List<List<int>> matrixTmp = new List<List<int>>();
        for(int i=0; i<matrix.Count; i++){
            matrixTmp.Add(new List<int>());
        }
        for(int i=0; i<matrix.Count; i++){
            for(int j=0; j<matrix[i].Count; j++){
                matrixTmp[matrix.Count-i-1].Add(matrix[i][j]);
            }
        }
        return matrixTmp;
    }
    //垂直旋转
    public static List<List<int>> matrixTurnAround2(List<List<int>> matrix){
        List<List<int>> matrixTmp = new List<List<int>>();
        for(int i=0; i<matrix.Count; i++){
            matrixTmp.Add(new List<int>());
        }
        for(int i=0; i<matrix.Count; i++){
            for(int j=0; j<matrix[i].Count; j++){
                matrixTmp[i].Add(matrix[i][matrix[i].Count-1-j]);
            }
        }
        return matrixTmp;
    }

}
