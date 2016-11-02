using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TextClassification
{
    public class Model
    {
        // Node state
        private string _type;
        private int _trainingSize;
        private int _testSize;
        private int _smoothingConstant;
        private string _trainingSetFile;
        private string _testSetFile;        
        private Dictionary<string, Dictionary<string, int>> _trainingData = new Dictionary<string, Dictionary<string, int>>();
        private Dictionary<string, double> _likelihood = new Dictionary<string, double>();
        private Dictionary<string, double> _prior = new Dictionary<string, double>();
        private Dictionary<string, int> _classCounts = new Dictionary<string, int>();
        private Dictionary<int, string> _prediction = new Dictionary<int, string>();

        public Model(string type, string trainingSetFile, string testSetFile, int smoothingConstant)
        {
            this._type = type;
            this._trainingSetFile = trainingSetFile;
            this._testSetFile = testSetFile;
            this._smoothingConstant = smoothingConstant;
        }

        public int TrainingSize
        {
            get { return this._trainingSize; }
            set { this._trainingSize = value; }
        }

        public int TestSize
        {
            get { return this._testSize; }
            set { this._testSize = value; }
        }

        public string Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        public Dictionary<string, Dictionary<string, int>> TrainingData
        {
            get { return this._trainingData; }
            set { this._trainingData = value; }
        }

        public Dictionary<string, double> Likelihood
        {
            get { return this._likelihood; }
            set { this._likelihood = value; }
        }

        public Dictionary<string, double> Prior
        {
            get { return this._prior; }
            set { this._prior = value; }
        }

        public Dictionary<int, string> Prediction
        {
            get { return this._prediction; }
            set { this._prediction= value; }
        }

        public void Train()
        {
                string[] docs = System.IO.File.ReadAllLines(_trainingSetFile);
                
                _trainingSize = docs.Count();

                // Calculate Frequency Count 
                // Loop each document
                foreach (string doc in docs)
                {
                    // EX: label word_1:count1 word_2:count_2...word_n:count_n
                    string cls = doc.Split(' ')[0];  // hold label
                    string words = doc.Substring(doc.IndexOf(' ') + 1);  // hold word_1:count1 word_2:count_2...word_n:count_n
                    
                    // Dictionary of Word Frequencies per Class
                    Dictionary<string, int> dictWordCount = new Dictionary<string, int>();

                    // Maintain how many documents per class
                    int classCnt = 0;
                    if (_classCounts.ContainsKey(cls))
                    {
                        _classCounts.TryGetValue(cls, out classCnt);
                        _classCounts.Remove(cls);
                        _classCounts.Add(cls, classCnt + 1);
                    }
                    else
                    {
                        _classCounts.Add(cls, classCnt + 1);
                    }

                    // If document is part of a class that already exists in the dictionary
                    // recreate the dictionary of count of words in that class
                    // if class exists, drop and add an updated version
                    // EX: 1 {
                    //        word_1:2
                    //        word_2:4
                    //        word_n:3
                    //       }

                    if (_trainingData.ContainsKey(cls))
                    {
                        _trainingData.TryGetValue(cls, out dictWordCount);
                    }

                    foreach (string word in words.Split(' ').ToList())
                    {
                        // foreach word in the document, get the frequencies after the ':'                         
                        string wd = word.Split(':')[0];
                        int wordCnt = Int32.Parse(word.Split(':')[1]);

                        // if the word exists in the dictionary already, get the existing count
                        // and add the occurences from this new document.  otherwise add the word to the dictionary
                        if (dictWordCount.ContainsKey(wd))
                        {
                            int newCnt = dictWordCount[wd] + wordCnt;
                            dictWordCount.Remove(wd);
                            dictWordCount.Add(wd, newCnt);
                        }
                        else
                        {
                            dictWordCount.Add(wd, wordCnt);
                        }

                    }

                    if (_trainingData.ContainsKey(cls))
                    {
                        //_trainingData.TryGetValue(cls, out dictWordCount);

                        // class dictionary has been updated, remove the class and readd it with the updated dictionary
                        _trainingData.Remove(cls);
                        _trainingData.Add(cls, dictWordCount);

                    }
                    else
                    {
                        _trainingData.Add(cls, dictWordCount);
                    }
                }

                // Calculate Prior Probabilities
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData)
                {
                    _prior.Add(kvp.Key, (double)_classCounts[kvp.Key] / _trainingSize);
                }
            int uniqueWords = getUniqueWords();

            if (this._type.Equals("MNB"))
            {
                // Multinomial Naive Bayes
                // Calculate Likelihoods for each word in each class

                // Loop each class
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData)
                {
                    // kvp.Key = -1
                    // kvp.Value = { 
                    //              kvp2.Key = word1
                    //              kvp2.Value = 4
                    //             }
                    // Loop each word in the class
                    foreach (KeyValuePair<string, int> kvp2 in kvp.Value) {
                        int wordOccurrenceInClass = getOccurencesOfWordInClass(kvp.Key, kvp2.Key);
                        int totalWordsInDocsFromClass = getUniqueWordsInClass(kvp.Key);

                        _likelihood.Add(kvp2.Key + "|" + kvp.Key, (double)(wordOccurrenceInClass + _smoothingConstant) / (totalWordsInDocsFromClass + uniqueWords));

                    }
                }

            }
            else
            {
                //// Bernoulli Naive Bayes;
                //string[] lines = System.IO.File.ReadAllLines(_trainingSetFile);

                ////Dictionary<string, List<string>> TrainingData = new Dictionary<string, List<string>>();
                ////Dictionary<string, Dictionary<string, int>> _trainingData = new Dictionary<string, Dictionary<string, int>>();
                //_trainingSize = lines.Count();

                //// Calculate Frequency Count 
                //foreach (string line in lines)
                //{
                //    string cls = line.Split(' ')[0];
                //    string words = line.Substring(line.IndexOf(' ') + 1);
                //    Dictionary<string, int> dictWordCount = new Dictionary<string, int>();

                //    int classCnt = 0;
                //    if (_classCounts.ContainsKey(cls))
                //    {
                //        _classCounts.TryGetValue(cls, out classCnt);
                //        _classCounts.Remove(cls);
                //        _classCounts.Add(cls, classCnt + 1);
                //    }
                //    else
                //    {
                //        _classCounts.Add(cls, classCnt + 1);
                //    }

                //    if (_trainingData.ContainsKey(cls))
                //    {

                //        _trainingData.TryGetValue(cls, out dictWordCount);

                //        foreach (string word in words.Split(' ').ToList())
                //        {
                //            string wd = word.Split(':')[0];
                //            int wordCnt = Int32.Parse(word.Split(':')[1]);

                //            if (dictWordCount.ContainsKey(wd))
                //            {
                //                int newCnt = dictWordCount[wd] + wordCnt;
                //                dictWordCount.Remove(wd);
                //                dictWordCount.Add(wd, newCnt);
                //            }
                //            else
                //            {
                //                dictWordCount.Add(wd, wordCnt);
                //            }

                //        }
                //        _trainingData.Remove(cls);
                //        _trainingData.Add(cls, dictWordCount);

                //    }
                //    else
                //    {
                //        foreach (string word in words.Split(' ').ToList())
                //        {
                //            string wd = word.Split(':')[0];
                //            int wordCnt = Int32.Parse(word.Split(':')[1]);

                //            if (dictWordCount.ContainsKey(wd))
                //            {
                //                int newCnt = dictWordCount[wd] + wordCnt;
                //                dictWordCount.Remove(wd);
                //                dictWordCount.Add(wd, newCnt);
                //            }
                //            else
                //            {
                //                dictWordCount.Add(wd, wordCnt);
                //            }

                //        }
                //        _trainingData.Add(cls, dictWordCount);
                //    }
                //}

                //// Calculate Prior Probabilities
                //foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData)
                //{
                //    _prior.Add(kvp.Key, (double)_classCounts[kvp.Key] / _trainingSize);
                //}

                // Calculate Likelihoods for each word in each class
                // Loop each class 
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData)
                {
                    // Loop each word in the class
                    foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                    {
                        //_likelihood.Add(kvp2.Key + "|" + kvp.Key, (double)(getDocsContainingWord(kvp2.Key, docs, kvp.Key) + _smoothingConstant)/(_classCounts[kvp.Key]+uniqueWords));
                        _likelihood.Add(kvp2.Key + "|" + kvp.Key, (double)getDocsContainingWord(kvp2.Key, docs, kvp.Key)/_classCounts[kvp.Key]);
                    }
                }

            }
        }

        public void Predict()
        {
             string[] docs = System.IO.File.ReadAllLines(_testSetFile);
             int docId = 0;   
             _testSize = docs.Count();
            Dictionary<string, double> posteriorDict = new Dictionary<string, double>();

            // Calculate Posterior Probabilities
            // Loop each document
            foreach (string doc in docs)
             {
                string actualClass = doc.Split(' ')[0];
                string words = doc.Substring(doc.IndexOf(' ') + 1);

                // Loop each class
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData) 
                {
                    double posterior = 0.0; // 1.0;
                    double likelihood = 0.0;
                    if (kvp.Key.Equals("1") || kvp.Key.Equals("-1"))
                    {
                        ;
                    }
                    // Loop each word
                    foreach (string word in words.Split(' ').ToList())
                    {
                        string wd = word.Split(':')[0];
    
                        // Calculate product of all likelihoods for the given class...word|class ex: brad|-1
                        try 
                        {
                            //posterior = posterior * _likelihood[wd + "|" + kvp.Key];    

                            // to prevent underflow, use log
                            likelihood = likelihood + Math.Log10(_likelihood[wd + "|" + kvp.Key]);
                        }
                        catch
                        {
                            // ignore words that don't appear in the dictionary
                            ;
                        }
                    }
                    // multiply by prior of the class
                    //posterior = posterior * _prior[kvp.Key];
                    posterior = Math.Log10(_prior[kvp.Key]) + likelihood;

                    posteriorDict.Add(kvp.Key, posterior);

                }
                
                Dictionary<string, double> sortedPosterior = posteriorDict.OrderByDescending(v => v.Value).ToDictionary(x => x.Key, x => x.Value);
                // Doc, ACTUAL_PREDICTED class
                _prediction.Add(docId, actualClass + "_" + sortedPosterior.First().Key);
                posteriorDict.Clear();
                docId++;
            }
        }


        private int getDocsContainingWord(string word, string[] docs, string targetCls)
        {
            int occurances = 0;            
            foreach (string doc in docs)
            {
                string cls = doc.Split(' ')[0];
                string words = doc.Substring(doc.IndexOf(' ') + 1);
                if (targetCls.Equals(cls))
                {
                    if (doc.Contains(word))
                    {
                        occurances++;
                    }
                }
            }
            return occurances;
        }
        private int getUniqueWordsInClass(string cls) 
        {
            return this._trainingData[cls].Count();        
        }

        private int getOccurencesOfWordInClass(string cls, string word)
        {
            return this._trainingData[cls][word];
        }

        private int getUniqueWords()
        {
            List<string> uniqueWords = new List<string>();
            // Loop all classes
            foreach (KeyValuePair<string, Dictionary<string, int>> kvp in this._trainingData) 
            {
                // Loop dictionary of unique word frequency                    
                foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                {
                    // Add uniqueWords
                    if (!uniqueWords.Contains(kvp2.Key))
                    {
                        uniqueWords.Add(kvp2.Key);
                    }
                }
            }

            return uniqueWords.Count();
        }
    
    }
}
