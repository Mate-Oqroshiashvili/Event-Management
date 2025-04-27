# Event Management

# ASP.NET Core Web API Project

## 🚀 Project Overview

This project is a full-stack ASP.NET Core Web API that includes:

- **Entity Framework Core** with **SQL Server**
- **JWT Authentication**
- **Redis Caching** with Docker
- **Real-time WebSocket Communication** (SignalR)
- **SMTP Email Sending** (Gmail SMTP)
- **Twilio SMS Integration**

---

## 📋 Prerequisites

Make sure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Redis)
- [Redis Image](https://hub.docker.com/_/redis)
- A Gmail account (SMTP server for email sending)
- A Twilio account (for SMS functionality)

---

## 📦 Cloning and Setup

1. **Clone the repository**

```bash
git clone https://github.com/Mate-Oqroshiashvili/Event-Management.git
cd Event-Management
```

2. **Restore NuGet packages**

```bash
dotnet restore
```

3. **Set up `appsettings.json`**

- simply remove `Example` extention and use it as `appsetting.json` file.
  
Or

- Copy `appsettings.example.json` to `appsettings.json`:

```bash
cp appsettingsExample.json appsettings.json
```

- Fill in the required fields in your new `appsettings.json`:
  - SQL Server Connection String
  - Redis configuration (e.g., `localhost:5002`)
  - JWT Key, Issuer, and Audience
  - Twilio API credentials
  - SMTP (Gmail) credentials

4. **Database Setup**

- Ensure your SQL Server is running.
- Apply database migrations:

```bash
dotnet ef database update
```

> If no migrations exist yet:
> 
> ```bash
> dotnet ef migrations add InitialCreate
> ```

5. **Run Redis using Docker**

Pull and start a Redis container:

```bash
docker pull redis
docker run --name CONTAINER_NAME -p 5002:6379 -d redis
docker start CONTAINER_NAME
docker exec -it CONTAINER_NAME redis-cli
```

6. **Run the Web API**

```bash
dotnet run
```

- The API will run at:
  - `https://localhost:7056` - HTTPS
  - `http://localhost:5056` - HTTP
- Access Swagger UI at:
  - `https://localhost:7056/swagger/index.html` - HTTPS
  - `http://localhost:5056/swagger/index.html` - HTTP

---

## ⚙️ Key Features

- **Entity Framework Core** — SQL Server database access via `DefaultConnection`.
- **Redis Caching** — Fast in-memory caching with Redis (configured via Docker).
- **WebSockets / SignalR** — Real-time features such as notifications or live updates.
- **SMTP Email Service** — Sending emails using Gmail’s SMTP server.
- **Twilio SMS Integration** — Sending SMS notifications via Twilio.
- **JWT Authentication** — Secure endpoints with JSON Web Tokens.

---

## 🔑 Environment Variables (appsettings.json)

| Key | Description |
|:----|:------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `ConnectionStrings:Redis` | Redis connection string (e.g., `localhost:6379`) |
| `Jwt:Key` | Secret key for signing JWT tokens |
| `Jwt:Audience` | Expected audience for JWT tokens |
| `Jwt:Issuer` | Token issuer |
| `Twilio:AccountSID` | Twilio account SID |
| `Twilio:AuthToken` | Twilio auth token |
| `Twilio:FromPhoneNumber` | Twilio verified sender number |
| `EmailSettings:SmtpServer` | SMTP server address (`smtp.gmail.com`) |
| `EmailSettings:Port` | SMTP server port (587 for Gmail) |
| `EmailSettings:UserName` | SMTP email username (Gmail address) |
| `EmailSettings:Password` | SMTP email password or app password |
| `EmailSettings:From` | Sender's email address |

---

## 🛠 Notes

- **Redis**:
  - Ensure Redis is running before launching the project.
- **CORS**:
  - Make sure CORS settings allow connections from your frontend if applicable.

---

## ✅ Done!

You are now ready to build, run, and deploy this ASP.NET Core Web API project!

---

# 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.
