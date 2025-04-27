# Event Management Frontend

🌐 **Angular 19 Application**

---

🚀 **Project Overview**  
This project is the front-end client for the Event Management system, built with Angular 19. It communicates with an ASP.NET Core Web API backend and provides a real-time, interactive user experience.

**Core Features:**

- Angular 19 and Angular CLI
- WebSocket real-time communication (SignalR)
- API integration with JWT authentication
- Redis caching via backend
- Docker support (for Redis)
- Responsive, mobile-friendly UI

---

📋 **Prerequisites**

Make sure you have the following installed:

- [Node.js](https://nodejs.org/) (v18+ recommended)
- [Angular CLI](https://angular.dev/tools/cli/setup-local) (v19.2.5)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Redis setup)
- Backend API running locally or remotely (see backend setup)

---

📦 **Cloning and Setup**

Clone the repository:

```bash
git clone https://github.com/Mate-Oqroshiashvili/Event-Management.git
git checkout frontend
cd Event-Management
```

Install dependencies:

```bash
npm install
```

Set up environment configuration:

- Navigate to `src/environments/`
- **Rename** the files:
  - `environment.example.ts` ➔ `environment.ts`
  - `environment.development.example.ts` ➔ `environment.development.ts`
- Fill in the required API URLs and SignalR Hub URLs inside both `environment.ts` and `environment.development.ts`:

| Key                        | Description                              |
| :------------------------- | :--------------------------------------- |
| `apiUrls.baseUrl`          | Base URL of your running Web API backend |
| `hubUrls.groupChatHubUrl`  | URL for SignalR Group Chat Hub           |
| `hubUrls.userStatusHubUrl` | URL for SignalR User Status Hub          |

Example:

```typescript
export const environment = {
  production: false,
  apiUrl: "https://localhost:7056/api/",
  hubUrl: "https://localhost:7056/",
};
```

---

⚙️ **Development Server**

Run the following command to start the development server:

```bash
ng serve
```

Access the application at:

- [http://localhost:4200/](http://localhost:4200/)

The app will automatically reload if you change any source files.

---

🏗️ **Building the Project**

Create a production build:

```bash
ng build
```

The build artifacts will be stored in the `dist/` directory.

For a specific environment (e.g., production):

```bash
ng build --configuration production
```

---

🧪 **Running Unit Tests**

Execute unit tests via [Karma](https://karma-runner.github.io/):

```bash
ng test
```

Tests will automatically run and watch for changes.

---

🚀 **Running End-to-End (E2E) Tests**

First, install an E2E testing package if needed (e.g., Cypress, Protractor).

Then run:

```bash
ng e2e
```

---

🛠 **Code Scaffolding**

Generate a new component, service, module, etc., using Angular CLI:

```bash
ng generate component component-name
ng generate service service-name
ng generate module module-name
```

---

📝 **Notes**

- **CORS**:  
  Ensure your backend API allows CORS requests from `http://localhost:4200` during development.

- **Authentication**:  
  The frontend expects a valid JWT token for protected routes and SignalR Hub connections.

- **SignalR Connections**:  
  Double-check that your Hub URLs are correct in your environment files for real-time communication features.

---

✅ **Done!**  
You are now ready to build, run, and extend the Event Management Frontend project!
