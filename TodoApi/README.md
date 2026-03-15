# TP .NET - ASP.NET Core APIs

## Projets
- **TodoApi** - API REST avec Entity Framework Core (In-Memory)
- **BooksApi** - API REST avec MongoDB

## Prérequis
- .NET 9 SDK
- Docker (optionnel)

## Lancer TodoApi
```bash
cd TodoApi
dotnet run --launch-profile https
```
Accéder à : http://localhost:5270/swagger

## Lancer BooksApi
```bash
cd BooksApi
dotnet run --launch-profile https
```
Accéder à : http://localhost:5182/swagger

## Lancer avec Docker

### BooksApi
```bash
cd BooksApi
docker build -t booksapi .
docker run -p 8080:8080 booksapi
```

### TodoApi
```bash
cd TodoApi
docker build -t todoapi .
docker run -p 8081:8080 todoapi
```