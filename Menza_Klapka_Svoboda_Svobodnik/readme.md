# 🍴 Objednávací systém v menze (UTB Minute)

Semestrální projekt do předmětu **Aplikační frameworky**.

## 👥 Členové týmu a poměr práce
| Jméno a příjmení | Role v týmu | Poměr práce |
|:---|:---|:---:|
| **Roman Klapka** - vedoucí | WebAPI & SSE | 1 |
| **Tomáš Svobodník** | Datový model & Backend | 1 |
| **Pavel Svoboda** | Blazor klient & UI | 1 |


---

## 🚀 Spuštění projektu

1. **Požadavky:** .NET 10 SDK, Docker Desktop nebo Podman (nutný pro běh databázového Serveru a Keycloaku v Aspire).
2. **Postup:**
   - Spusťe Docker Desktop nebo Podman.
   - Otevřete solution `UTB.Minute.slnx` ve Visual Studiu 2026 nebo JetBrains Rider.
   - Nastavte projekt `UTB.Minute.AppHost` jako **Start-up projekt**.
   - Spusťte projekt.
   - V prohlížeči se otevře **.NET Aspire Dashboard**, kde uvidíte stav všech služeb a odkazy na klientské aplikace.

---

## 📂 Struktura řešení

- `UTB.Minute.AppHost`: Aspire orchestrace.
- `UTB.Minute.Db`: Datové entity a `DbContext`.
- `UTB.Minute.DbManager`: Obsahuje endpoint pro **Http Command** (reset databáze a seedování).
- `UTB.Minute.Contracts`: Sdílená DTO, aby byla zajištěna typová bezpečnost mezi API a klienty.
- `UTB.Minute.WebApi`: Hlavní byznys logika, správa objednávek a SSE hub. **(SSE bude dokončeno pro finální odevzdání)**
- `UTB.Minute.AdminClient`: Aplikace pro vedení menzy (správa jídel a menu). **(Bude dokončeno pro finální odevzdání)**
- `UTB.Minute.CanteenClient.Cook`: Rozhraní pro zaměstnance menzy kteří vydávají jídla. **(Bude dokončeno pro finální odevzdání)**
- `UTB.Minute.CanteenClient.Student`: Rozhraní pro studenty kteří si jídlo objednávají. **(Bude dokončeno pro finální odevzdání)**

## 🛠️ Klíčová implementační rozhodnutí

### 1. Autorizace a Keycloak
**(Bude doplněno po implemtaci ve finálním odevzdání)**

### 2. Real-time notifikace (SSE)
**(Bude doplněno po implemtaci ve finálním odevzdání)**

### 3. Business pravidla
**(Bude doplněno po implemtaci ve finálním odevzdání)**

---

## 📝 Poznámky k odevzdání
* **Stav:** Projekt má funkční všechny požadované části pro Půlsemestrální odevzdání
* **Testování:** Unit testy v `UTB.Minute.WebAPI.Tests` pokrývají otestování všech implementovaných API endpointů pro půlsemestrální odevzdání
* **Problémy:** ---

---

## 🧪 Seznam implementovaných API endpointů

### Jídla (Meals)
* `GET /meals` - Seznam všech dostupných jídel.
* `POST /meals` - Vytvoření nového jídla.
* `GET /meals/{id}` - Detail konkrétního jídla.
* `PATCH /meals/{id}` - Úprava jídla a jeho (de)aktivace.

### Menu (MenuItems)
* `GET /menu` - Zobrazení aktuálního denního menu.
* `POST /menu` - Přidání jídla do menu pro konkrétní datum a počet porcí.
* `PATCH /menu/{id}` - Úprava dostupnosti nebo data v menu.
* `DELETE /menu/{id}` - Odstranění položky z menu.

### Objednávky (Orders)
* `GET /orders` - Přehled všech objednávek (pro kuchaře).
* `POST /orders` - Vytvoření nové objednávky na základě položky z menu.
* `PATCH /orders/{id}/status` - Změna stavu objednávky (Preparing -> ReadyToPickUp -> Delivered).

### Správa DB (DbManager)
* `POST /reset` - Kompletní promazání a reinicializace databáze.
* `POST /seed` - Naplnění databáze testovacími daty pro ukázku funkčnosti.