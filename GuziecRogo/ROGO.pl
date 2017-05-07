?- use_module(library(clpfd)).
/*
	zapewnienie odpowiedniej dlugosci listy
*/
dlugosc_listy([], 0).
dlugosc_listy([_|Ls], Length) :-
	Length #= Length0 + 1,
	dlugosc_listy(Ls, Length0).

/*
	sprawdzenie czy pola w liscie znajduja sie obok siebie - pole ponizej lub powyzej rozni sie o szerokosc
*/
czy_sasiednie([H1,H2],Szerokosc):-
	S1 is H2 - Szerokosc, S2 is H2 - 1, S3 is H2 + 1, S4 is H2 + Szerokosc,
	%write(H1+' = '+[S1, S2, S3, S4]), nl,
	member(H1, [S1, S2, S3, S4]),
	!.
czy_sasiednie([H1,H2|T],Szerokosc):-
	S1 is H2 - Szerokosc, S2 is H2 - 1, S3 is H2 + 1, S4 is H2 + Szerokosc,
	%write(H1+' = '+[S1, S2, S3, S4]), nl,
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
	
/*
	Funkcja pomocnicza, wywoływana przez GUI
*/	
szukaj(Szerokosc, Wysokosc, LiczbaKrokow, Dobrze, Najlepiej, ListaWartosci, ListaIndeksow):-
	IloscPol is Szerokosc*Wysokosc,
	LK is LiczbaKrokow+1,
	dlugosc_listy(ListaIndeksow,LK),
	Cos is IloscPol-1,
	numlist(0,Cos,ZakresIndeksow),
	wybierz_niezerowe(ListaWartosci,ZakresIndeksow,ListaGlow),
	szukaj_rozwiazania(Szerokosc, IloscPol, Dobrze, Najlepiej, LiczbaKrokow, ListaWartosci, ListaIndeksow, ListaGlow, Wyniki).
	
	
/*
	Funkcja znajdująca rozwiązania, która sprawdza pozostałe warunki
*/	
szukaj_rozwiazania(Szerokosc, IloscPol, Dobrze, Najlepiej, LiczbaKrokow, ListaWartosci, ListaIndeksow, ListaGlow, Wyniki):-
	ListaIndeksow ins 0..IloscPol,
	nth0(0,ListaIndeksow,Glowa),
	nth0(0,ListaIndeksow,_,SciezkaBezGlowy),
	
	/*element(0, ListaGlow, V),
	element(LiczbaKrokow, ListaGlow, V),*/
	
	all_distinct(SciezkaBezGlowy),
	sprawdz_sume(SciezkaBezGlowy,ListaWartosci,Suma),
	Suma#>=Dobrze,
	Suma#=<Najlepiej,
	czy_sasiednie(ListaIndeksow,Szerokosc),
	%maplist(=<(0),ListaIndeksow),
	last(ListaIndeksow,Ostatni),
	Glowa#=Ostatni.