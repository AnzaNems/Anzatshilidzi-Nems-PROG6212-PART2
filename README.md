\# RaceDay API - PROG6212 Part 2



\*\*Student:\*\* Anzatshilidzi Nimrod Nemafukani

\*\*Student Number:\*\* ST10480934

\*\*Course:\*\* PROG6212 Programming 2B



\## System Description

The RaceDay API is a secure, RESTful backend built with ASP.NET Core 8 for managing road race events. It supports two distinct roles:

\- \*\*Organiser\*\*: Creates and manages events, defines categories, and captures race results.

\- \*\*Participant\*\*: Views events, enrols in specific categories, and views personal race results.



\## Security \& Part 1 Feedback Implementation

To address the Part 1 feedback regarding privilege escalation, this API implements a strict approval gate:



\- Separate \*\*Organiser\*\* and \*\*Participant\*\* entities are used (not a single generic profile table).

\- When a user registers and selects "Organiser", the backend creates the account but sets `IsApproved = false`.

\- If that Organiser attempts to log in, the API returns \*\*403 Forbidden\*\* until an administrator approves the account.

\- This prevents users from self-granting elevated privileges through the registration endpoint.



\## Tech Stack

\- ASP.NET Core 8 Web API

\- Entity Framework Core (Code-First)

\- SQL Server LocalDB (local) / SQLite In-Memory (tests)

\- JWT Authentication + Server-Side Session Management

\- xUnit Integration Testing

\- GitHub Actions CI/CD



\## Setup and Run Instructions

1\. Clone the repository.

2\. Open `RaceDay.API.sln` in Visual Studio 2022.

3\. Open \*\*Package Manager Console\*\* and set Default Project to `RaceDay.API`.

4\. Run `Update-Database` to create the SQL Server LocalDB tables.

5\. Press `F5` to run. Swagger UI opens automatically.



\## Testing

To run the tests locally:

1\. Open \*\*Test Explorer\*\* in Visual Studio and click \*\*Run All\*\*, OR

2\. Run `dotnet test` from the solution root.



\## CI/CD Evidence

The GitHub Actions workflow automatically restores, builds, and tests the application on every push.



!\[GitHub Actions Green Build](docs/ci-screenshot.png)



\## YouTube Video Demonstration

\[PASTE\_YOUR\_UNLISTED\_YOUTUBE\_LINK\_HERE]

