// Pawel Piatek
using System;
using System.IO;
using System.Diagnostics;
namespace HangmanGame
{
    class Game
    {
        private const string FILE_NAME_CAC = "countries_and_capitals.txt";
        private const string FILE_NAME_HS = "high_score.txt";

        static void Main(string[] args)
        {
            // load data from file 
            if (!File.Exists(FILE_NAME_CAC))
            {
                Console.WriteLine($"{FILE_NAME_CAC} not exists!");
            }
            string[] data = System.IO.File.ReadAllLines(FILE_NAME_CAC);


            // begin the game
            Console.WriteLine("Welcome to Hangman Game.");
            bool run = true;
            while (run)
            {

                // rand a country and capital
                Random generator = new Random();
                int n = generator.Next(data.Length);
                string country = data[n].Split(" | ")[0];
                string capital = data[n].Split(" | ")[1];
                capital = capital.ToUpper();

                // hide answer
                string answer = "";
                for (int i = 0; i < capital.Length; i++)
                {
                    if (capital[i] == ' ')
                    {
                        answer += ' ';
                    }
                    else
                    {
                        answer += '_';
                    }
                }

                Console.WriteLine("If you ready press Enter...");
                Console.ReadKey();

                // player guessing
                var watch = System.Diagnostics.Stopwatch.StartNew();
                string notInWord = "";
                int numberOfLives = 5;
                int numberOfAttempts = 0;
                bool win = false;
                bool guessing = true;
                while (guessing)
                {
                    show_board_game(numberOfLives, answer, notInWord, country);

                    string attempt = Console.ReadLine();
                    attempt = attempt.ToUpper();
                    numberOfAttempts++;

                    // check if this is letter or whole word
                    if (attempt.Length == 1)
                    {
                        // letter
                        bool guess = false;
                        string tmp = "";
                        for (int i = 0; i < capital.Length; i++)
                        {
                            if (attempt[0] == capital[i])
                            {
                                tmp += attempt[0];
                                guess = true;
                            }
                            else
                            {
                                tmp += answer[i];
                            }
                        }
                        answer = tmp;
                        if (!guess)
                        {
                            numberOfLives--;
                            notInWord += attempt[0] + ", ";
                        }

                        // check if player already guess the capital
                        if (answer == capital)
                        {
                            win = true;
                            guessing = false;
                        }
                    }
                    else
                    {
                        // whole word
                        if (attempt == capital)
                        {
                            answer = capital;
                            win = true;
                            guessing = false;
                        }
                        else
                        {
                            numberOfLives -= 2;
                        }
                    }
                    
                    // stop guessing if no lives
                    if (numberOfLives <= 0) guessing = false;
                }

                watch.Stop();
                show_board_game(numberOfLives, answer, notInWord, country);

                // finish game
                if (win)
                {
                    var myTime = watch.ElapsedMilliseconds / 1000.0;
                    Console.WriteLine($"You guessed the capital after {numberOfAttempts} letters. It took you {myTime} seconds.");

                    string[] positions = new string[11];
                    bool askForName = true;
                    using (StreamReader sr = File.OpenText(FILE_NAME_HS))
                    {
                        // check if there is a new record
                        string position;
                        string header = sr.ReadLine(); // avoid header 
                        int i = 0;
                        positions[i] = header; // header
                        while ((position = sr.ReadLine()) != null)
                        {
                            i++;
                            string hsTime = position.Split(" | ")[2];
                            float fhsTime = float.Parse(hsTime);

                            // save a new record
                            if (myTime < fhsTime && askForName)
                            {
                                askForName = false;
                                Console.WriteLine("New record: Give your name: ");
                                string name = Console.ReadLine();
                                DateTime now = DateTime.Now;
                                string new_position = name + " | " + now + " | " + myTime + " | " + capital;
                                positions[i] = new_position;
                            }
                            else
                            {
                                positions[i] = position;
                            }
                        }
                    }

                    using (StreamWriter sw = File.CreateText(FILE_NAME_HS))
                    {
                        for (int i = 0; i < 11; i++)
                        {
                            sw.WriteLine(positions[i]);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"You lose. The proper answer was {capital}");
                }
                
                // show high score after finish game
                show_high_scores();

                // restart game
                string decision;
                do
                {
                    Console.WriteLine("Do you want to restart game? (y/n)");
                    decision = Console.ReadLine();
                }
                while (decision != "n" && decision != "y");
                if (decision == "n" || decision == "N") run = false;
                else Console.Clear();
            }
        }

        static void show_high_scores()
        {
            // load high score
            Console.WriteLine("\nHigh Score");
            using (StreamReader sr = File.OpenText(FILE_NAME_HS))
            {
                string position;
                while ((position = sr.ReadLine()) != null)
                {
                    Console.WriteLine(position);
                }
            }
        }

        static void show_board_game(int numberOfLives, string answer, string notInWord, string country)
        {
            Console.Clear();
            Console.WriteLine($"Your have {numberOfLives} lives left.");
            draw_body(numberOfLives);
            Console.WriteLine(answer);
            Console.WriteLine($"The letter you tried: {notInWord}");
            Console.Write("Give a letter or guess the whole word: ");
            if (numberOfLives < 2) Console.Write($"\nTIP: The capital of {country}: ");
        }

        static void draw_body(int numberOfLives)
        {
            // draw a body
            if (numberOfLives <= 4)
            {
                Console.WriteLine("____");
            }
            else
            {
                Console.WriteLine();
            }
            if (numberOfLives <= 3)
            {
                Console.WriteLine("|  |");
            }
            else
            {
                Console.WriteLine();
            }
            if (numberOfLives <= 2)
            {
                Console.WriteLine("|  o");
            }
            else
            {
                Console.WriteLine();
            }
            if (numberOfLives <= 1)
            {
                Console.WriteLine("| /|\\");
            }
            else
            {
                Console.WriteLine();
            }
            if (numberOfLives <= 0)
            {
                Console.WriteLine("| / \\");
            }
            else
            {
                Console.WriteLine();
            }
        }
    }


}
