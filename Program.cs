using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RSA_05122017
{
    class Program
    {
        static UInt64 NWD(UInt64 a, UInt64 b)
        {
            UInt64 c;
            while(b != 0)
            {
                c = a % b;
                a = b;
                b = c;
            }
            return a;
        }

        //using extended Euclidean algorithm to find modular multiplicative inverse
        //x*e%euler = 1;

        //used
        static UInt64 MMIrecursive(UInt64 a, UInt64 b, UInt64 s0 = 1, UInt64 s1 = 0)
        {
            return b == 0 ? s0 : MMIrecursive(b, a % b, s1, s0 - s1 * (a / b));
        }

        //not used
        static UInt64 MMI(UInt64 e, UInt64 euler)
        {
            UInt64 t = 0, newt = 1,
                   r = euler, newr = e,
                   quotient, prov;

            while (newr != 0)
            {
                quotient = r / newr;

                prov = newt;
                newt = t - quotient * newt;
                t = prov;

                prov = newr;
                newr = r - quotient * newr;
                r = prov;
            }

            if (r > 1) Console.WriteLine("e jest nieodwracalne!");
            if (t < 0) t = t + euler;

            return t;
        }

        static void Szyfrowanie()
        {
            ///---Podanie p i q, przeliczenie czy się zgadza---
            int p = 0; bool pPrime = true; bool qSaved = false;
            int q = 0; bool qPrime = true; bool pSaved = false;

            int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317 };

            while (true)
            {
                Console.WriteLine("Liczby p oraz q muszą być różnymi od siebie liczbami pierwszymi z przedziału (1 tys. - 100 tys.)!");

                try
                {
                    if (!pSaved)
                    {
                        Console.WriteLine("Podaj p: ");
                        p = int.Parse(Console.ReadLine());
                        pPrime = true;
                    }

                    if (!qSaved)
                    {
                        Console.WriteLine("Podaj q: ");
                        q = int.Parse(Console.ReadLine());
                        qPrime = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                int pSquared = (int)(Math.Ceiling(Math.Sqrt(p)));
                int qSquared = (int)(Math.Ceiling(Math.Sqrt(q)));

                //spr. czy w przedziale
                if (p > 1000 && p < 100000 && q > 1000 && q < 100000)
                {
                    if (p == q)
                    {
                        pPrime = false;
                        qPrime = false;

                        Console.WriteLine("p nie może być równe q, podaj inne liczby!");
                        continue;
                    }

                    //spr. czy p pierwsze
                    foreach (int x in primes)
                    {
                        if (x < pSquared && !pSaved)
                        {
                            if (p % x == 0)
                            {
                                Console.WriteLine("p nie jest liczbą pierwszą, gdyż dzieli się na " + x.ToString());
                                pPrime = false;
                            }
                        }
                        if (x < qSquared && !qSaved)
                        {
                            if (q % x == 0)
                            {
                                Console.WriteLine("q nie jest liczbą pierwszą, gdyż dzieli się na " + x.ToString());
                                qPrime = false;
                            }
                        }
                    }

                    if (pPrime) pSaved = true;
                    if (qPrime) qSaved = true;

                    if (!pPrime || !qPrime) continue;
                    else break;

                }
                else
                {
                    Console.WriteLine("p lub q są spoza zakresu");
                    continue;
                }
            }
            ///----przeliczenie n oraz liczby eulera----

            UInt64 n = (UInt64)p * (UInt64)q;
            Console.WriteLine("n wynosi: " + n);

            UInt64 euler = (UInt64)(p - 1) * (UInt64)(q - 1);
            Console.WriteLine("phi(n) wynosi: " + euler);

            ///------------przeliczanie e oraz d--------------
            int e = 0;
            UInt64 d = 0;
            Random rnd = new Random();
            for (int it = rnd.Next(5, 10); it < primes.Length; it++)
            {
                if (NWD((UInt64)primes[it], euler) == (UInt64)1)
                {
                    e = primes[it];
                    d = MMIrecursive((UInt64)e, euler);

                    if (d > euler - 1) continue;
                    else break;
                }
            }
            Console.WriteLine("e wynosi: " + e);
            Console.WriteLine("d wynosi: " + d);

            ///--------przyjmowanie wiadomości---------
            string m;
            Console.WriteLine("\n-----\nPodaj wiadomosc (alfabet: [A-Za-z], małe litery są zamieniane na duże, nie używaj spacji):");
            m = Console.ReadLine();

            //Zamiana na duze litery
            m = m.ToUpper();

            //Zamiana na liczby zgodnie z tabl. ASCII
            byte[] asciiBytes = Encoding.ASCII.GetBytes(m);

            //--------szyfrowanie---------
            Console.WriteLine("\nSzyfrogram:");
            //szyfrogram(c) = (wiadomosc)^e mod n
            BigInteger[] ciphered = new BigInteger[asciiBytes.Length];

            for (int i = 0; i < asciiBytes.Length; i++)
            {
                ciphered[i] = BigInteger.ModPow(asciiBytes[i], e, n); // (int)(Pow(asciiBytes[i],(UInt16)e)%n);
                Console.Write(ciphered[i] + " ");
            }

            //-------deszyfrowanie---------
            Console.WriteLine("\n\nZaszyfrowałem (podano w celu sprawdzenia):");
            //wiadomosc(m) = (szyfrogram)^d mod n
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                Console.Write((char)(BigInteger.ModPow(ciphered[i], d, n)));
            }
        }

        static void Deszyfrowanie()
        {
            //---klucze
            UInt64 d = 0;
            do Console.WriteLine("Podaj klucz d:"); while (!UInt64.TryParse(Console.ReadLine(), out d));

            UInt64 n = 0;
            do Console.WriteLine("Podaj klucz n:"); while (!UInt64.TryParse(Console.ReadLine(), out n));

            //---szyfrogram
            char[] delimiters = { ' ', ',' };
            string ciphered;

            Console.WriteLine("\n\nPodaj szyfrogram rozdzielając liczby znakiem spacji bądź przecinka, zakończ ENTERem.");
            ciphered = Console.ReadLine();

            string[] cipheredDelimitered = ciphered.Split(delimiters);
            UInt64[] cipheredDelimiteredInts = new UInt64[cipheredDelimitered.Length];

            //---deszyfracja
            Console.WriteLine("\nRozszyfrowane:");

            for (int i = 0; i < cipheredDelimitered.Length; i++)
            {
                UInt64.TryParse(cipheredDelimitered[i], out cipheredDelimiteredInts[i]);
                Console.Write((char)(BigInteger.ModPow(cipheredDelimiteredInts[i], d, n)));
            }
        }

        //MAIN
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Chcesz (s)zyfrować czy (d)eszyfrować? Podaj odpowiednią literę.");
                char menu1 = Console.ReadKey().KeyChar;

                if(menu1 == 's')
                {
                    Console.Write("\n---\n");
                    Szyfrowanie();
                    break;
                }
                else if(menu1 == 'd')
                {
                    Console.Write("\n---\n");
                    Deszyfrowanie();
                    break;
                }
                else
                {
                    continue;
                }
            }

            Console.Write("\n\nNaciśnij ENTER aby zakończyć\n\n");
            while (Console.ReadKey().Key != ConsoleKey.Enter) ;
        }
    }
}
