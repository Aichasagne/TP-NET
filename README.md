# TP .NET - ASP.NET Core APIs

## Projets
- **TodoApi** - API REST avec Entity Framework Core (In-Memory)
- **BooksApi** - API REST avec MongoDB + Authentification JWT

## URLs de production
- **TodoApi** : https://todoapi-p9om.onrender.com
- **BooksApi** : https://booksapi-v2.onrender.com

---

## Prérequis
- .NET 9 SDK
- Docker (optionnel)

---

## Lancer TodoApi localement
```bash
cd TodoApi
dotnet run --launch-profile https
```
Accéder à : http://localhost:5270/api/todoitems
Swagger : http://localhost:5270/swagger

---

## Lancer BooksApi localement
```bash
cd BooksApi
dotnet run --launch-profile https
```
Accéder à : http://localhost:5182/api/books

---

## Lancer avec Docker

### BooksApi
```bash
cd BooksApi
docker build -t booksapi .
docker run -p 8080:8080 \
  -e "BookStoreDatabase__ConnectionString=VOTRE_CONNECTION_STRING" \
  -e "JwtSettings__Secret=VOTRE_SECRET" \
  -e "JwtSettings__Issuer=BooksApi" \
  -e "JwtSettings__Audience=BooksApiUsers" \
  booksapi
```

### TodoApi
```bash
cd TodoApi
docker build -t todoapi .
docker run -p 8081:8080 todoapi
```

---

## Authentification JWT (BooksApi)

### Règles de sécurité
| Endpoint | Rôle requis |
|----------|-------------|
| GET /api/books | Public (aucun token) |
| GET /api/books/{id} | Authentifié (User ou Admin) |
| POST /api/books | Admin uniquement |
| PUT /api/books/{id} | Admin uniquement |
| DELETE /api/books/{id} | Admin uniquement |

---

### Étape 1 — Créer un compte Admin
```bash
curl -X POST https://booksapi-v2.onrender.com/api/auth/register-admin \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","email":"admin@test.com","password":"Admin123!"}'
```

### Étape 2 — Créer un compte User
```bash
curl -X POST https://booksapi-v2.onrender.com/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"user1","email":"user1@test.com","password":"User123!"}'
```

### Étape 3 — Se connecter et obtenir un token
```bash
curl -X POST https://booksapi-v2.onrender.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'
```
Réponse :
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiration": "2026-03-22T11:44:42Z",
  "role": "Admin"
}
```

### Étape 4 — Utiliser le token

#### Ajouter un livre (Admin seulement)
```bash
curl -X POST https://booksapi-v2.onrender.com/api/books \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer VOTRE_TOKEN" \
  -d '{"bookName":"Clean Code","price":29.99,"category":"Programming","author":"Robert Martin"}'
```

#### Lire les livres (public)
```bash
curl https://booksapi-v2.onrender.com/api/books
```

#### Tester le refus d'accès (User essaie de POST)
```bash
curl -X POST https://booksapi-v2.onrender.com/api/books \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN_USER" \
  -d '{"bookName":"Test","price":10,"category":"Test","author":"Test"}'
```
→ Réponse attendue : **403 Forbidden**

#### Sans token (essai de POST)
```bash
curl -X POST https://booksapi-v2.onrender.com/api/books \
  -H "Content-Type: application/json" \
  -d '{"bookName":"Test","price":10,"category":"Test","author":"Test"}'
```
→ Réponse attendue : **401 Unauthorized**

---

## Variables d'environnement (production)

| Variable | Description |
|----------|-------------|
| `BookStoreDatabase__ConnectionString` | Connection string MongoDB Atlas |
| `JwtSettings__Secret` | Clé secrète JWT |
| `JwtSettings__Issuer` | Issuer JWT (BooksApi) |
| `JwtSettings__Audience` | Audience JWT (BooksApiUsers) |