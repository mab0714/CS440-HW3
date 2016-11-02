﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TextClassification
{
    class Program
    {
        static void Main(string[] args)
        {            

            string trainingSet = "";
            try
            {
                trainingSet = args[0];
            }
            catch
            {
                trainingSet = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\3\\TextClassification\\fisher_train_2topic.txt";
            }

            string testSet = "";
            try
            {
                trainingSet = args[1];
            }
            catch
            {
                testSet = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\3\\TextClassification\\fisher_test_2topic.txt";
            }

            int k = 1;
            Model mod = new Model("MNB", trainingSet, testSet, k);  //MNB

            TextClassification.Program test = new TextClassification.Program();
            test.run_cmd("C:\\Users\\mabiscoc\\Documents\\Visual Studio 2013\\Projects\\TextClassification\\TextClassification\\plot_confusion_matrix.py", "");

            mod.Train();
            mod.Predict();

            // Assignment
            // Document, ACTUAL_PREDICTED
            
            // Get Confusion Matrix
            int trueN = 0;
            int trueP = 0;
            int falseN = 0;
            int falseP = 0;
            foreach (KeyValuePair<int, string> kvp in mod.Prediction)
            {
                int doc = kvp.Key;
                string assignment = kvp.Value;

                int actualValue = Int32.Parse(assignment.Split('_')[0]);
                int predictedValue = Int32.Parse(assignment.Split('_')[1]);

                if (actualValue == predictedValue)
                {
                    trueP++;
                }


            }

            Console.WriteLine("Accuracy: (" + trueP + "/" + mod.Prediction.Count + ") " + (double)trueP / mod.Prediction.Count);

        do

            {
                Console.WriteLine("Press q to quit");
            } while (Console.ReadKey().KeyChar != 'q');

        }

        private void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "c:\\python27\\python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

      
    }
}
