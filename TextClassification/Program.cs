using System;
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
                trainingSet = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\3\\TextClassification\\CS440-HW3\\fisher_train_2topic.txt";
            }

            string testSet = "";
            try
            {
                trainingSet = args[1];
            }
            catch
            {
                testSet = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\3\\TextClassification\\CS440-HW3\\fisher_test_2topic.txt";
            }

            int k = 1;
            Model mnb = new Model("MNB", trainingSet, testSet, k);  //MNB

            //TextClassification.Program test = new TextClassification.Program();
            //test.run_cmd("C:\\Users\\mabiscoc\\Documents\\Visual Studio 2013\\Projects\\TextClassification\\TextClassification\\plot_confusion_matrix.py", "");

            mnb.Train();
            mnb.Predict();

            // Assignment
            // Document, ACTUAL_PREDICTED
            
            // Get Confusion Matrix
            int trueP = 0;

            foreach (KeyValuePair<int, string> kvp in mnb.Prediction)
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

            Console.WriteLine("Multinomial Naive Bayes Accuracy: (" + trueP + "/" + mnb.Prediction.Count + ") " + (double)trueP / mnb.Prediction.Count);

            foreach(KeyValuePair<string, double> kvp in mnb.Prior)
            {
                Console.WriteLine("Top 10 Likelihoods for class: " + kvp.Key);
                int i = 1;
                foreach (KeyValuePair<string, double> kvp2 in mnb.Likelihood.OrderByDescending(v => v.Value).Where(y => y.Key.Split('|')[1] == kvp.Key).ToDictionary(x => x.Key, x => x.Value))
                {
                    Console.WriteLine("   " + i + ". " + kvp2.Key.Split('|')[0] + " : " + kvp2.Value);
                    i++;
                    if (i > 10)
                    {
                        break;
                    }
                }
                //mnb.Likelihood
            }


            k = 1;
            Model ber = new Model("BER", trainingSet, testSet, k);  //MNB

            //TextClassification.Program test = new TextClassification.Program();
            //test.run_cmd("C:\\Users\\mabiscoc\\Documents\\Visual Studio 2013\\Projects\\TextClassification\\TextClassification\\plot_confusion_matrix.py", "");

            ber.Train();
            ber.Predict();

            // Assignment
            // Document, ACTUAL_PREDICTED

            // Get Confusion Matrix
            trueP = 0;

            foreach (KeyValuePair<int, string> kvp in ber.Prediction)
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

            Console.WriteLine("Bernoulli Naive Bayes Accuracy: (" + trueP + "/" + ber.Prediction.Count + ") " + (double)trueP / ber.Prediction.Count);

            foreach (KeyValuePair<string, double> kvp in mnb.Prior)
            {
                Console.WriteLine("Top 10 Likelihoods for class: " + kvp.Key);
                int i = 1;
                foreach (KeyValuePair<string, double> kvp2 in mnb.Likelihood.OrderByDescending(v => v.Value).Where(y => y.Key.Split('|')[1] == kvp.Key).ToDictionary(x => x.Key, x => x.Value))
                {
                    Console.WriteLine("   " + i + ". " + kvp2.Key.Split('|')[0] + " : " + kvp2.Value);
                    i++;
                    if (i > 10)
                    {
                        break;
                    }
                }
                //mnb.Likelihood
            }
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
