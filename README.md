# Sistema de Información de Vuelos (SIV) ✈️

Este es el repositorio oficial del **Sistema de Información de Vuelos (SIV)**, una solución integral desarrollada bajo los principios de **Clean Architecture** (Arquitectura Limpia) para la gestión, seguimiento y notificación en tiempo real de operaciones aeroportuarias.

El sistema está dividido en múltiples aplicaciones y capas para garantizar separación de responsabilidades (SoC), escalabilidad y un estricto control de acceso basado en roles (RBAC).
<img width="1701" height="1219" alt="image" src="https://github.com/user-attachments/assets/a255e1ee-691b-472b-8f08-2eb4a0a2ed46" />

---

## 🏗 Arquitectura y Estructura del Proyecto

La documentación formal de requisitos, arquitectura, diagramas UML, matriz de cumplimiento y evidencias se encuentra en [`docs/`](docs/README.md).

El sistema está dividido en las siguientes capas (Clean Architecture):

- **Siv.Domain:** Entidades centrales del negocio (Vuelo, Puerta, Auditoría, Usuario, etc.) y las interfaces de los repositorios.
- **Siv.Application:** Lógica de negocio, casos de uso (Servicios), DTOs y validaciones.
- **Siv.Persistence:** Configuración de Entity Framework Core, DbContext, migraciones y repositorios concretos.
- **Siv.Api.Web:** API principal para exponer los datos a la aplicación Web. Contiene el Hub de SignalR (`NotificacionesHub`) para la comunicación en tiempo real.
- **Siv.Api.Desktop:** API secundaria diseñada exclusivamente para procesar las operaciones que vienen desde la aplicación de escritorio y comunicarse con la API Web.
- **Siv.Web:** Aplicación frontend (ASP.NET Core MVC) que simula el "Tablero del Aeropuerto", visible para los pasajeros y personal.
- **Siv.Desktop:** Aplicación frontend (WPF) para la administración operativa interna del aeropuerto.

---

## 🔐 Roles y Credenciales (RBAC)

El sistema soporta 3 roles con accesos y permisos distintos. Al ejecutar el proyecto por primera vez, la base de datos (LocalDB) se siembra automáticamente con estos usuarios de prueba:

| Rol | Usuario | Contraseña | Descripción |
| :--- | :--- | :--- | :--- |
| **Administrador** | `admin` | `Admin123!` | Acceso total al sistema, configuración de vuelos, usuarios y visibilidad. |
| **Operador** | `operador1` | `Operador123!` | Puede realizar **Cambios Operativos** (ej. cambiar puerta de embarque, actualizar estado del vuelo). |
| **Auditor** | `auditor1` | `Auditor123!` | Acceso de solo lectura al módulo de **Auditorías** para revisar el historial de cambios. |

---

## ✨ Características Principales

*   **Tablero de Vuelos en Tiempo Real:** Interfaz web que muestra los vuelos organizados, simulando las pantallas físicas de un aeropuerto.
*   **Comunicación Bidireccional (SignalR):** Cualquier cambio operativo realizado desde el Escritorio (ej. cambio de puerta) se refleja instantáneamente en la Web sin recargar la página.
*   **Auditoría Estricta:** Registro de "Valor Anterior" y "Valor Nuevo" por cada cambio operativo, asegurando la trazabilidad.
*   **Gestor de Puertas de Embarque:** Validación estricta para asignar vuelos únicamente a puertas existentes y disponibles.
*   **Inyección de Dependencias & SOLID:** Código altamente mantenible, escalable y preparado para pruebas unitarias.

---

## 🚀 Guía de Ejecución Local

### Prerrequisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (o superior).
- SQL Server LocalDB (incluido con Visual Studio).

### Pasos para iniciar el sistema

Debido a que el proyecto consta de múltiples aplicaciones, es necesario iniciarlas en el orden correcto para evitar conflictos de inicialización en la base de datos:

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/alfre09/Siv-alfre.git
   cd Siv-alfre
   ```

2. **Ejecutar la API Web (Inicializa la BD):**
   ```bash
   cd Siv.Api.Web
   dotnet run
   ```
   *(Espera a que diga "Now listening on..." para asegurar que la base de datos y los datos semilla se hayan creado).*

3. **Ejecutar las demás aplicaciones (En terminales separadas):**
   - **API Desktop:**
     ```bash
     cd Siv.Api.Desktop
     dotnet run
     ```
   - **Aplicación Web (Tablero):**
     ```bash
     cd Siv.Web
     dotnet run
     ```
   - **Aplicación WPF (Escritorio):**
     ```bash
     cd Siv.Desktop
     dotnet run
     ```

Una vez en ejecución:
- La Web estará disponible en: `http://localhost:5100`
- La App de Escritorio (WPF) abrirá su ventana nativa de Login.

---

## 🛠 Tecnologías Utilizadas

- **Backend:** C#, .NET 8, ASP.NET Core Web API
- **Frontend Web:** ASP.NET Core MVC, Razor Pages, Bootstrap, SignalR (JS)
- **Frontend Desktop:** WPF (Windows Presentation Foundation), MVVM
- **Base de Datos:** Entity Framework Core (Code-First), SQL Server (LocalDB)
- **Arquitectura:** Clean Architecture, Patrón Repositorio, Unit of Work

---
*Desarrollado como Proyecto Final del Sistema de Información de Vuelos.*
