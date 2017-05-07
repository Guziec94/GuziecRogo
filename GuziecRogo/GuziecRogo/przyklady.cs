using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuziecRogo
{
    class przyklady
    {
        public int szerokosc;
        public int wysokosc;
        public int dobry_wynik;
        public int najlepszy_wynik;
        public int liczba_krokow;
        public int[,] dane;
        public przyklady(int szerokosc, int wysokosc, int dobry_wynik, int najlepszy_wynik, int liczba_krokow, int[,] dane)
        {
            this.szerokosc = szerokosc;
            this.wysokosc = wysokosc;
            this.dobry_wynik = dobry_wynik;
            this.najlepszy_wynik = najlepszy_wynik;
            this.liczba_krokow = liczba_krokow;
            this.dane = dane;
        }
        public przyklady(int nr_przykladu)
        {
            switch (nr_przykladu)
            {
                case 0:
                    szerokosc = 4;
                    wysokosc = 6;
                    dobry_wynik = 2;
                    najlepszy_wynik = 3;
                    liczba_krokow = 6;
                    dane = new int[6, 4] {
                    {-1,-1,-1,-1},
                    {-1,0,1,-1},
                    {-1,1,0,-1},
                    {-1,0,1,-1},
                    {-1,0,0,-1},
                    {-1,-1,-1,-1}};
                break;
                case 1:
                    szerokosc = 4;
                    wysokosc = 4;
                    dobry_wynik = 5;
                    najlepszy_wynik = 6;
                    liczba_krokow = 12;
                    dane = new int[4, 4] {
                    {0,1,-1,9},
                    {1,0,1,0},
                    {0,1,0,1},
                    {0,0,1,0}};
                break;
                case 2:
                    szerokosc = 3;
                    wysokosc = 3;
                    dobry_wynik = 36;
                    najlepszy_wynik = 36;
                    liczba_krokow = 8;
                    dane = new int[3, 3] {
                    {1,2,3 },
                    {8,-1,4 },
                    {7,6,5 } };
                break;
            }
        }
    }
}