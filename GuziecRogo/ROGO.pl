?- use_module(library(clpfd)).
%initialization(set_prolog_flag(clpfd_goal_expansion, false)).

/*
	zapewnienie odpowiedniej dlugosci listy i OstatniElemet=Glowa
*/
inicjalizuj_liste(Dlugosc, JakasLista):-
	last(JakasLista,Element),
	nth0(0,JakasLista,Element),
	length(JakasLista, Dlugosc).
	
	
/*
	sprawdzenie czy pola w liscie znajduja sie obok siebie - pole ponizej lub powyzej rozni sie o szerokosc
*/
czy_sasiednie([H1,H2],Szerokosc):-
	S1 is H2 - Szerokosc, S2 is H2 - 1, S3 is H2 + 1, S4 is H2 + Szerokosc,
	%write(H1+' = '+[S1, S2, S3, S4]), nl,
	%H1 in {S1,S2,S3,S4},/*wersja z ograniczeniami chyba nieco wolniejsza*/
	member(H1, [S1, S2, S3, S4]),
	!.
czy_sasiednie([H1,H2|T],Szerokosc):-
	S1 is H2 - Szerokosc, S2 is H2 - 1, S3 is H2 + 1, S4 is H2 + Szerokosc,
	%write(H1+' = '+[S1, S2, S3, S4]), nl,
	%H1 in {S1,2,S3,S4},/*wersja z ograniczeniami chyba nieco wolniejsza*/
	member(H1, [S1, S2, S3, S4]),
	czy_sasiednie([H2|T],Szerokosc).

	
/*
	sumowanie wartosci wybranych indeksow
*/
sprawdz_sume([],ListaWartosci,0):-
	abort.
sprawdz_sume([H|T],ListaWartosci,Suma):-
	(length(T,0),
	nth0(H,ListaWartosci,Glowa),
	Suma is Glowa
	;
	sprawdz_sume(T,ListaWartosci,SumaOgona),
	nth0(H,ListaWartosci,Glowa),
	Suma is SumaOgona+Glowa).
	
	
/*
	Funkcja filtrująca indeksy, dla których wartość 2giej listy przyjmuje wartość dodatnią
*/
wybierz_niezerowe([],[],[]).
wybierz_niezerowe([H|T],[H1|T1],S) :-
	H=<0,
	wybierz_niezerowe(T,T1,S).
wybierz_niezerowe([H|T],[H1|T1],[H1|S]) :-
	H>0,
	wybierz_niezerowe(T,T1,S).	
	
	
wybierz_nieujemne([],[],[]).
wybierz_nieujemne([H|T],[H1|T1],S) :-
	H<0,
	wybierz_nieujemne(T,T1,S).
wybierz_nieujemne([H|T],[H1|T1],[H1|S]) :-
	H>=0,
	wybierz_nieujemne(T,T1,S).	

	
	
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
	inicjalizuj_liste(LK,ListaIndeksow),
	nth0(0,ListaIndeksow,_,SciezkaBezGlowy),
		
	MaxIndeks is IloscPol-1,
	numlist(0,MaxIndeks,ZakresIndeksow),
	
	wybierz_niezerowe(ListaWartosci,ZakresIndeksow,ListaGlow),
	list_to_domain(ListaGlow, ZestawGlow),
	nth0(0,ListaIndeksow,Glowa),
	Glowa in ZestawGlow,
	%member(Glowa, ListaGlow),/*przyspiesza, ale zmienia wyjscie - nie znajduje wszystkich rozwiązań*/

	wybierz_nieujemne(ListaWartosci,ZakresIndeksow,ListaNieujemnych),
	list_to_domain(ListaNieujemnych,ZestawNieujemnych),
	ListaIndeksow ins ZestawNieujemnych,	
	
	all_different(SciezkaBezGlowy),
	
	sprawdz_sume(SciezkaBezGlowy,ListaWartosci,Suma),
	Suma#>=Dobrze,
	Suma#=<Najlepiej,
	
	czy_sasiednie(ListaIndeksow,Szerokosc).
	/*maplist(=<(0),ListaIndeksow)*/