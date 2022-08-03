# PublicAppAutoForex
## Spis
* [Opis](#opis)
* [Funkcjonalności](#funkcjonalności)
* [Wygląd](#wygląd)
* [Technologie](#technologie)

## Opis
Projekt został napisany w języku C# z wykorzystaniem technologii .NET (w szczególności WinForms).

Pracę rozpoczęto od zaznajomienia się z xAPI .NET Wrappers (dostępnego na stronie http://developers.xstore.pro/api/tutorials/first_steps_dotnet2)
Wykonana została aplikacja konsolowa obsługująca otwieranie i zamykanie pozycji walutowych.
Projekt został zaktualizowany do aplikacji okienkowej. 

Repozytorium zawiera przykładowe pliki projektu: okienek aplikacji oraz zarządzania bazą danych. 
Zaprogramowane metody do automatycznego zarządzania pozycjami oraz analiza danych historycznych nie została zapisana w tym repozytorium.  

## Funkcjonalności
Aplikacja umożliwia:
 - w trybie "streaming":
     - otwierać i zamykać pozycje,
     - wyświetlać zamknięte pozycje na wykresie, z zaznaczeniem punktu otwarcia i zamknięcia pozycji oraz profitu z niej,
     - przeglądać aktualny wykres jednej z 9 wybranych par walutowych.

 - w trybie "history":
     - pobrać dane historyczne z platformy XTB z całego dostępnego zakresu (jest to około miesiąca danych dla "minutowych świeczek"),
     - zapisać pobrane dane w bazie danych,
     - usunąć wybraną tablicę, 
     - pobrać dane zapisane w bazie danych wybierając jedną z listy pozycję oznaczoną datą utworzenia.
     - 
 - w obu trybach:
     - zmieniać granice wykresów,
     - zmieniać wartość wskaźników SMA, które wyświetlane są na wykresach.
 
 ## Wygląd
Po uruchomieniu aplikacji wyświetla się okienko logowania:

![Okno logowania](https://github.com/Mechadronik/PublicAppAutoForex/blob/main/Login.JPG?raw=true)

Użytkownik wpisuje to login i hasło do portalu XTB. Jeżeli wciśnięty jest klawisz CapsLock, wyświetla się odpowiednia informacja. 
W przypadku wpisania złych danych logowania również pojawi się czerwony napis o tym informujący, zaraz po nie udanej próbie zalogowania.

Po zalogowaniu wyświetli się główne okno aplikacji.	
Do wyboru są dwa tryby działania aplikacji: "streaming" oraz "history".
W pierwszym z nich możliwe jest min. otwieranie i zamykanie pozycji, śledzenie aktualnych zmian dla wybranych walut.

![Okno główne streaming](https://github.com/Mechadronik/PublicAppAutoForex/blob/main/ModeStreaming.JPG?raw=true)

W trybie danych historycznych możliwe jest zapisywanie danych do tabeli bazy danych oraz wyświetlanie z wybranego okresu zapisanych danych na wykresie. 

![Okno główne history](https://github.com/Mechadronik/PublicAppAutoForex/blob/main/ModeHistory.JPG?raw=true)

  
## Technologies
Projekt wykonano z wykorzystaniem:
* Visual Studio 2022 Community,
* Microsoft SQL Server Management Studio 18,
* języków C#, SQL,
* WinForms. 
