# Supper_Sapper_WPF
##  | Sapper Game | WPF Application 

Projekt utworzony jako projekt semestralny z zajęć programowania obiektowego w C_Sharp

# Założenia Projektu:

## Cele 
* zapoznanie się z głównymi metodami opracowania graficznego interfejsu użytkownika (XAML) oraz oprogramowania go,
* wykształcenie umiejętności separowania kodu aplikacji od opisu jej wyglądu,
* współpraca w zespole, podział zadań,
* praca z systemem kontroli wersji (git, GitHub),
* wykształcenie umiejętności tworzenia testów jednostkowych i świadomości konieczności ich opracowania
* nabycie umiejętności właściwego dokumentowania kodu (dokumentacja XML)
* nabycie umiejętności tworzenia dokumentacji API (logiki aplikacji)
* tworzenie dokumentacji projektu na GitHub

## Aspekty techniczne opracowanej aplikacji
* aplikacja desktopowa WPF (klasyczna, UWP, fluent UI)
* do opracowania interfejsu graficznego wykorzystany XAML
* interfejs graficzny aplikacji musi być responsywny, wykorzystanie `async`/`await`
* aplikacja musi mieć wyraźnie rozdzieloną część logiki od części interfejsu
* cześć logiki aplikacji musi być przetestowana (testy jednostkowe dla publicznych funkcjonalności)
* _solution_ składa się co najmniej z:
  - projektu typu _Class Library_ zawierającego logikę aplikacji
  - projektu będącego właściwą aplikacją z interfejsem graficznym
  - projektu z testami jednostkowymi dla _Class Library_
* projekt typu _Class Library_ powinien być udokumentowany (np. automatycznie wygenerowana dokumentacja html w DocFx)
* zalecana lokalizacja aplikacji (wersja polska i angielska)
* zalecane wykorzystanie _Resources_ (elementy graficzne, multimedialne aplikacji)
* aplikacja w wersji ostatecznej powinna być dostarczona w formie instalatora
* ewentualna konfiguracja zainstalowanej aplikacji powinna odbywać się z poziomu zewnętrznych plików konfiguracyjnych lub wpisów w rejestrze

