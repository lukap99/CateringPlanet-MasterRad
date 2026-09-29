# Nacrt strukture master rada

**Radni naslov (prema prijavi teme):** Projektovanje i implementacija višeslojne veb aplikacije za ketering servis zasnovane na ASP.NET Core MVC arhitekturi i REST API servisu

**Student:** Luka Popović, 569/22
**Mentor:** dr Selver Pepić, prof. strukovnih studija
**Ustanova:** Visoka škola strukovnih studija za informacione tehnologije (ITS), Beograd

> Napomena: ovo je radni nacrt za prelazak u Word. Struktura po poglavljima prati odobrenu prijavu teme („Struktura – po poglavljima“ iz prijave) i zahteve iz *Uputstva o izradi završnog rada na master strukovnim studijama* (Beograd, oktobar 2024). Formalni delovi rada (korice, naslovna strana, izjava, sažeci) preuzeti su iz istog uputstva.

---

## 0. Formalni delovi rada (prema Uputstvu)

Redosled delova u finalnom dokumentu:

1. **Korice** — standardizovane, bez logoa (Prilog 3 Uputstva)
2. **Naslovna strana** — identična koricama, sa logoom VŠSSIT (Prilog 4 Uputstva)
3. **Naslov** — preporučljivo najviše dva reda
4. **Zahvalnica i/ili posveta** — opciono
5. **Rezime (sažetak) i ključne reči na srpskom jeziku** — najviše jedna strana; sadrži: ciljeve, predmet, metodologiju, rezultate, zaključke; do 5 ključnih reči
   - Predlog ključnih reči: *višeslojna arhitektura; ASP.NET Core MVC; REST API; Entity Framework Core; Bootstrap 5*
6. **Abstract i keywords na engleskom jeziku**
7. **Izjava o akademskoj čestitosti** (Prilog 2 Uputstva)
8. **Sadržaj** — automatski generisan u Wordu (References → Table of Contents)
9. **Uvod** → 10. **Poglavlja 1–4** → 11. **Zaključak** → 12. **Literatura** → 13. **Prilozi** → 14. **Radna biografija**

---

## UVOD (obim ~5% rada, maks. 10%)

Cilj uvodnog poglavlja: na jasan i koncizan način postaviti temu i okvir istraživanja (prema Uputstvu, uvod opisuje temu, istraživačke ciljeve i pitanja, predmet, metode, hipoteze i strukturu rada).

Predložene celine uvoda:

1. **Uvodna razmatranja**
   - Značaj digitalizacije poslovanja u ugostiteljstvu i ketering industriji; rast onlajn naručivanja hrane.
   - Problem: tradicionalni način rada ketering servisa (telefon, papir, Excel) i potreba za pouzdanim, centralizovanim informacionim sistemom.

2. **Predmet i problem rada**
   - Predmet: projektovanje i implementacija višeslojnog informacionog sistema za onlajn ketering servis na .NET platformi.
   - Problem istraživanja: kako pravilno struktuirati veb aplikaciju (razdvajanje odgovornosti, *separation of concerns*) da bude održiva, proširiva i pogodna za timski rad, a istovremeno dostupna i drugim klijentima kroz REST API.

3. **Ciljevi rada** (preuzeti i proširiti iz prijave teme)
   - Primarni cilj: projektovanje i implementacija veb aplikacije za podršku poslovanju ketering servisa primenom ASP.NET Core MVC obrasca i višeslojne arhitekture.
   - Posebni ciljevi:
     - primena MVC (*Model–View–Controller*) obrasca radi razdvajanja podataka, poslovne logike i prezentacionog sloja;
     - implementacija REST API servisa kao komponente za standardizovanu razmenu podataka i integraciju sa drugim klijentima (veb, mobilni);
     - razvoj funkcionalno kompletnog sistema: upravljanje korisnicima, menijem i narudžbinama;
     - primena savremenih mehanizama autentifikacije, autorizacije i zaštite podataka;
     - kreiranje rešenja koje je osnova za dalju nadogradnju.

4. **Istraživačka pitanja** (predlog, 4–5 pitanja)
   - P1: Kako višeslojna arhitektura i MVC obrazac obezbeđuju jasno razdvajanje odgovornosti između prezentacije, poslovne logike i pristupa podacima?
   - P2: Na koji način REST API servis povećava fleksibilnost sistema i omogućava integraciju sa različitim klijentskim aplikacijama?
   - P3: Kako ASP.NET Core Identity, JWT autentifikacija i *role-based* autorizacija obezbeđuju sigurnost sistema?
   - P4: Kako Bootstrap 5 i Razor views doprinose izradi responzivnog i konzistentnog korisničkog interfejsa?
   - P5: Da li ovakva struktura povećava održivost koda i olakšava paralelan (timski) razvoj?

5. **Hipoteze** (predlog)
   - H1: Primena višeslojne arhitekture smanjuje međuzavisnost (*coupling*) slojeva i povećava održivost sistema.
   - H2: Izdvajanje REST API servisa omogućava nezavisan razvoj više klijentskih aplikacija nad istom poslovnom logikom.
   - H3: Implementirani sistem zadovoljava definisane funkcionalne zahteve, što se potvrđuje testiranjem.

6. **Metode rada** (preuzeti iz prijave teme)
   - Analiza problema i definisanje funkcionalnih i nefunkcionalnih zahteva.
   - Projektovanje strukture aplikacije (višeslojna arhitektura, model podataka).
   - Implementacija u C# uz ASP.NET Core, Entity Framework Core i SQL Server.
   - Testiranje osnovnih funkcionalnosti i analiza rezultata.

7. **Struktura rada po poglavljima** — kratak opis sadržaja svakog poglavlja (jedan pasus po poglavlju).

---

## 1. POGLAVLJE — ANALIZA ZAHTEVA I OPIS SISTEMA

(Prema prijavi: „analiza poslovnog domene, korisničkih uloga i funkcionalnih zahteva sistema“.)

### 1.1 Analiza poslovnog domena
- Opis ketering servisa: ponuda jela i *combo* menija, dostava, naplata, program lojalnosti.
- Analiza poslovnih procesa: pregled ponude → poručivanje → potvrda → priprema → dostava; administracija menija i korisnika.
- Analiza postojećeg stanja — nasleđeni sistem (aplikacija CateringAgency) i razlozi za prelazak na novu, modernizovanu platformu (predmet rada: „tehnički unapređen, pouzdan i proširiv informacioni sistem“).

### 1.2 Korisničke uloge sistema
Tabelarni prikaz uloga i prava pristupa:

| Uloga (*role*) | Opis | Tipične funkcionalnosti |
|---|---|---|
| *Guest* (gost) | neautentifikovan korisnik | pregled menija, kategorija i *combo* ponude; registracija/prijava |
| *Customer* (kupac) | registrovani korisnik | korpa (session-based cart), poručivanje, pregled svojih narudžbina, loyalty points |
| *Manager* (menadžer) | zaposleni ketering servisa | upravljanje jelima, kategorijama, *combo* menijima; obrada narudžbina (promena statusa); API administracija narudžbina |
| *Admin* (administrator) | administrator sistema | sve menadžerske funkcije + upravljanje korisnicima i ulogama |

### 1.3 Funkcionalni zahtevi
- Modul korisnika: registracija, prijava/odjava, profil, ASP.NET Core Identity.
- Modul menija: kategorije, jela (slika, cena, opis), *combo* meniji.
- Modul naručivanja: korpa, *checkout*, izbor načina dostave i plaćanja, statusi narudžbine.
- Modul lojalnosti: sakupljanje poena (*loyalty points*), popusti na narudžbine.
- Modul administracije (*Admin area*): CRUD jela/kategorija/*combo* menija, pregled i promena statusa narudžbina, upravljanje korisnicima.
- REST API funkcionalnosti: autentifikacija, pregled menija, kreiranje i pregled narudžbina, administracija narudžbina, podaci o nalogu.

### 1.4 Nefunkcionalni zahtevi
- Bezbednost: autentifikacija, autorizacija po ulogama, zaštita od CSRF (*anti-forgery*), JWT za API.
- Održivost i proširivost: modularna, višeslojna struktura koda.
- Upotrebljivost: responzivni interfejs (Bootstrap 5), rad na desktop i mobilnim uređajima.
- Performanse i pouzdanost: efikasan pristup podacima kroz EF Core, jasno upravljanje greškama.
- Dokumentovanost API-ja (Swagger/OpenAPI).

### 1.5 Prikaz zahteva
- *Use case* dijagram sistema po ulogama.
- Tablice/opisi glavnih *use case*-eva (npr. „Pregled menija“, „Kreiranje narudžbine“, „Promena statusa narudžbine“).
- (Opciono) aktivacioni dijagram toka narudžbine — pogodan za kasniju diskusiju.

---

## 2. POGLAVLJE — PROJEKTOVANJE SISTEMA

(Prema prijavi: „struktura aplikacije, model podataka i primena MVC šablona i REST API servisa u projektovanju sistema“.)

### 2.1 Tehnološki okvir i izbor tehnologija
Kratak pregled i obrazloženje izbora svake tehnologije:

- **.NET 8 / ASP.NET Core** — moderna, *open-source*, *cross-platform* platforma; performanse, modularni *middleware pipeline*, ugrađeni *dependency injection* (DI) kontejner.
- **MVC obrazac (Model–View–Controller)** — razdvajanje podataka, poslovne logike i prezentacije.
- **Razor view engine** — serversko renderovanje HTML-a, *tag helpers*, *layouts*, *partial views*.
  - Napomena za preciznost termina: projekat koristi **Razor views u okviru ASP.NET Core MVC**, ne *Razor Pages* (to je poseban UI obrazac); u radu koristiti termin *Razor views*.
- **Entity Framework Core (Code-First)** — ORM; migracije kao jedan izvor istine za šemu baze.
- **SQL Server** — relaciona baza podataka.
- **ASP.NET Core Identity** — autentifikacija, upravljanje korisnicima i ulogama.
- **Bootstrap 5** — CSS okvir za responzivan i konzistentan interfejs (grid sistem, komponente, forme).
- **REST API + JSON, Swagger/OpenAPI** — standardizovana razmena podataka i dokumentovan API.
- **JWT (JSON Web Token)** — autentifikacija API korisnika bez stanja na serveru (*stateless*).
- **xUnit** — jedinično testiranje poslovne logike.

### 2.2 Višeslojna arhitektura i razdvajanje odgovornosti (separation of concerns)
Ovo je centralni deo rada — detaljnije ga razraditi:

- Opšti koncept višeslojne arhitekture (*layered / N-tier architecture*) i *Clean Architecture* principa: slojevi zavise samo od slojeva ispod sebe; poslovna logika ne zavisi od infrastrukture.
- Prikaz slojeva sistema (dijagram):

  - **CateringPlanet.Domain** — *domain layer*: entiteti, enumeracije, interfejsi; bez referenci na druge projekte (čisto jezgro sistema).
  - **CateringPlanet.Application** — *application layer*: DTO objekti, servisi i *use cases*; referiše samo Domain.
  - **CateringPlanet.Infrastructure** — *infrastructure layer*: EF Core `DbContext`, konfiguracije, migracije, *seeding*, servisi za čuvanje slika; referiše Application.
  - **CateringPlanet.Web** — *presentation layer* (ASP.NET Core MVC aplikacija); referiše Application i Infrastructure.
  - **CateringPlanet.Api** — *presentation layer* (REST API + Swagger); referiše Application i Infrastructure.
  - (Opciono) **CateringPlanet.Tests** — xUnit test projekat.

- **Pravilo zavisnosti (dependency rule)** — grafički prikaz: Application → Domain; Infrastructure → Application; Web/Api → Application + Infrastructure; Domain ne referiše ništa.
- **Prednosti za timski rad i održivost** (povezati sa istraživačkim pitanjima P1 i P5):
  - nezavisan razvoj slojeva (paralelan rad članova tima);
  - lakše testiranje (poslovna logika bez zavisnosti od baze i UI-ja);
  - zamenljivost infrastrukture (npr. promena baze ili servisa za slike bez diranja poslovne logike);
  - jasna organizacija koda i lakše uvođenje novih članova tima;
  - dve nezavisne prezentacije (MVC web aplikacija i REST API) nad istom poslovnom logikom — konkretan dokaz vrednosti slojevite strukture.
- **Dependency Injection** — registracija servisa u `Program.cs`, vezivanje interfejsa i implementacija kao mehanizam koji slojeve drži „labavo povezanim“ (*loose coupling*).

### 2.3 Model podataka
- Opis entiteta (iz Domain sloja): `User`, `Food`, `FoodCategory`, `ComboMenu`, `ComboMenuItem`, `Order`, `OrderFoodItem`, `OrderComboItem`, `DeliveryMethod`, `PaymentMethod`, `OrderDiscount`, `PointTransaction`, uz `BaseEntity` (Id, CreatedAt, ModifiedAt).
- Enumeracije: `OrderStatus`, `PaymentStatus`, `UserRole`.
- **ER dijagram** baze podataka (izraditi u alatu po izboru, npr. draw.io / Visio).
- Veza sa ASP.NET Core Identity: `ApplicationUser : IdentityUser` (proširenje Identity korisnika poslovnim podacima, npr. ime, prezime, poeni).
- Mapiranje entiteta: *Fluent API* konfiguracije (`Configurations/`) i `HasData()` za šifarnike.

### 2.4 Projektovanje MVC prezentacionog sloja (CateringPlanet.Web)
- Struktura: `Controllers/`, `Views/`, `ViewModels/`, `ViewComponents/`, `Models/`, `wwwroot/`.
- Kontroleri po modulima: `HomeController`, `MenuController`, `AccountController`, `CheckoutController`, `OrderController`; *Admin area*: `AdminMenuController`, `AdminOrderController`, `AdminUserController`.
- *ViewModels* umesto direktnog prosleđivanja entiteta pogledima (dodatno razdvajanje odgovornosti na prezentacionom sloju).
- *ViewComponents* i *partial views* za delove koji se ponavljaju (npr. korpa, navigacija po ulogama).
- *Layout* stranica, sekcije, *tag helpers*, validacija obrazaca (*DataAnnotations* + *client-side validation*).

### 2.5 Projektovanje REST API sloja (CateringPlanet.Api)
- Resursi i *endpoint*-ovi (tabela predlog):

| Metod | Endpoint | Pristup | Opis |
|---|---|---|---|
| POST | `/api/auth/login` | anonimno | prijava, izdavanje JWT tokena |
| GET | `/api/menu/categories` | anonimno | kategorije |
| GET | `/api/menu/foods` | anonimno | jela |
| GET | `/api/menu/combos` | anonimno | *combo* meniji |
| GET/POST | `/api/orders` | autorizovano | lista sopstvenih narudžbina / kreiranje narudžbine |
| GET | `/api/orders/{id}` | autorizovano | detalji sopstvene narudžbine |
| GET | `/api/admin/orders` | Manager/Admin | sve narudžbine |
| PATCH | `/api/admin/orders/{id}/status` | Manager/Admin | promena statusa narudžbine |
| GET | `/api/account` | autorizovano | profil + poeni |

- DTO objekti na granici API-ja (`LoginRequestDto`, `LoginResponseDto`, `CreateOrderRequestDto`, `UpdateOrderStatusRequestDto`) — zaštita da interni tipovi (Identity, domain entiteti) ne „iscure“ kroz API.
- JWT Bearer autentifikacija i zaštita *endpoint*-ova atributom `[Authorize]` / `[Authorize(Roles = ...)]`.
- Swagger/OpenAPI dokumentacija, CORS podešavanja.

### 2.6 Projektovanje bezbednosti
- ASP.NET Core Identity: registracija, prijava, *password hashing*, cookie autentifikacija za web deo.
- *Role-based* autorizacija i navigacija prilagođena ulogama.
- Zaštita formi od CSRF napada, validacija unosa, bezbedan rad sa otpremljenim slikama (ograničenje veličine, čitljiva imena fajlova — *slug*).

---

## 3. POGLAVLJE — IMPLEMENTACIJA REŠENJA

(Prema prijavi: „realizacija sistema korišćenjem ASP.NET Core tehnologije, Entity Framework Core alata i pratećih veb tehnologija“.)

### 3.1 Organizacija rešenja
- Struktura Visual Studio *solution*-a i *project references* (grafički prikaz + opis svakog projekta).

### 3.2 Implementacija Domain sloja
- Entiteti i enumeracije (kratki listinzi sa objašnjenjima, npr. `Food`, `Order`, `OrderStatus`).
- `BaseEntity` i zajednički članovi.
- Interfejsi repozitorijuma (`IRepository`, `IFoodRepository`, `IOrderRepository`) kao ugovori koje ispunjava Infrastructure sloj.

### 3.3 Implementacija Application sloja
- DTO klase kao granični tipovi podataka.
- Servisi i poslovna pravila:
  - `MenuService` — pregled kategorija, jela, *combo* menija;
  - `OrderService` — kreiranje narudžbine, pregled, promena statusa;
  - `AccountService` — podaci o nalogu, poeni;
  - `PricingService` — pravila cena: popusti na narudžbinu, cena dostave, obračun loyalty poena (decimalna aritmetika).
- Primer koda: računanje ukupne cene narudžbine sa popustom i dostavom.

### 3.4 Implementacija Infrastructure sloja
- `CateringPlanetDbContext : IdentityDbContext<ApplicationUser>`.
- Code-First pristup i migracije (postupak kreiranja i primene migracija).
- *Fluent API* konfiguracije entiteta i `HasData()` šifarnici.
- *Seeding* podataka — `DbInitializer` (demo podaci: jela, *combo* meniji, korisnici, narudžbine).
- Rad sa slikama: `IImageStorageService` interfejs + `LocalImageStorageService` implementacija (bezbedno čuvanje otpremljenih slika, *slug* imena fajlova, rezervna slika).
- Repozitorijumi kao EF Core implementacije interfejsa iz Domain sloja.

### 3.5 Implementacija MVC veb aplikacije (CateringPlanet.Web)
- `Program.cs` — registracija servisa (DI), konfiguracija Identity-ja, *middleware pipeline*, rutiranje, seed poziv.
- Kontroleri po modulu — primer `MenuController` (pregled menija) i `OrderController` (tok naručivanja).
- Korpa zasnovana na sesiji (*session-based cart*).
- *Admin area* — administracija jela, kategorija, *combo* menija (uključujući otpremanje slika), narudžbina i korisnika.
- Razor views:
  - *Layout* i organizacija pogleda po kontrolerima;
  - *partial views* i *ViewComponents* (npr. korpa, navigacija);
  - *tag helpers*, validacija obrazaca, prikaz grešaka;
  - dinamičko prikazivanje UI elemenata prema ulozi korisnika.
- Primer ključnog listinga sa objašnjenjem (npr. forma za naručivanje ili prikaz statusa narudžbine).

### 3.6 Implementacija REST API aplikacije (CateringPlanet.Api)
- `Program.cs` — registracija JWT autentifikacije, Swagger-a, CORS-a.
- `AuthController` — izdavanje JWT tokena (401 za pogrešne kredencijale).
- `MenuController`, `OrdersController` (samo sopstvene narudžbine), `AdminOrdersController` (Manager/Admin), `AccountController`.
- DTO objekti i mapiranje.
- Primer: poziv API-ja kroz Swagger UI ili `CateringPlanet.Api.http` fajl.

### 3.7 Korisnički interfejs aplikacije
- Prikaz ekrana aplikacije (screenshots): početna strana, pregled menija, detalji jela, korpa, *checkout*, profil, lista narudžbina, admin panel.
- Obrazloženje primene Bootstrap 5: grid sistem, komponente (kartice, forme, tabele, bedževi za statuse), responzivni dizajn na mobilnim uređajima, konzistentan vizuelni identitet kroz *layout*.
- Povezivanje sa istraživačkim pitanjem P4: kako Bootstrap 5 + Razor views omogućavaju moderan i održiv interfejs.

### 3.8 Ključni delovi izvornog koda
- Izbor 5–8 najvažnijih listinaga sa detaljnim objašnjenjima (po ugledu na koleginicu iz PIR2 primera, pododeljak „Кључни делови изворног кода“).

---

## 4. POGLAVLJE — TESTIRANJE I ANALIZA REZULTATA

(Prema prijavi: „rezultati testiranja sistema, kao i analiza funkcionalnosti i ponašanja aplikacije“; Uputstvo zahteva posebno poglavlje posvećeno konkretnom istraživanju/studiji slučaja.)

### 4.1 Plan i metodologija testiranja
- Opis pristupa: funkcionalno (ručno) testiranje po modulima i ulogama + jedinično testiranje (xUnit) + testiranje API-ja kroz Swagger.
- Test podaci (demo korisnici po ulogama iz seed podataka).

### 4.2 Funkcionalno testiranje veb aplikacije
- Test scenariji po modulima (tabelarno: korak → očekivani rezultat → stvarni rezultat → status):
  - registracija i prijava; pristup po ulogama (Guest/Customer/Manager/Admin);
  - pregled menija i *combo* ponude;
  - dodavanje u korpu, izmena korpe, *checkout* i kreiranje narudžbine;
  - promena statusa narudžbine iz admin dela;
  - CRUD jela i kategorija, otpremanje slika;
  - obračun cene sa popustom/dostavom i dodela loyalty poena.

### 4.3 Testiranje REST API servisa
- Autentifikacija (uspešna/neuspešna prijava, JWT token).
- Pristup zaštićenim *endpoint*-ovima bez i sa tokenom (401/403/200).
- Kreiranje narudžbine kroz API i pregled statusa.
- Promena statusa narudžbine iz uloge Manager/Admin.

### 4.4 Jedinično testiranje (xUnit)
- Testovi poslovne logike (npr. `PricingService` — obračun cena i poena) bez zavisnosti od baze — demonstracija prednosti višeslojne arhitekture.

### 4.5 Analiza rezultata i diskusija
- Analiza ispunjenosti funkcionalnih i nefunkcionalnih zahteva.
- Diskusija istraživačkih pitanja i hipoteza (P1–P5, H1–H3):
  - potvrda koristi *separation of concerns*: izmene u jednom sloju ne zahtevaju izmene u drugim slojevima;
  - jedna poslovna logika, dva klijenta (MVC web + REST API) — potvrda fleksibilnosti;
  - opažanja o održivosti i pogodnosti za timski rad;
  - ograničenja trenutnog rešenja.
- Uporedna tabela zahteva i stepena ispunjenosti.

---

## ZAKLJUČAK (obim ~5% rada)

- Rezime ostvarenih ciljeva rada (povezati sa ciljevima iz uvoda i prijave teme).
- Odgovori na istraživačka pitanja i potvrda/opovrgavanje hipoteza (bez novih podataka i bez novih izvora).
- Praktični doprinos rada: tehnički unapređen, pouzdan i proširiv sistem za onlajn ketering usluge.
- Preporuke i pravci daljeg razvoja (predlozi):
  - mobilna aplikacija kao klijent postojećeg REST API-ja;
  - onlajn plaćanje (integracija *payment gateway*-ja);
  - notifikacije korisnika o statusu narudžbine (e-mail/SMS/push);
  - izveštavanje i analitika prodaje;
  - proširenje sistema za rad više ketering servisa (*multi-tenant*).

---

## LITERATURA

- Stil: IEEE ili APA, dosledno kroz ceo rad (prema Uputstvu).
- Polazna lista iz prijave teme (10+ izvora — obavezno je zadržati i dopuniti aktuelnim izvorima):
  - postojeća literatura iz prijave (C#, ASP.NET MVC, Bootstrap, HTML/CSS/JS, itd.);
- Preporučene dopune (aktuelni zvanični izvori, primeri):
  - Microsoft Learn — ASP.NET Core MVC / Razor / EF Core / Identity / JWT / Web API dokumentacija;
  - zvanična Bootstrap 5 dokumentacija (getbootstrap.com);
  - literatura o čistoj/višeslojnoj arhitekturi (npr. R. C. Martin — *Clean Architecture*);
  - Microsoft Learn — migracije EF Core, `DbContext` konfiguracija;
  - OpenAPI/Swagger specifikacija.
- Napomena iz Uputstva: svaka referenca sa spiska mora biti citirana u tekstu, i obrnuto; Wikipedia nije relevantna referenca.

---

## PRILOZI

1. **Popis korišćenih skraćenica** — tabela skraćenica sa punim nazivima (MVC, REST, API, JWT, EF, ORM, DI, DTO, CRUD, CSRF, SQL, HTML, CSS, JSON, CORS, itd.).
2. **Popis slika i grafika** — (automatski u Wordu) dijagrami arhitekture, *use case* dijagram, ER dijagram, screenshots ekrana.
3. **Popis tabela** — (automatski u Wordu) tabele uloga, zahteva, *endpoint*-ova, test scenarija.
4. **Ostali prilozi**:
   - Prilog A: Ključni listinzi izvornog koda (po potrebi);
   - Prilog B: Uputstvo za instalaciju i pokretanje aplikacije (prerequisiti: .NET 8 SDK, SQL Server; migracije; seed podaci; pokretanje Web + Api projekata);
   - Prilog C: Test scenariji (kompletan skup tabela testiranja).

---

## RADNA BIOGRAFIJA

- Na osnovu biografije iz prijave teme (rođen 18. 9. 1999. u Jagodini; Srednja elektrotehnička i građevinska škola „Nikola Tesla“ u Jagodini; osnovne strukovne studije IT na ITS-u od 2018; master strukovne studije IT na ITS-u od 2023; stručna praksa u firmi Informatika A.D.).

---

## TEHNIČKE NAPOMENE ZA FINALNU OBRADU (iz Uputstva)

- Font: Verdana, Regular, veličina 10; naslovi: Verdana Bold 12 (velika slova); podnaslovi: Verdana Bold 10 (velika slova).
- Margine: 2 cm (leva 2,5 cm); poravnanje: *justify*; prored: *single*; orijentacija: *portrait* (izuzetno *landscape* za široke tabele/slike).
- Svako poglavlje počinje na novoj strani.
- Header: naziv rada, ime i prezime studenta, broj indeksa; Footer: broj strane / ukupan broj strana.
- Slike: preporučena rezolucija 300 dpi.
- Obim: uvod ~5% (maks. 10%), srednji deo ~50 strana, zaključak ~5%.
- Nazivi fajlova bez naših slova (npr. `keteringServisPopovic.doc/.pdf/.ppt`).
- Citiranje: IEEE ili APA, dosledno; popis literature na kraju rada.

---

## MAPIRANJE NA PRIJAVU TEME (kontrolna tačka)

| Struktura iz prijave teme | Poglavlje u ovom nacrtu |
|---|---|
| Uvod | Uvod (ciljevi, predmet, metode, pitanja, hipoteze) |
| Analiza zahteva i opis sistema | Poglavlje 1 |
| Projektovanje sistema | Poglavlje 2 |
| Implementacija rešenja | Poglavlje 3 |
| Testiranje i analiza rezultata | Poglavlje 4 |
| Zaključak | Zaključak |
| Literatura | Literatura |
| Radna biografija | Radna biografija |
