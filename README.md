# 🍴 UTB.Minute - Systém pro správu objednávek jídel

Moderní backendové řešení pro univerzitní menzu postavené na technologii **.NET 10** a orchestrované pomocí **.NET Aspire**. Projekt demonstruje integraci mikroslužeb, automatizované testování a kontejnerizaci databáze.

---

## 🏗️ Architektura systému

Projekt je rozdělen do několika logických celků pro zajištění čistoty kódu a škálovatelnosti:

* **`UTB.Minute.AppHost`**: Centrální orchestrátor (Aspire), který spravuje životní cyklus všech služeb a SQL Serveru v Dockeru.
* **`UTB.Minute.WebApi`**: Jádro systému. Obsahuje logiku pro správu jídel, denních menu a zpracování objednávek.
    * *Klíčová vlastnost:* Implementován mechanismus atomického snižování počtu dostupných porcí při každé objednávce.
* **`UTB.Minute.DbManager`**: Servisní nástroj pro vývojáře. Nabízí endpoint `/db/reset`, který zajistí čistý start databáze.
* **`UTB.Minute.Db`**: Datová vrstva obsahující Entity Framework Core DbContext a definice entit.
* **`UTB.Minute.Contracts`**: Sdílené DTO (Data Transfer Objects) zajišťující typovou bezpečnost při komunikaci.

---

## 🧪 Automatizované testování

Součástí řešení je komplexní integrační test **`Complete_Flow_Test`**, který simuluje reálný scénář používání aplikace:
1.  **Spin-up**: Automatické nastartování SQL kontejneru a všech služeb.
2.  **Initialization**: Reset databáze a naplnění seed daty (Svíčková, Guláš).
3.  **Validation**: Ověření dostupnosti menu přes WebApi.
4.  **Transaction**: Vytvoření objednávky studentem.
5.  **Final Check**: Verifikace, že systém správně odečetl porci z inventáře (Business logic validation).

> [!TIP]
> Testy byly odladěny pro běh v Docker prostředí s ohledem na latenci startu SQL Serveru (implementován delay pro stabilitu).

---

## 🚀 Jak začít

1.  **Předpoklady**: Nainstalovaný **Docker Desktop** a **.NET 10 SDK**.
2.  **Otevření**: Otevřete soubor `UTB.Minute.sln` ve Visual Studiu 2022 (v17.12+).
3.  **Spuštění**: Nastavte projekt `UTB.Minute.AppHost` jako startovací a stiskněte **F5**.
4.  **Dashboard**: Po spuštění se otevře Aspire Dashboard, kde můžete sledovat logy všech služeb.

---

## 🛠️ Použité technologie
- **Framework:** .NET 10
- **Orchestrace:** .NET Aspire
- **ORM:** Entity Framework Core (SQL Server)
- **Testování:** xUnit, FluentAssertions, Aspire Testing Library