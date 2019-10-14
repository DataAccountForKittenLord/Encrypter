using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoder
{
    public static class Usage
    {
        public const string alphabetLower = "abcdefghijklmnopqrstuvwxyz";
        public const string alphabetCaps =  "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static int ToInt(this char sym)
        {
            return Convert.ToInt32(Convert.ToString(sym));
        }

        public static char ToChar(this int num)
        {
            return Convert.ToChar(Convert.ToString(num));
        }
    }

    public static class Encrypter
    {
        //private static bool AreRandNumsSet = false;


        private static bool Switched = false;


        private static int[] randOffset = new int[3];

        private static string GetPrefix(char symbol)
        {
            if (Char.IsDigit(symbol))
            {
                Switched = !Switched;
                if (Switched) return "%%//";
                return "//%%";
            }
            else if (Char.IsLetter(symbol))
            {
                Switched = !Switched;
                if (Switched) return "%/%/";
                return "/%/%";
            }
            else
            {
                Switched = !Switched;
                if (Switched) return "%%%%";
                return "////";
            }
        }

        private static string ReturnEncryptedSymbol(this char symbol)
        {
            string prefix = GetPrefix(symbol);
            string letter;

            if (Char.IsLetter(symbol))
            {
                bool caps = false;

                int temp;

                if (Usage.alphabetLower.Contains(symbol.ToString()))
                {
                    temp = Array.IndexOf(Usage.alphabetLower.ToCharArray(), symbol);
                }
                else
                {
                    temp = Array.IndexOf(Usage.alphabetCaps.ToCharArray(), symbol);
                    caps = true;
                }

                temp += GetCurrentRandomOffset();

                if(temp >= Usage.alphabetLower.Length)
                {
                    temp -= Usage.alphabetLower.Length;
                }

                if (caps)
                    letter = Usage.alphabetCaps[temp].ToString();
                else
                    letter = Usage.alphabetLower[temp].ToString();
            }

            else if (Char.IsNumber(symbol))
            {
                int temp = symbol.ToInt();

                temp += GetCurrentRandomOffset();

                letter = Usage.alphabetCaps[temp].ToString();
            }

            else
            {
                letter = symbol.ToString();
            }

            return prefix + letter;
        }

        private static void SettingRandOffset()
        {
            Random rnd = new Random();

            randOffset[0] = rnd.Next(1, 7);
            randOffset[1] = rnd.Next(1, 7);
            randOffset[2] = rnd.Next(1, 7);

            //AreRandNumsSet = true;
        }


        private static int RandomIndex = 0;

        private static int GetCurrentRandomOffset()
        {
            int ret = randOffset[RandomIndex];

            RandomIndex++;

            if (RandomIndex >= 3) RandomIndex = 0;

            return ret;
        }

        public static string Encrypt(this string toEncString)
        {
            SettingRandOffset();

            string retStr = "";

            retStr += $"%&{Usage.alphabetLower[randOffset[0]]}";

            retStr += $"%&{Usage.alphabetLower[randOffset[1]]}";

            for(int i = 0; i < toEncString.Length; i++)
            {
                retStr += toEncString[i].ReturnEncryptedSymbol();

                //Switched = !Switched;
            }

            retStr += $"%&{Usage.alphabetLower[randOffset[2]]}";

            return retStr;
        }
    }

    public static class Decrypter
    {        
        private static int[] randomOffset = new int[3];


        private static int RandomIndex = 0;

        private static int GetCurrentRandomOffset()
        {
            int ret = randomOffset[RandomIndex];

            RandomIndex++;

            if (RandomIndex >= 3) RandomIndex = 0;

            return ret;
        }

        private static void SetRandomOffset(string toDecStr)
        {
            randomOffset[0] = Array.IndexOf(Usage.alphabetLower.ToCharArray(), toDecStr[2]);
            randomOffset[1] = Array.IndexOf(Usage.alphabetLower.ToCharArray(), toDecStr[5]);
            randomOffset[2] = Array.IndexOf(Usage.alphabetLower.ToCharArray(), toDecStr[toDecStr.Length - 1]);
        }
        
        private static char ReturnDecryptedSymbol(this string toDecSymbol)
        {
            char toret = ' ';

            char toop;

            if(toDecSymbol[0] == toDecSymbol[2] && toDecSymbol[3] != toDecSymbol[2])       // Letter
            {
                int offset = GetCurrentRandomOffset();
                toop = toDecSymbol[4];


                if (Usage.alphabetLower.Contains(toop))
                {
                    //lower

                    int temp = Array.IndexOf(Usage.alphabetLower.ToCharArray(), toop) - offset;

                    if (temp < 0) temp += 26;

                    toret = Usage.alphabetLower[temp];
                }
                else
                {
                    //CAPS

                    int temp = Array.IndexOf(Usage.alphabetCaps.ToCharArray(), toop) - offset;

                    if (temp < 0) temp += 26;

                    toret = Usage.alphabetCaps[temp];
                }
            }
            else if(toDecSymbol[0] == toDecSymbol[1] && toDecSymbol[2] != toDecSymbol[1])  // Number
            {
                int offset = GetCurrentRandomOffset();
                toop = toDecSymbol[4];


                int temp = toop.ToInt();

                temp -= offset;

                toop = temp.ToChar();
            }
            else if (toDecSymbol[0] == toDecSymbol[3]) // Other
            {
                toret = toDecSymbol[4];
            }
            else                                       // Other
            {
                toret = toDecSymbol[4];
            }

            return toret;
        }
        
        public static string Decrypt(this string toDecStr)
        {
            SetRandomOffset(toDecStr);

            string retStr = "";

            for(int i = 6; i < toDecStr.Length-5; i += 5)
            {
                string temp = $"{toDecStr[i + 0]}{toDecStr[i + 1]}{toDecStr[i + 2]}{toDecStr[i + 3]}{toDecStr[i + 4]}";

                retStr += ReturnDecryptedSymbol(temp);
            }

            return retStr;
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write \"a\" to encrypt\nWrite \"b\" to decrypt");
            string result = Console.ReadLine();

            if (result != "a" && result != "b") 
            { 
                Console.WriteLine("Listen to instructions next time, nigga"); 
                Process.Start("https://youtu.be/Z8U8kf_BGbs?t=24"); 
                return; 
            }

            string str = result == "a" ? "Write your string to encrypt" : "Write your encrypted string";
            Console.WriteLine(str);
            string tocode = Console.ReadLine();

            switch (result)
            {
                case "a":
                    Console.WriteLine(tocode.Encrypt());
                    break;
                case "b":
                    Console.WriteLine(tocode.Decrypt());
                    break;
            }
        }
    }
}
