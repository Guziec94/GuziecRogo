using System;
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
                    cell.Foreground = Brushes.LightGreen;//puste pole, zerowe
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            /*dataGrid2D.UnselectAllCells();
           var cos = new DataGridCellInfo(dataGrid2D.Items[1], dataGrid2D.Columns[2]);
           dataGrid2D.SelectedCells.Add(cos);
           dataGrid2D.Focus(); */
            //koloruj_komorke(0, 0, '2');

            /*if (!PlEngine.IsInitialized)
            {
            String[] param = { "-q" };  // suppressing informational and banner messages
            PlEngine.Initialize(param);
            PlQuery.PlCall("assert(father(martin, inka))");
            PlQuery.PlCall("assert(father(uwe, gloria))");
            PlQuery.PlCall("assert(father(uwe, melanie))");
            PlQuery.PlCall("assert(father(uwe, ayala))");
            using (var q = new PlQuery("father(P, C), atomic_list_concat([P,' is_father_of ',C], L)"))
            {
            foreach (PlQueryVariables v in q.SolutionVariables)
                label.Content += v["L"].ToString() + "\n";

            label.Content = "all children from uwe:" + "\n";
            q.Variables["P"].Unify("uwe");
            foreach (PlQueryVariables v in q.SolutionVariables)
                label.Content += v["C"].ToString() + "\n";
            }
            PlEngine.PlCleanup();
            label.Content += "finshed!" + "\n";
            }*/
        }

        private void stworz_tabele_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
            catch(Exception)
            {
                MessageBox.Show("Podano nieprawidłowe wymiary tabeli.");
            }
            /*DynamicGrid.Children.Clear();
            DynamicGrid.ColumnDefinitions.Clear();
            DynamicGrid.RowDefinitions.Clear();
            for (int i = 0; i < szerokosc; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                DynamicGrid.ColumnDefinitions.Add(gridCol);
            }
            for (int i = 0; i < wysokosc; i++)
            {
                RowDefinition gridRow = new RowDefinition();
                DynamicGrid.RowDefinitions.Add(gridRow);
            }
            DynamicGrid.Width = 578;
            DynamicGrid.HorizontalAlignment = HorizontalAlignment.Center;
            DynamicGrid.VerticalAlignment = VerticalAlignment.Center;
            DynamicGrid.ShowGridLines = true;
            DynamicGrid.Background = new SolidColorBrush(Colors.LightSteelBlue);*/
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
            catch(Exception)
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
            try
            {
                dobry_wynik = int.Parse(dobrze_textBox.Text);
                najlepszy_wynik = int.Parse(najlepiej_textBox.Text);
                liczba_krokow = int.Parse(liczba_krokow_textBox.Text);
                if (dobry_wynik <= 0 || najlepszy_wynik <= 0 || liczba_krokow < 4 || liczba_krokow % 2 != 0 || liczba_krokow > szerokosc * wysokosc)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wystąpił błąd, możliwości to:\n- podano nieprawidłowe progi punktowe dla tej gry\n- podano zbyt małą liczbę kroków,\n- liczba kroków jest nieparzysta, czyli gra jest nierozwiązywalna\n- liczba kroków jest większa niż liczba pól.");
                return;
            }
            int[] dane_z_tabeli = dataGrid2D.ItemsSource2D.Cast<int>().ToArray<int>();
            dane = new int[wysokosc, szerokosc];
            int iterator = 0;
            string lista_prologowa = "[";
            for (int i = 0; i < wysokosc; i++)//wiersz
            {
                lista_prologowa += "[";
                for (int j = 0; j < szerokosc; j++)//kolumna
                {
                    lista_prologowa += dane_z_tabeli[iterator++] + ",";
                }
                lista_prologowa = lista_prologowa.Substring(0, lista_prologowa.Length - 1) + "],";
            }
            lista_prologowa = lista_prologowa.Substring(0, lista_prologowa.Length - 1) + "]";
            znalezione_rozwiazania.Items.Clear();


            test();
            //tu można odpalać prologa
            if (!PlEngine.IsInitialized)
            {
                String sciezka = @"..\..\..\ROGO.pl";
                String[] param = { "-q -f",sciezka };  // suppressing informational and banner messages
                PlEngine.Initialize(param);
                using (var q = new PlQuery("szukaj(2,2,4,10,10,[1,2,3,4],Lista)."))
                {
                    String cos = "";
                    try
                    {
                        foreach (PlQueryVariables v in q.SolutionVariables)
                        {
                            cos += v["Lista"].ToString()+"\n";
                            znalezione_rozwiazania.Items.Add(v["Lista"].ToString());
                        }
                    }
                    catch(PlException E)
                    {
                        if(E.Message== "Execution Aborted")
                        {
                            label.Content = cos;
                            MessageBox.Show("Koniec");
                        }
                        else
                        {
                            MessageBox.Show(E.Message);
                        }
                    }
                }
                PlEngine.PlCleanup();
            }

            znalezione_rozwiazania.IsEnabled = true;
            wyswietl_rozwiazanie.IsEnabled = true;
        }

        private void test()//funkcja tworzy 2 tablice prologowe: jedna przechowujaca wartosci wierzcholkow, druga przechowujaca krawedzie grafu
        {
            int[] dane_z_tabeli = dataGrid2D.ItemsSource2D.Cast<int>().ToArray<int>();
            string wartosci_wierzcholkow = "[";//lista prologowa
            string krawedzie_grafu = "";//lista prologowa zawierajaca krawedzie grafu (nie powtarzajace sie - tylko w prawo i w dol)
            for (int komorka = 0; komorka < dane_z_tabeli.Length - 1; komorka++)
            {
                wartosci_wierzcholkow += dane_z_tabeli[komorka] + ", ";
            }
            wartosci_wierzcholkow += dane_z_tabeli[dane_z_tabeli.Length - 1] + "]";
            for (int komorka = 0; komorka < dane_z_tabeli.Length-1; komorka++)//-1 w zakresie, bo nie trzeba sprawdzac ostatniej komorki
            {
                if (dane_z_tabeli[komorka] >= 0)//sprawdza czy komorka jest przechodnia
                {
                    if (komorka < dane_z_tabeli.Length - szerokosc)
                    {
                        if (komorka % szerokosc == szerokosc - 1)//ostatnia komorka w rzedzie
                        {
                            if (dane_z_tabeli[komorka + szerokosc] >= 0)//sprawdza czy komorka jest przechodnia
                            {
                                krawedzie_grafu += "assert(krawedz(" + komorka + "," + (komorka + szerokosc) + ",1)).";
                            }
                        }
                        else
                        {
                            wartosci_wierzcholkow += dane_z_tabeli[komorka] + ", ";
                            if (dane_z_tabeli[komorka + 1] >= 0)//sprawdza czy komorka jest przechodnia
                            {
                                krawedzie_grafu += "assert(krawedz(" + komorka + "," + (komorka + 1) + ",1)).";
                            }
                            if (dane_z_tabeli[komorka + szerokosc] >= 0)//sprawdza czy komorka jest przechodnia
                            {
                                krawedzie_grafu += "assert(krawedz(" + komorka + "," + (komorka + szerokosc) + ",1)).";
                            }
                        }
                    }
                    else//odpowiada za ostatni rząd komórek
                    {
                        if (dane_z_tabeli[komorka] >= 0)//sprawdza czy komorka jest przechodnia
                        {
                            krawedzie_grafu += "assert(krawedz(" + komorka + "," + (komorka + 1) + ",1)).";
                        }
                    }
                }
            }
        }

        private void GetCombination(List<int> list)
        {
            double count = Math.Pow(2, list.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        Console.Write(list[j]);
                    }
                }
                Console.WriteLine();
            }
        }

        private void znalezione_rozwiazania_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void wyswietl_rozwiazanie_Click(object sender, RoutedEventArgs e)
        {
            sciezka_do_wyswietlenia = new int[] { 3, 3, 4, 3, 5, 3, 5, 4, 6, 4, 6, 5, 6, 6, 5, 6, 5, 5, 4, 5, 3, 5, 3, 4 };//kolumna, wiersz, kolumna, wiersz, ...
            for (int i=0;i< sciezka_do_wyswietlenia.Length;i+=2)
            {
                koloruj_komorke(sciezka_do_wyswietlenia[i], sciezka_do_wyswietlenia[i +1], '2');
            }
        }
    }
}