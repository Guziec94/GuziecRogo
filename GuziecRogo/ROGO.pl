?- use_module(library(clpfd)).
/*
zapewnienie odpowiedniej długości listy oraz warunku OstatniElemet=Glowa
*/
inicjalizuj_liste(Dlugosc, JakasLista):-
	last(JakasLista, Element),
	nth0(0,JakasLista, Element),
	length(JakasLista, Dlugosc).
	
	
/*
sprawdzenie czy pola w liście znajdują się obok siebie;
pole poniżej/powyżej rożni się o szerokość;
pole po lewej/prawej różni się o 1;
*/
czy_sasiednie([H1, H2], Szerokosc, Szer):-
	S1 is H2 - Szerokosc,	/* Góra */	
 	S2 is H2 - 1,		/* Lewo */
 	S3 is H2 + 1,		/* Prawo */
 	S4 is H2 + Szerokosc,	/* Dół */
	X is H1 mod Szerokosc,
	(X = 0 -> member(H1, [S1, S2, S4]);
	X = Szer -> member(H1, [S1, S3, S4]);
	X \= 0 -> member(H1, [S1, S2, S3, S4])),
	!.
czy_sasiednie([H1, H2 | T], Szerokosc, Szer):-
	S1 is H2 - Szerokosc,	/* Góra */	
 	S2 is H2 - 1,		/* Lewo */
 	S3 is H2 + 1,		/* Prawo */
 	S4 is H2 + Szerokosc,	/* Dół */
	X is H1 mod Szerokosc,
	(X = 0 -> member(H1, [S1, S2, S4]);
	X = Szer -> member(H1, [S1, S3, S4]);
 	X \= 0 -> member(H1, [S1, S2, S3, S4])),
	czy_sasiednie([H2 | T], Szerokosc, Szer).

	
/*
sumowanie wartości wybranych indeksów
*/
sprawdz_sume([],_,0):-
	abort.
sprawdz_sume([H | T], ListaWartosci, Suma):-
	(length(T, 0),
	nth0(H, ListaWartosci, Glowa),
	Suma is Glowa
	;
	sprawdz_sume(T, ListaWartosci, SumaOgona),
	nth0(H, ListaWartosci, Glowa),
	Suma is SumaOgona + Glowa).
	
	
/*
Funkcja filtrująca indeksy, dla których wartość drugiej listy przyjmuje wartość dodatnią
*/
wybierz_niezerowe([], [], []).
wybierz_niezerowe([H | T], [_ | T1], S) :-
	H=<0,
	wybierz_niezerowe(T, T1, S).
wybierz_niezerowe([H | T], [H1 | T1], [H1 | S]) :-
	H>0,
	wybierz_niezerowe(T, T1, S).	


/*
Funkcja filtrująca indeksy, dla których wartość drugiej listy przyjmuje wartość dodatnią lub zerową
*/	
wybierz_nieujemne([], [], []).
wybierz_nieujemne([H | T], [_ | T1], S) :-
	H<0,
	wybierz_nieujemne(T, T1, S).
wybierz_nieujemne([H | T], [H1 | T1], [H1 | S]) :-
	H>=0,
	wybierz_nieujemne(T, T1, S).	

		
/*
funkcja do konwersji list na domain
*/
list_to_domain([H], H..HT) :-
	HT is H + 0.
list_to_domain([H | T], '\\/'(H .. HT, TDomain)) :-
	HT is H + 0,
	list_to_domain(T, TDomain).	
	
	
/*
Główna funkcja wywoływana przez GUI
*/	
szukaj(Szerokosc, Wysokosc, LiczbaKrokow, Dobrze, Najlepiej, ListaWartosci, ListaIndeksow):-
	IloscPol is Szerokosc*Wysokosc,
	LK is LiczbaKrokow+1,/*LK to długość listy wynikowej, gdzie element pierwszy jest dodany jako ostatni*/
	Szer is Szerokosc-1,/*Szer jest używane w sprawdzaniu sąsiedniości elementów*/
	inicjalizuj_liste(LK, ListaIndeksow),
	nth0(0, ListaIndeksow, _, SciezkaBezGlowy),		
	MaxIndeks is IloscPol-1,
	numlist(0, MaxIndeks, ZakresIndeksow),	
	wybierz_nieujemne(ListaWartosci, ZakresIndeksow, ListaNieujemnych),
	list_to_domain(ListaNieujemnych, ZestawNieujemnych),
	ListaIndeksow ins ZestawNieujemnych,/*zapewnienie, że elementy znalezionego rozwiązania nie będą zawierać ujemnych (nieprzechodnich) pól*/
	wybierz_niezerowe(ListaWartosci, ZakresIndeksow, ListaGlow),
	list_to_domain(ListaGlow, ZestawGlow),
	nth0(0,ListaIndeksow, Glowa),
	Glowa in ZestawGlow,
	all_different(SciezkaBezGlowy),
	sprawdz_sume(SciezkaBezGlowy, ListaWartosci, Suma),
	Suma#>=Dobrze,
	Suma#=<Najlepiej,	
	czy_sasiednie(ListaIndeksow, Szerokosc, Szer).
	

/*
Dodatkowa funkcja wywoływana przez GUI umożliwia wyszukiwanie ścieżek rozpoczynających się z podanego pola
*/	
szukaj_po_glowie(Szerokosc, Wysokosc, LiczbaKrokow, Dobrze, Najlepiej, ListaWartosci, Glowa, ListaIndeksow):-
	IloscPol is Szerokosc*Wysokosc,
	LK is LiczbaKrokow+1,/*LK to długość listy wynikowej, gdzie element pierwszy jest dodany jako ostatni*/
	Szer is Szerokosc-1,/*Szer jest używane w sprawdzaniu sąsiedniości elementów*/
	inicjalizuj_liste(LK, ListaIndeksow),
	nth0(0, ListaIndeksow, _, SciezkaBezGlowy),
	MaxIndeks is IloscPol-1,
	numlist(0, MaxIndeks, ZakresIndeksow),	
	wybierz_nieujemne(ListaWartosci, ZakresIndeksow, ListaNieujemnych),
	list_to_domain(ListaNieujemnych, ZestawNieujemnych),
	ListaIndeksow ins ZestawNieujemnych,/*zapewnienie, że elementy znalezionego rozwiązania nie będą zawierać ujemnych (nieprzechodnich) pól*/	
	nth0(0, ListaIndeksow, Glowa),	
	all_different(SciezkaBezGlowy),	
	sprawdz_sume(SciezkaBezGlowy, ListaWartosci, Suma),
	Suma#>=Dobrze,
	Suma#=<Najlepiej,	
	czy_sasiednie(ListaIndeksow, Szerokosc, Szer).