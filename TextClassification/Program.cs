using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
//using Microsoft.Office.Interop.Word;
//using IronPython.Hosting;
//using Microsoft.Scripting.Hosting;

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
                trainingSet = @"I:\Backup\Masters\UIUC\2016\Fall\CS_440\Homework\3\TextClassification\CS440-HW3\fisher_train_40topic.txt";
            }

            string testSet = "";
            try
            {
                testSet = args[1];
            }
            catch
            {
                testSet = @"I:\Backup\Masters\UIUC\2016\Fall\CS_440\Homework\3\TextClassification\CS440-HW3\fisher_test_40topic.txt";
            }

            string pythonScript = "";
            try
            {
                pythonScript = args[2];
            }
            catch
            {
                pythonScript = @"I:\Backup\Masters\UIUC\2016\Fall\CS_440\Homework\3\TextClassification\CS440-HW3\TextClassification\plot_confusion_matrix.py";
            }

            //doPython();
            int i;

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

            string calculateTopLikelihood = "";
            int tLikelihood = 0;
            keepAsking = true;
            while (keepAsking)
            {
                Console.WriteLine("Want to calculate Top Likelihoods? (integer value)");
                Console.WriteLine("1. Yes");
                Console.WriteLine("2. No");
                calculateTopLikelihood = Console.ReadLine(); // Read string from console
                if (int.TryParse(calculateTopLikelihood, out tLikelihood)) // Try to parse the string as an integer
                {
                    if (tLikelihood > 2)
                    {
                        Console.WriteLine("Please enter value between 1 and 2!");
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

            string calculateOddsRatio = "";
            int oRatio = 0;
            keepAsking = true;
            while (keepAsking)
            {
                Console.WriteLine("Want to calculate Odds Ratio? (integer value)");
                Console.WriteLine("1. Yes");
                Console.WriteLine("2. No");
                calculateOddsRatio = Console.ReadLine(); // Read string from console
                if (int.TryParse(calculateOddsRatio, out oRatio)) // Try to parse the string as an integer
                {
                    if (oRatio > 2)
                    {
                        Console.WriteLine("Please enter value between 1 and 2!");
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

            string groupWords = "";
            int gWords = 0;
            double percentMatch = 0.0;
            string percentMatchStr = "";
            keepAsking = true;
            while (keepAsking)
            {
                Console.WriteLine("Want to group similar words? (integer value)");
                Console.WriteLine("1. Yes");
                Console.WriteLine("2. No");
                groupWords = Console.ReadLine(); // Read string from console
                if (int.TryParse(groupWords, out gWords)) // Try to parse the string as an integer
                {
                    if (gWords > 2)
                    {
                        Console.WriteLine("Please enter value between 1 and 2!");
                        continue;
                    }
                    else
                    {
                        if (gWords == 1)
                        {
                            Console.WriteLine("Want percentage should they match? (double value, 0.0 - 1.0)");
                            percentMatchStr = Console.ReadLine();
                            if (double.TryParse(percentMatchStr, out percentMatch)) // Try to parse the string as an integer
                            {
                                if (percentMatch > 1.0)
                                {
                                    Console.WriteLine("Please enter value between 0.0 and 1.0!");
                                    continue;
                                }
                                else
                                {
                                    keepAsking = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Not an double!");
                            }


                        }
                        keepAsking = false;
                    }
                }
                else
                {
                    Console.WriteLine("Not an integer!");
                }
            }

            string createWordCloud = "";
            int wCloud = 0;
            keepAsking = true;
            while (keepAsking)
            {
                Console.WriteLine("Want to see a word cloud for the test data? (integer value)");
                Console.WriteLine("1. Yes");
                Console.WriteLine("2. No");
                createWordCloud = Console.ReadLine(); // Read string from console
                if (int.TryParse(createWordCloud, out wCloud)) // Try to parse the string as an integer
                {
                    if (wCloud > 2)
                    {
                        Console.WriteLine("Please enter value between 1 and 2!");
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

            Model mnb = new Model("MNB", trainingSet, testSet, k);
            mnb.GroupWords = gWords;
            mnb.PercentMatch = percentMatch;
            Model ber = new Model("BER", trainingSet, testSet, k);
            ber.GroupWords = gWords;
            ber.PercentMatch = percentMatch;
            if (modelToRun == 1 || modelToRun == 3)
            {


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

                if (tLikelihood == 1)
                {
                    foreach (KeyValuePair<string, double> kvp in mnb.Prior)
                    {
                        Console.WriteLine("Top 10 Likelihoods for class: " + kvp.Key);
                        i = 1;
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
                if (oRatio == 1)
                {
                    // Builds Odds Ratio defined by the HW
                    // odds(Fij=1, c1, c2) = P(Fij=1 | c1) / P(Fij=1 | c2)
                    Dictionary<string, double> oddsRatio = new Dictionary<string, double>();
                    foreach (string word in mnb.UniqueWords)
                    {
                        // getVal in class
                        // class = -1, 1
                        oddsRatio.Add(word, mnb.Likelihood[word + "|-1"] / mnb.Likelihood[word + "|1"]);
                    }

                    i = 1;
                    Console.WriteLine("Top 10 Odds Ratio for class (c1 = -1, c2 = 1): ");
                    foreach (KeyValuePair<string, double> kvp in oddsRatio.OrderByDescending(v => v.Value).ToDictionary(x => x.Key, x => x.Value))
                    {
                        Console.WriteLine("   " + i + ". " + kvp.Key + " : " + kvp.Value);
                        i++;
                        if (i > 10)
                        {
                            break;
                        }
                    }
                }

                string actualVal = "";
                string predictedVal = "";
                string cls = "";
                Dictionary<int, Dictionary<string, int>> newTrainingData = new Dictionary<int, Dictionary<string, int>>();
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp in mnb.TrainingData.OrderBy(x => x.Key))
                {
                    newTrainingData.Add(Int32.Parse(kvp.Key), kvp.Value);
                }

                foreach (KeyValuePair<int, Dictionary<string, int>> kvp in newTrainingData.OrderBy(x => x.Key))
                {
                    cls = cls + "," + kvp.Key;
                }
                foreach (KeyValuePair<int, string> kvp in mnb.Prediction)
                {
                    actualVal = actualVal + "," + kvp.Value.Split('_')[0];
                    predictedVal = predictedVal + "," + kvp.Value.Split('_')[1];
                }

                TextClassification.Program python = new TextClassification.Program();
                python.run_cmd(pythonScript, actualVal.Trim(','), predictedVal.Trim(','), cls.Trim(','));


            }

            if (modelToRun == 2 || modelToRun == 3)
            {

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

                if (tLikelihood == 1)
                {
                    foreach (KeyValuePair<string, double> kvp in ber.Prior)
                    {
                        Console.WriteLine("Top 10 Likelihoods for class: " + kvp.Key);
                        i = 1;
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

                if (oRatio == 1)
                {
                    // Builds Odds Ratio defined by the HW
                    // odds(Fij=1, c1, c2) = P(Fij=1 | c1) / P(Fij=1 | c2)
                    Dictionary<string, double> oddsRatio = new Dictionary<string, double>();
                    foreach (string word in ber.UniqueWords)
                    {
                        // getVal in class
                        // class = -1, 1
                        oddsRatio.Add(word, ber.Likelihood[word + "|-1"] / ber.Likelihood[word + "|1"]);
                    }

                    i = 1;
                    Console.WriteLine("Top 10 Odds Ratio for class (c1 = -1, c2 = 1): ");
                    foreach (KeyValuePair<string, double> kvp in oddsRatio.OrderByDescending(v => v.Value).ToDictionary(x => x.Key, x => x.Value))
                    {
                        Console.WriteLine("   " + i + ". " + kvp.Key + " : " + kvp.Value);
                        i++;
                        if (i > 10)
                        {
                            break;
                        }
                    }
                }

                string actualVal = "";
                string predictedVal = "";
                string cls = "";
                Dictionary<int, Dictionary<string, int>> newTrainingData = new Dictionary<int, Dictionary<string, int>>();
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp in ber.TrainingData.OrderBy(x => x.Key))
                {
                    newTrainingData.Add(Int32.Parse(kvp.Key), kvp.Value);
                }

                foreach (KeyValuePair<int, Dictionary<string, int>> kvp in newTrainingData.OrderBy(x => x.Key))
                {
                    cls = cls + "," + kvp.Key;
                }
                foreach (KeyValuePair<int, string> kvp in ber.Prediction)
                {
                    actualVal = actualVal + "," + kvp.Value.Split('_')[0];
                    predictedVal = predictedVal + "," + kvp.Value.Split('_')[1];
                }

                TextClassification.Program python = new TextClassification.Program();
                python.run_cmd(pythonScript, actualVal.Trim(','), predictedVal.Trim(','), cls.Trim(','));


            }
            if (wCloud == 1)
            {
                // Create WordCloud
                string[] docs = System.IO.File.ReadAllLines(testSet);
                //int docId = 0;

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

                    if (modelToRun == 1 || modelToRun == 3)
                    {
                        foreach (KeyValuePair<string, Dictionary<string, int>> kvp in mnb.PredictionData.OrderBy(l =>l.Key))
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
                        foreach (KeyValuePair<string, Dictionary<string, int>> kvp in ber.PredictionData.OrderBy(l => l.Key))
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
            }
            do
            {
                Console.WriteLine("Press q to quit");
            } while (Console.ReadKey().KeyChar != 'q');

        }

        //private static void doPython()
        //{
        //    ScriptEngine engine = Python.CreateEngine();
        //    var paths = engine.GetSearchPaths();
        //    paths.Add(@"C:\Python27\Lib\site-packages");
        //    engine.SetSearchPaths(paths);
        //    engine.ExecuteFile(@"I:\Backup\Masters\UIUC\2016\Fall\CS_440\Homework\3\TextClassification\CS440-HW3\TextClassification\plot_confusion_matrix.py");
        //}

        private void run_cmd(string cmd, string arg1, string arg2, string arg3)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "c:\\python27\\python.exe";
            start.Arguments = string.Format("{0} {1} {2} {3}", cmd, arg1, arg2, arg3);
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
