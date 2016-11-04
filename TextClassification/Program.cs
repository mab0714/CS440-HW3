using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;

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
                trainingSet = "C:\\Users\\mabiscoc\\Documents\\Visual Studio 2013\\Projects\\TextClassification\\CS440-HW3\\rt-train.txt";
            }

            string testSet = "";
            try
            {
                testSet = args[1];
            }
            catch
            {
                testSet = "C:\\Users\\mabiscoc\\Documents\\Visual Studio 2013\\Projects\\TextClassification\\CS440-HW3\\rt-test.txt";
            }

            string model = "";
            int modelToRun = 0;
            bool keepAsking = true;
            while (keepAsking)
            {
                Console.WriteLine("What model do you want to run? (1-3)");
                Console.WriteLine("1. Multinomial Naive Bayes");
                Console.WriteLine("2. Bernoulli Naive Bayes");
                Console.WriteLine("3. Both");

                model = Console.ReadLine(); // Read string from console
                if (int.TryParse(model, out modelToRun)) // Try to parse the string as an integer
                {
                    if (modelToRun > 3)
                    {
                        Console.WriteLine("Please enter value between 1 and 3!");
                        continue;
                    }
                    else
                    {
                        keepAsking = false;
                    }
                }
                else
                {
                    Console.WriteLine("Not an integer!");
                }
            }

            string smoothingConstant = "";
            int k = 0;
            keepAsking = true;
            while (keepAsking)
            {
                Console.WriteLine("What smoothing constant do you want to apply? (integer value)");
                smoothingConstant = Console.ReadLine(); // Read string from console
                if (int.TryParse(smoothingConstant, out k)) // Try to parse the string as an integer
                {                    
                        keepAsking = false;                    
                }
                else
                {
                    Console.WriteLine("Not an integer!");
                }
            }

            Model mnb = new Model("MNB", trainingSet, testSet, k);  
            Model ber = new Model("BER", trainingSet, testSet, k);  

            if (modelToRun == 1 || modelToRun == 3) 
            { 

                

                //TextClassification.Program test = new TextClassification.Program();
                //test.run_cmd("C:\\Users\\mabiscoc\\Documents\\Visual Studio 2013\\Projects\\TextClassification\\TextClassification\\plot_confusion_matrix.py", "");

                DateTime startTrain = DateTime.Now;
                mnb.Train();
                DateTime endTrain = DateTime.Now;
                DateTime startTest = DateTime.Now;
                mnb.Predict();
                DateTime endTest = DateTime.Now;
                // Assignment
                // Document, ACTUAL_PREDICTED
            
                // Get Confusion Matrix
                int accuratePrediction = 0;

                foreach (KeyValuePair<int, string> kvp in mnb.Prediction)
                {
                    int doc = kvp.Key;
                    string assignment = kvp.Value;

                    int actualValue = Int32.Parse(assignment.Split('_')[0]);
                    int predictedValue = Int32.Parse(assignment.Split('_')[1]);

                    if (actualValue == predictedValue)
                    {
                        accuratePrediction++;
                    }
                }
                Console.WriteLine("***************Multinomial Naive Bayes Summary:***************");
                Console.WriteLine(" Training Data: " + trainingSet);
                Console.WriteLine(" Test Data: " + testSet);
                Console.WriteLine(" Smoothing Constant: " + k);
                Console.WriteLine(" Accuracy: (" + accuratePrediction + "/" + mnb.Prediction.Count + ") " + (double)accuratePrediction / mnb.Prediction.Count);
                Console.WriteLine(" Training Started: " + startTrain);
                Console.WriteLine(" Training Ended: " + endTrain);
                Console.WriteLine(" Training Duration: " + (endTrain - startTrain));
                Console.WriteLine(" Testing Started: " + startTest);
                Console.WriteLine(" Testing Ended: " + endTest);
                Console.WriteLine(" Testing Duration: " + (endTest - startTest));
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

            }

            if (modelToRun == 2 || modelToRun == 3)
            {                           

<<<<<<< HEAD
                //TextClassification.Program test = new TextClassification.Program();
                //test.run_cmd("C:\\Users\\mabiscoc\\Documents\\Visual Studio 2013\\Projects\\TextClassification\\TextClassification\\plot_confusion_matrix.py", "");
=======
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
>>>>>>> origin/master

                DateTime startTrain = DateTime.Now;
                ber.Train();
                DateTime endTrain = DateTime.Now;
                DateTime startTest = DateTime.Now;
                ber.Predict();
                DateTime endTest = DateTime.Now;

                // Assignment
                // Document, ACTUAL_PREDICTED

                // Get Confusion Matrix
                int accuratePrediction = 0;

                foreach (KeyValuePair<int, string> kvp in ber.Prediction)
                {
                    int doc = kvp.Key;
                    string assignment = kvp.Value;

                    int actualValue = Int32.Parse(assignment.Split('_')[0]);
                    int predictedValue = Int32.Parse(assignment.Split('_')[1]);

                    if (actualValue == predictedValue)
                    {
                        accuratePrediction++;
                    }
                }

                Console.WriteLine("***************Bernoulli Naive Bayes Summary:***************");
                Console.WriteLine(" Training Data: " + trainingSet);
                Console.WriteLine(" Test Data: " + testSet);
                Console.WriteLine(" Smoothing Constant: " + k);            
                Console.WriteLine(" Accuracy: (" + accuratePrediction + "/" + ber.Prediction.Count + ") " + (double)accuratePrediction / ber.Prediction.Count);
                Console.WriteLine(" Training Started: " + startTrain);
                Console.WriteLine(" Training Ended: " + endTrain);
                Console.WriteLine(" Training Duration: " + (endTrain - startTrain));
                Console.WriteLine(" Testing Started: " + startTest);
                Console.WriteLine(" Testing Ended: " + endTest);
                Console.WriteLine(" Testing Duration: " + (endTest - startTest));

                foreach (KeyValuePair<string, double> kvp in ber.Prior)
                {
                    Console.WriteLine("Top 10 Likelihoods for class: " + kvp.Key);
                    int i = 1;
                    foreach (KeyValuePair<string, double> kvp2 in ber.Likelihood.OrderByDescending(v => v.Value).Where(y => y.Key.Split('|')[1] == kvp.Key).ToDictionary(x => x.Key, x => x.Value))
                    {
                        Console.WriteLine("   " + i + ". " + kvp2.Key.Split('|')[0] + " : " + kvp2.Value);
                        i++;
                        if (i > 10)
                        {
                            break;
                        }
                    }
                }
            }
            // Create WordCloud
            string[] docs = System.IO.File.ReadAllLines(testSet);
            int docId = 0;

<<<<<<< HEAD
            string wordCloud = testSet;
            wordCloud = wordCloud.Substring(0, wordCloud.IndexOf('.')) + ".htm";

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(wordCloud))
            {
                file.WriteLine("<html>");
                file.WriteLine("<head>");
                file.WriteLine("<title>Word Clouds</title>");
                file.WriteLine("</head>");
                file.WriteLine("<body>");
=======
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
>>>>>>> origin/master

                if (modelToRun == 1 || modelToRun == 3)
                {
                    foreach (KeyValuePair<string, Dictionary<string, int>> kvp in mnb.PredictionData)
                    {
                        file.WriteLine("<p>*********************Document: " + kvp.Key + "*********************</p>");
                        foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                        {
                            file.WriteLine("<font size='" + kvp2.Value + "'>" + kvp2.Key + "</font>");
                        }
                    }
                }
                if (modelToRun == 2)
                {
                    foreach (KeyValuePair<string, Dictionary<string, int>> kvp in ber.PredictionData)
                    {
                        file.WriteLine("<p>*********************Document: " + kvp.Key + "*********************</p>");
                        foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                        {
                            file.WriteLine("<font size='" + kvp2.Value + "'>" + kvp2.Key + "</font>");
                        }
                    }
                }

                //foreach (string doc in docs)
                //{
                //    file.WriteLine("<p>*********************Document: " + docId + "*********************</p>");
                //    // If the line doesn't contain the word 'Second', write the line to the file.
                //    string words = doc.Substring(doc.IndexOf(' ') + 1);
               
                //    foreach (string word in words.Split(' ').ToList())
                //    {
                //        string wd = word.Split(':')[0];
                //        int frequency = Int32.Parse(word.Split(':')[1]);
                //        file.WriteLine("<font size='"+ frequency + "'>" + wd + "</font>");                        
                //    }
                //    docId++;
                //}
                file.WriteLine("</body>");
                file.WriteLine("</html>");
            }

            Console.WriteLine(" Word Cloud created on test data: " + wordCloud);
            System.Diagnostics.Process.Start(wordCloud);

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
