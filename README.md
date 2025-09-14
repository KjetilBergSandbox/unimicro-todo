# Unimicro Todo App ⭐

A simple Todo application built for Unimicro's hiring process. Built with C# ASP.NET Core and React TypeScript.

## Tech Stack
- **Backend**: C# ASP.NET Core, Entity Framework Core
- **Frontend**: React, TypeScript
- **Database**: SQLite (for simplicity, can be replaced with any other database)
- **Containerization**: Docker, Docker Compose

## Features
-- Stump

## Hosting
The application is hosted on [Microsoft Azure](https://azure.microsoft.com/) and can be accessed [here](https://unimicro-todo-app.azurewebsites.net/). No effort will be made to keep the application online after the hiring process.

## Running Locally
The application can be run locally using Docker Compose:
```bash
docker-compose up --build
```

Alternatively, without Docker, you can run the backend and frontend separately:
```bash
cd backend
dotnet run
```
```bash
cd frontend
npm install
npm start
```
The backend will be available at `http://localhost:5000` and the frontend at `http://localhost:3000`.

## Configuration
-- Stump

## Notice: AI
Unimicro requested the disclosure of AI usage in the project.
Throughout the project, copilot was used to assist with code suggestions everywhere. All code is written by me, with input from copilot. Any accepted suggestions are reviewed and modified by me to ensure they meet the project requirements and my coding standards. Copilot is a useful tool for enhancing productivity and cleanliness, but if requested I would be fine without it.

## Notice: FluentAssertions
The backend uses FluentAssertions for unit testing, which is only free for non-commercial use.

## License
This project is licensed under the MIT License.