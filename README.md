# WorkTrack.App

##  WorkTrack.App — Role Based Task & Project Management System

**WorkTrack.App** is an ASP.NET Core MVC application with Identity authentication that provides **enterprise-style role-based workflow management** for three types of users:

- **Admin** — Manage users, roles, and projects  
- **Manager** — Manage projects, teams, and tasks  
- **Employee** — View assigned work and update task progress  

The application is designed following **modern software engineering practices**, including clean architecture, service layer separation, and secure authentication.

---

## Features

### Authentication & Authorization
- ASP.NET Identity Authentication
- Role-based access control
- Secure password hashing

### Role-Based Dashboards
- Admin Dashboard — User and project management  
- Manager Dashboard — Task creation and assignment  
- Employee Dashboard — Task tracking and updates  

### Task & Project Management
- Project creation and management  
- Task assignment and tracking  
- Status updates:
  - Pending
  - In Progress
  - Completed  

### Utility Pages
- Terms & Conditions  
- Support  
- Settings  
- Help  

### Dashboard Analytics
- Task summary cards  
- User-specific workspace views  

---

## Technologies Used

- ASP.NET Core MVC 10.0  
- ASP.NET Identity  
- Entity Framework Core (Code First)  
- SQL Server  
- Tailwind CSS  
- Razor Views  
- Dependency Injection Architecture  

---

## Prerequisites

- .NET SDK 10.0 (or matching project framework version)
- SQL Server Database

---

## Setup Instructions
### 1. Clone Repository
```bash
git clone https://github.com/Dineshprasadpant/project.git
cd project
```
### 2. Restore Dependencies
```bash
dotnet restore