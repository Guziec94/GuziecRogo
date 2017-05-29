using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuziecRogo
{
    class rozwiazanie
    {
        public string sciezka;
        public int[] sciezka_do_wyswietlenia;
        public int wynik;
        
        public rozwiazanie(string s, int[] lista_wartosci)
        {
            sciezka = s;
            sciezka_do_wyswietlenia = s.Substring(1, s.Length - 2).Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            wynik = 0;
            for(int i=0;i<sciezka_do_wyswietlenia.Length-1;i++)
            {
                wynik += lista_wartosci[sciezka_do_wyswietlenia[i]];
            }
        }

        public override string ToString()
        {
            return "W=" + wynik + " L=" + sciezka + "";
        }
    }
}
