using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApp12
{
    class scoreAndChromosome //değerleri kullanabilmek ve karşılabilmek için oluşturulan sınıf
    {
        public char[] chromosome { get; set; }
        public int fitnessScore { get; set; }
        public scoreAndChromosome()
        {
            chromosome = new char[19];
        }
        public scoreAndChromosome(char[] gens, int score)
        {
            this.chromosome = gens;
            this.fitnessScore = score;
        }
        public scoreAndChromosome(char[] gens)
        {
            this.chromosome = gens;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            Random random = new Random();
            int populationSize = 100; //kullanılacak olan popülasyonun sayısı
            int parentsNum = 50; //ebeveyn sayısı
            int eliteSize = 20; //popülasyondan seçilen ve yeni nesile direkt olarak geçecek olan iyi kromozomların sayısı
            string realPasscode = "YapayZekaYöntemleri"; //çözülecek olan şifre
            int passcodeLength = realPasscode.Length;
            char[] chars = new char[passcodeLength]; //çözülecek şifrenin uzunluğunda dizi tanımlama
            chars = realPasscode.ToCharArray();
            int passcodeLowerBound = 0;
            int passcodeUpperBound = passcodeLength;
            int testingSize = 5; //başarımızı ölçebilmek için aynı işlemi kaç kere denemek istediğimizi gireceğimiz değişken. 
            int generationsOfFive = 0; //başarı hesabı için beş denemedeki değerleri toplayacağımız değişken
            long times = 0; //ortalama süre hesabı için toplam süreyi tutacağımız değişken
            Console.WriteLine("IN THE PROJECT\nPOPULATİON SİZE : {0}\nPARENTS SİZE : {1}\nELİTE SİZE : {2}",populationSize,parentsNum,eliteSize);
            for (int count = 0; count < testingSize; count++) //test sayısı kadar kodumuzu çalıştıralım.
            {
                Console.WriteLine("{0}. TEST RESULTS: ",count+1);
                watch.Start();
                string realDiscoveredPasscode = "";
                int generation = 0;
                string controle = "";                
                string theNearestPasscode = "";
                string realTheNearestPasscode = "";
                theNearestPasscode = "";
                List<scoreAndChromosome> population = new List<scoreAndChromosome>();
                for (int t = 0; t < populationSize; t++)
                {
                    scoreAndChromosome chromosome = new scoreAndChromosome();
                    for (int j = 0; j < passcodeLength; j++)
                    {
                        chromosome.chromosome[j] = chars[random.Next(passcodeLowerBound, passcodeUpperBound)];
                    }
                    population.Add(chromosome);
                }
                while (true)
                {
                    generation++;
                    List<scoreAndChromosome> fitnessScores = fitness(population);
                    foreach (scoreAndChromosome fitScore in population)
                    {
                        if (fitScore.fitnessScore == passcodeLength)
                        {
                            string discoveredPasscode = "";
                            for (int j = 0; j < passcodeLength; j++)
                            {
                                discoveredPasscode += fitScore.chromosome[j];
                            }
                            realDiscoveredPasscode += discoveredPasscode;
                            controle = "var";
                            break;
                        }
                        else if (fitScore.fitnessScore == passcodeLength - 1 & theNearestPasscode == "")
                        {
                            for (int k = 0; k < passcodeLength; k++)
                            {
                                theNearestPasscode += fitScore.chromosome[k];
                            }
                            realTheNearestPasscode += theNearestPasscode;
                            break;
                        }
                    }
                    if (controle == "var")
                    {
                        break;
                    }
                    List<scoreAndChromosome> parents = selectParents(fitnessScores);
                    List<scoreAndChromosome> children = createChildren(parents);
                    population = mutation(children);
                }
                watch.Stop();
                generationsOfFive += generation;
                times += watch.ElapsedMilliseconds;
                Console.WriteLine("Cracked in {0} generations, and {1} milliseconds! \nSecret passcode = {2} \nThe nearest passcode ={3} \nDiscovered passcode = {4}", generation, watch.Elapsed.Milliseconds, realPasscode, realTheNearestPasscode, realDiscoveredPasscode);
                Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            }
            double meanOfGenerations = generationsOfFive / testingSize; //ortalama kaçıncı jenerasyonda bulunuyor
            double meanOfTimes = times / testingSize; //ortalama kaç saniyede bulunuyor
            Console.WriteLine("Mean of testing sets generations : {0} generations\nMean of testing times : {1} milliseconds", meanOfGenerations, meanOfTimes);
            Console.ReadKey();

            //genetik algoritmalarda iyi olan genlere göre sıralama ve seçim yapabilmemiz için şifreyle eşleşen karakter sayısını hesapladığımız kod parçası
            List<scoreAndChromosome> fitness(List<scoreAndChromosome> nPopulation)
            {
                for (int i = 0; i < populationSize; i++)
                {
                    int matches = 0;
                    for (int j = 0; j < passcodeLength; j++)
                    {
                        if (realPasscode[j] == nPopulation[i].chromosome[j])
                        {
                            matches++;
                        }
                    }
                    nPopulation[i].fitnessScore = matches;
                }
                return nPopulation;
            }

            //eşleşen karakter sayısına göre sıralayıp ebeveyn sayısı kadar seçim yapan kod parçası
            List<scoreAndChromosome> selectParents(List<scoreAndChromosome> fitnessScores)
            {
                scoreAndChromosome tester = new scoreAndChromosome();
                List<scoreAndChromosome> parents = new List<scoreAndChromosome>();
                for (int i = 0; i < populationSize; ++i)
                {
                    for (int j = 0; j < populationSize; ++j)
                    {
                        if (fitnessScores[i].fitnessScore > fitnessScores[j].fitnessScore)
                        {
                            tester = fitnessScores[i];
                            fitnessScores[i] = fitnessScores[j];
                            fitnessScores[j] = tester;
                        }
                    }
                }
                List<scoreAndChromosome> parentList = new List<scoreAndChromosome>();
                for (int i = 0; i < parentsNum; i++)
                {
                    parents.Add(fitnessScores[i]);
                }
                return parents;
            }

            //çocuk oluşumunda gereken çaprazlama işlemini yapan kod parçası
            scoreAndChromosome crossOver(scoreAndChromosome parent1, scoreAndChromosome parent2)
            {
                int genA = random.Next(0, 19);
                int genB = random.Next(0, 19);
                int startGen = Math.Min(genA, genB);
                int endGen = Math.Max(genA, genB);
                scoreAndChromosome newChild = new scoreAndChromosome();
                for (int i = 0; i < realPasscode.Length; ++i)
                {
                    if (i < startGen || i > endGen)
                    {
                        newChild.chromosome[i] = parent1.chromosome[i];
                    }
                    else
                    {
                        newChild.chromosome[i] = parent2.chromosome[i];
                    }
                }
                return newChild;
            }

            //rastgele seçilen ebeveynleri çaprazlama işlemine alıp çocukları oluşturan kod parçası
            List<scoreAndChromosome> createChildren(List<scoreAndChromosome> parents)
            {
                List<scoreAndChromosome> children = new List<scoreAndChromosome>();
                int numberOfChildren = populationSize - eliteSize;
                for (int i = 0; i < eliteSize; i++)
                {
                    children.Add(parents[i]);
                }
                for (int i = 0; i < numberOfChildren; i++)
                {
                    scoreAndChromosome parent1 = parents[random.Next(parentsNum)];
                    scoreAndChromosome parent2 = parents[random.Next(parentsNum)];
                    children.Add(crossOver(parent1, parent2));
                }
                return children;
            }

            //belirli bir olasılığa göre rastgele bir genin değiştirilmesi (mutasyon) işleminin gerçekleştiği kod parçası
            List<scoreAndChromosome> mutation(List<scoreAndChromosome> childrenSet)
            {
                for (int i = 0; i < childrenSet.Count; i++)
                {
                    if (random.NextDouble() > 0.1)
                    {
                        continue;
                    }
                    else
                    {
                        int mutatedPosition = random.Next(passcodeLength);
                        char mutatedGen = chars[random.Next(chars.Length)];
                        childrenSet[i].chromosome[mutatedPosition] = mutatedGen;
                    }
                }
                return childrenSet;
            }
        }
    }
}
