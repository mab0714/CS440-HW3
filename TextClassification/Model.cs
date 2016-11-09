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
        private Dictionary<string, Dictionary<string, int>> _predictionData = new Dictionary<string, Dictionary<string, int>>();

        private Dictionary<string, double> _likelihood = new Dictionary<string, double>();
        private Dictionary<string, double> _prior = new Dictionary<string, double>();
        private Dictionary<string, int> _classCounts = new Dictionary<string, int>();
        private Dictionary<int, string> _prediction = new Dictionary<int, string>();
        private List<string> _uniqueWords = new List<string>();

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

        public Dictionary<string, Dictionary<string, int>> PredictionData
        {
            get { return this._predictionData; }
            set { this._predictionData = value; }
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
            set { this._prediction = value; }
        }

        public List<string> UniqueWords
        {
            get { return this._uniqueWords; }
            set { this._uniqueWords = value; }
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

                foreach (string word in this._uniqueWords)
                {

                    // Loop each class
                    foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData)
                    {
                        // kvp.Key = -1
                        // kvp.Value = { 
                        //              kvp2.Key = word1
                        //              kvp2.Value = 4
                        //             }
                        // Loop each word in the class

                        // Need to loop all words, not just words per class
                        // need to account for words that aren't in the class for this class and give it the smoothing constant

                        //foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                        //{

                        int wordOccurrenceInClass = getOccurencesOfWordInClass(kvp.Key, word);
                        int totalWordsInDocsFromClass = getUniqueWordsInClass(kvp.Key);

                        _likelihood.Add(word + "|" + kvp.Key, (double)(wordOccurrenceInClass + _smoothingConstant) / (totalWordsInDocsFromClass + uniqueWords));

                        //}
                    }


                }


                //// Loop each class
                //foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData)
                //{
                //    // kvp.Key = -1
                //    // kvp.Value = { 
                //    //              kvp2.Key = word1
                //    //              kvp2.Value = 4
                //    //             }
                //    // Loop each word in the class

                //    // Need to loop all words, not just words per class
                //    // need to account for words that aren't in the class for this class and give it the smoothing constant

                //    foreach (KeyValuePair<string, int> kvp2 in kvp.Value) {
                //        int wordOccurrenceInClass = getOccurencesOfWordInClass(kvp.Key, kvp2.Key);
                //        int totalWordsInDocsFromClass = getUniqueWordsInClass(kvp.Key);

                //        _likelihood.Add(kvp2.Key + "|" + kvp.Key, (double)(wordOccurrenceInClass + _smoothingConstant) / (totalWordsInDocsFromClass + uniqueWords));

                //    }
                //}

            }
            else
            {
                //// Bernoulli Naive Bayes;

                // Calculate Likelihoods for each word in each class
                // Loop each class 
                foreach (string word in this._uniqueWords)
                {
                    foreach (KeyValuePair<string, Dictionary<string, int>> kvp in _trainingData)
                    {
                        // Loop each word in the class
                        //foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                        //{
                        //_likelihood.Add(kvp2.Key + "|" + kvp.Key, (double)(getDocsContainingWord(kvp2.Key, docs, kvp.Key) + _smoothingConstant)/(_classCounts[kvp.Key]+uniqueWords));
                        _likelihood.Add(word + "|" + kvp.Key, (double)(getDocsContainingWord(word, docs, kvp.Key) + 1) / (_classCounts[kvp.Key] + 2));
                        //}
                    }
                }

            }
        }

        public void Predict()
        {
            string[] docs = System.IO.File.ReadAllLines(_testSetFile);
            int docId = 0;
            _testSize = docs.Count();


            // Calculate Frequency Count 
            // Loop each document
            foreach (string doc in docs)
            {
                // EX: label word_1:count1 word_2:count_2...word_n:count_n
                string cls = doc.Split(' ')[0];  // hold label
                string words = doc.Substring(doc.IndexOf(' ') + 1);  // hold word_1:count1 word_2:count_2...word_n:count_n

                // Dictionary of Word Frequencies per Class
                Dictionary<string, int> dictWordCount = new Dictionary<string, int>();

                // If document is part of a class that already exists in the dictionary
                // recreate the dictionary of count of words in that class
                // if class exists, drop and add an updated version
                // EX: 1 {
                //        word_1:2
                //        word_2:4
                //        word_n:3
                //       }

                if (_predictionData.ContainsKey(cls))
                {
                    _predictionData.TryGetValue(cls, out dictWordCount);
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

                if (_predictionData.ContainsKey(cls))
                {
                    // class dictionary has been updated, remove the class and readd it with the updated dictionary
                    _predictionData.Remove(cls);
                    _predictionData.Add(cls, dictWordCount);

                }
                else
                {
                    _predictionData.Add(cls, dictWordCount);
                }
            }

            Dictionary<string, double> posteriorDict = new Dictionary<string, double>();

            if (this._type.Equals("MNB"))
            {
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

                        // Loop each word
                        foreach (string word in words.Split(' ').ToList())
                        {
                            string wd = word.Split(':')[0];
                            int frequency = Int32.Parse(word.Split(':')[1]);

                            // Calculate product of all likelihoods for the given class...word|class ex: brad|-1
                            try
                            {
                                //posterior = posterior * _likelihood[wd + "|" + kvp.Key];    

                                // to prevent underflow, use log
                                likelihood = likelihood + Math.Log10(_likelihood[wd + "|" + kvp.Key])*frequency;
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
            else
            {
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

                        // Loop each word
                        foreach (string word in words.Split(' ').ToList())
                        {
                            string wd = word.Split(':')[0];
                            int frequency = Int32.Parse(word.Split(':')[1]);

                            // Calculate product of all likelihoods for the given class...word|class ex: brad|-1
                            try
                            {
                                //posterior = posterior * _likelihood[wd + "|" + kvp.Key];    

                                // to prevent underflow, use log
                                if (this._uniqueWords.Contains(wd))
                                {
                                    likelihood = likelihood + Math.Log10(_likelihood[wd + "|" + kvp.Key]);
                                }
                                else
                                {
                                    likelihood = likelihood + Math.Log10(1 - _likelihood[wd + "|" + kvp.Key]);
                                }
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
            try
            {
                return this._trainingData[cls][word];
            }
            catch
            {
                return 0;
            }
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
            this._uniqueWords = uniqueWords;
            return uniqueWords.Count();
        }

        //http://social.technet.microsoft.com/wiki/contents/articles/26805.c-calculating-percentage-similarity-of-2-strings.aspx

        // <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary>
        int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }
        double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }

    }
}
