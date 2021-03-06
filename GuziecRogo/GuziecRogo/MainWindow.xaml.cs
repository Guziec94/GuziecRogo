﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Collections.Generic;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;

namespace GuziecRogo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public int szerokosc;
        public int wysokosc;
        public int dobry_wynik;
        public int najlepszy_wynik;
        public int liczba_krokow;
        public int start;
        public int[,] dane;//tablica zawierająca dane z DataGrid2d dane[wiersz,kolumna]
        public int[] sciezka_do_wyswietlenia;//tablica przechowujaca rozwiazanie gotowe do wyswietlenia (format: [kolumna, wiersz]*ilosc_krokow)

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void koloruj_komorke(int kolumna, int wiersz, char wariant)
        {
            DataGridRow row = dataGrid2D.ItemContainerGenerator.ContainerFromItem(dataGrid2D.Items[wiersz]) as DataGridRow;
            DataGridCell cell = dataGrid2D.Columns[kolumna].GetCellContent(row).Parent as DataGridCell;
            if (wariant == '1')//zwykle pole
            {
                cell.Background = Brushes.White;
                cell.Foreground = Brushes.Black;
            }
            else if (wariant == '2')//pole sciezki
            {
                cell.Background = Brushes.LightGreen;
                if (dane[wiersz, kolumna] == 0)
                {
                    cell.Foreground = Brushes.Black;//puste pole, zerowe
                }
                else
                {
                    cell.Foreground = Brushes.Black;
                }
            }
            else if (wariant == '0')//pole puste, zerowe
            {
                cell.Background = Brushes.White;
                cell.Foreground = Brushes.White;
            }
            else if (wariant == '-')//pole zablokowane
            {
                cell.Background = Brushes.Black;
                cell.Foreground = Brushes.Black;
            }
        }
        private void koloruj_komorke(int kolumna, int wiersz, int wartosc)
        {
            DataGridRow row = dataGrid2D.ItemContainerGenerator.ContainerFromItem(dataGrid2D.Items[wiersz]) as DataGridRow;
            DataGridCell cell = dataGrid2D.Columns[kolumna].GetCellContent(row).Parent as DataGridCell;
            if (wartosc > 0)//zwykle pole
            {
                cell.Background = Brushes.White;
                cell.Foreground = Brushes.Black;
            }
            else if (wartosc == 0)//pole puste, zerowe
            {
                cell.Background = Brushes.White;
                cell.Foreground = Brushes.White;
            }
            else if (wartosc < 0)//pole zablokowane
            {
                cell.Background = Brushes.Black;
                cell.Foreground = Brushes.Black;
            }
        }

        private string tablica_prologowa(int[] tablica)
        {
            string wyjscie = "[";
            foreach (var liczba in tablica)
            {
                wyjscie += liczba + ",";
            }
            wyjscie = wyjscie.Substring(0, wyjscie.Length - 1) + "]";
            return wyjscie;
        }

        private void stworz_tabele_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                znalezione_rozwiazania.ItemsSource = null;
                szerokosc = Convert.ToInt32(szerokosc_textBox.Text);
                wysokosc = Convert.ToInt32(wysokosc_textBox.Text);
                dataGrid2D.Background = Brushes.White;
                dataGrid2D.Foreground = Brushes.White;
                dataGrid2D.RowHeight = (double)550 / wysokosc - 0.1;
                dataGrid2D.ColumnWidth = (double)550 / szerokosc - 0.1;
                if (wysokosc > szerokosc)
                {
                    dataGrid2D.FontSize = (double)550 / wysokosc * 0.5;
                }
                else
                {
                    dataGrid2D.FontSize = (double)550 / szerokosc * 0.5;
                }
                if (szerokosc <= 1 || wysokosc <= 1)
                {
                    throw new Exception();
                }
                dane = new int[wysokosc, szerokosc];
                dataGrid2D.ItemsSource2D = dane;
                znajdz_rozwiazanie.IsEnabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Podano nieprawidłowe wymiary tabeli.");
            }
        }

        private void dataGrid2D_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                var kolumna = e.Column.DisplayIndex;
                var wiersz = e.Row.GetIndex();
                string wartosc_komorki = ((TextBox)e.EditingElement).Text;
                if (wartosc_komorki.Substring(0, 1) == "-")//nieprzechodnia komorka - cala czarna
                {
                    koloruj_komorke(kolumna, wiersz, '-');
                }
                else if (wartosc_komorki == "0")//komorka przechodnia o zerowej wartosci - cala biala
                {
                    koloruj_komorke(kolumna, wiersz, '0');
                }
                else//komorka z dodatnia wartoscia - czarny tekst, biale tlo
                {
                    koloruj_komorke(kolumna, wiersz, '1');
                }
            }
            catch (Exception)
            { }
        }

        private void wyswietl_przykladowe_dane()
        {
            szerokosc_textBox.Text = szerokosc.ToString();
            wysokosc_textBox.Text = wysokosc.ToString();
            liczba_krokow_textBox.Text = liczba_krokow.ToString();
            dobrze_textBox.Text = dobry_wynik.ToString();
            najlepiej_textBox.Text = najlepszy_wynik.ToString();
            dataGrid2D.Background = Brushes.White;
            dataGrid2D.Foreground = Brushes.White;
            dataGrid2D.RowHeight = (double)550 / wysokosc - 0.1;
            dataGrid2D.ColumnWidth = (double)550 / szerokosc - 0.1;
            if (wysokosc > szerokosc)
            {
                dataGrid2D.FontSize = (double)550 / wysokosc * 0.5;
            }
            else
            {
                dataGrid2D.FontSize = (double)550 / szerokosc * 0.5;
            }
            dataGrid2D.ItemsSource2D = dane;
            znajdz_rozwiazanie.IsEnabled = true;
        }

        private void lista_przykladow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            znalezione_rozwiazania.ItemsSource = null;
            wyswietl_rozwiazanie.IsEnabled = false;
            przyklady test = new przyklady(lista_przykladow.SelectedIndex);
            szerokosc = test.szerokosc;
            wysokosc = test.wysokosc;
            liczba_krokow = test.liczba_krokow;
            dobry_wynik = test.dobry_wynik;
            najlepszy_wynik = test.najlepszy_wynik;
            dane = test.dane;
            wyswietl_przykladowe_dane();
            ((IInvokeProvider)(new ButtonAutomationPeer(koloruj_tabele).GetPattern(PatternInterface‌​.Invoke))).Invoke();
        }

        private void koloruj_tabele_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < szerokosc; i++)//kolumna
            {
                for (int j = 0; j < wysokosc; j++)//wiersz
                {
                    koloruj_komorke(i, j, dane[j, i]);
                }
            }
        }

        private void znajdz_rozwiazanie_Click(object sender, RoutedEventArgs e)
        {
            sciezka_do_wyswietlenia = null;
            try//sprawdzenie poprawnosci wprowadzonych danych
            {
                dobry_wynik = int.Parse(dobrze_textBox.Text);
                najlepszy_wynik = int.Parse(najlepiej_textBox.Text);
                liczba_krokow = int.Parse(liczba_krokow_textBox.Text);
                if (dobry_wynik <= 0 || najlepszy_wynik <= 0 || liczba_krokow < 4 || liczba_krokow % 2 != 0 || liczba_krokow > szerokosc * wysokosc)
                {
                    throw new Exception();
                }
                if (uproszczone_wyszukiwanie.IsChecked == true)
                {
                    start = int.Parse(uproszczone_wyszukiwanie_TextBox.Text);
                    if (start < 0 || start >= szerokosc * wysokosc)
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wystąpił błąd, możliwości to:\n- podano nieprawidłowe progi punktowe dla tej gry\n- podano zbyt małą liczbę kroków,\n- liczba kroków jest nieparzysta, czyli gra jest nierozwiązywalna\n- liczba kroków jest większa niż liczba pól,\n- warość pola uproszczone wyszukiwanie jest błędna (<0 lub >=" + (szerokosc * wysokosc) + ").");
                znajdz_rozwiazanie.Content = "Znajdź rozwiązanie";
                return;
            }

            int iterator = 0;
            int[] dane_z_tabeli = dataGrid2D.ItemsSource2D.Cast<int>().ToArray<int>();//załadowanie danych z tabeli do tablicy intów
            if (dane_z_tabeli[start] < 0 && uproszczone_wyszukiwanie.IsChecked == true)
            {
                MessageBox.Show("Punkt startowy w uproszczonym wyszukiwaniu musi być przechodnim polem.");
                return;
            }
            dane = new int[wysokosc, szerokosc];
            for (int j = 0; j < wysokosc; j++)//wierszem 
            {
                for (int i = 0; i < szerokosc; i++)//kolumna
                {
                    dane[j, i] = dane_z_tabeli[iterator++];
                }
            }

            if (!PlEngine.IsInitialized)
            {
                String sciezka = @"..\..\..\ROGO.pl";//ścieżka do pliku ROGO.pl
                String[] param = { "-q -f", sciezka };//parametry uruchomieniowe prologa
                PlEngine.Initialize(param);//uruchomienie prologa z wcześniej ustalonymi parametrami
            }
            if (PlEngine.IsInitialized)
            {
                String lista_wartosci = tablica_prologowa(dane_z_tabeli);//prologowa lista zawierająca wartości wszystkich pól
                String zapytanie = "";
                if (uproszczone_wyszukiwanie.IsChecked == true)
                {
                    zapytanie = "szukaj_po_glowie(" + szerokosc + "," + wysokosc + "," + liczba_krokow + "," + dobry_wynik + "," + najlepszy_wynik + "," + lista_wartosci + "," + start + ",Lista).";
                }
                else
                {
                    zapytanie = "szukaj(" + szerokosc + "," + wysokosc + "," + liczba_krokow + "," + dobry_wynik + "," + najlepszy_wynik + "," + lista_wartosci + ",Lista).";
                }
                List<rozwiazanie> cos = new List<rozwiazanie>();//jedynie do testów
                try
                {
                    using (var q = new PlQuery(zapytanie))
                    {
                        foreach (PlQueryVariables v in q.SolutionVariables)
                        {
                            string test = v["Lista"].ToString();
                            rozwiazanie znalezione_rozwiazanie = new rozwiazanie(test, dane_z_tabeli);
                            cos.Add(znalezione_rozwiazanie);
                        }
                    }
                }
                catch (PlException E)
                {
                    if (E.Message == "Execution Aborted")//poprawne zakończenie - wywoływane z ROGO.pl
                    {
                        cos = cos.OrderBy(x => x.wynik).ThenBy(x => x.sciezka).ToList();
                        znalezione_rozwiazania.ItemsSource = cos;
                        MessageBox.Show("Zakończono wyszukiwanie.");
                    }
                    else
                    {
                        MessageBox.Show(E.Message);
                    }
                }
                //PlEngine.PlCleanup();//moze powodowac bledy podczas szybkiego uruchomienia kolejnego zapytania
            }
            znalezione_rozwiazania.IsEnabled = true;
            wyswietl_rozwiazanie.IsEnabled = true;
        }

        private void znalezione_rozwiazania_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((IInvokeProvider)(new ButtonAutomationPeer(koloruj_tabele).GetPattern(PatternInterface‌​.Invoke))).Invoke();
            if (e.AddedItems.Count > 0)
            {
                sciezka_do_wyswietlenia = ((rozwiazanie)e.AddedItems[0]).sciezka_do_wyswietlenia;
            }
        }

        private void wyswietl_rozwiazanie_Click(object sender, RoutedEventArgs e)
        {
            if (znalezione_rozwiazania.HasItems && sciezka_do_wyswietlenia != null) 
            {
                int wiersz, kolumna;
                foreach (int numer_komorki in sciezka_do_wyswietlenia)
                {
                    wiersz = numer_komorki / szerokosc;
                    kolumna = numer_komorki - wiersz * szerokosc;
                    koloruj_komorke(kolumna, wiersz, '2');
                }
            }
        }

        private void uproszczone_wyszukiwanie_Click(object sender, RoutedEventArgs e)
        {
            if (uproszczone_wyszukiwanie.IsChecked == true)
            {
                uproszczone_wyszukiwanie_TextBox.IsEnabled = true;
            }
            else
            {
                uproszczone_wyszukiwanie_TextBox.IsEnabled = false;
            }
        }
    }
}