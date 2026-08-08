# SIV — Sistema de Información de Vuelos

Refactor completo del proyecto original bajo arquitectura **n-layer**, principios **SOLID**
y separación estricta de responsabilidades entre capas. Backend en español (dominio, casos
de uso, nombres de clases y variables), código en inglés donde aplica (nombres reservados de
frameworks, atributos, etc.). Sin comentarios en el código, como fue solicitado.

## Estado de esta entrega

Esta entrega incluye la solución completa **con el cliente web funcional**:

| Proyecto            | Estado      | Descripción                                                        |
|---------------------|-------------|---------------------------------------------------------------------|
| `Siv.Domain`        | ✅ Completo | Entidades con comportamiento, interfaces de repositorio, reglas.   |
| `Siv.Application`   | ✅ Completo | DTOs, servicios de aplicación, mapeadores, excepciones.            |
| `Siv.Persistence`   | ✅ Completo | DbContext, `EntityTypeConfiguration`, repositorios, migraciones.   |
| `Siv.Api.Desktop`   | ✅ Completo | API REST para el futuro cliente de escritorio (WPF).               |
| `Siv.Api.Web`       | ✅ Completo | API REST (BFF) para el cliente web, contratos reducidos.           |
| `Siv.Web`           | ✅ Completo | Cliente ASP.NET Core MVC que consume `Siv.Api.Web`.                |
| `Siv.Desktop`       | ⏳ Pendiente | Cliente WPF (.NET 9) que consumirá `Siv.Api.Desktop`. Se entrega en la siguiente iteración. |

## Arquitectura

```
Siv.Domain            → Entidades, reglas de negocio, interfaces de repositorio (sin dependencias)
Siv.Application        → Casos de uso, DTOs, servicios de aplicación (depende de Domain)
Siv.Persistence         → EF Core, DbContext, EntityTypeConfiguration, repositorios (depende de Domain)
Siv.Api.Desktop          → API REST para el cliente de escritorio (depende de Application y Persistence)
Siv.Api.Web               → API REST para el cliente web (depende de Application y Persistence)
Siv.Web                     → ASP.NET Core MVC, consume Siv.Api.Web vía HttpClient
Siv.Desktop (pendiente)      → WPF .NET 9, consumirá Siv.Api.Desktop vía HttpClient
```

Ambas APIs comparten la misma base de datos (`SivDb` por defecto) para que los cambios
operativos registrados desde cualquiera de los dos clientes se reflejen en el otro.

### Principios aplicados

- **SRP**: los servicios de aplicación orquestan; las reglas de negocio (transición de
  estados, validación de cambios operativos) viven en la entidad `Vuelo`, no en los servicios.
- **OCP/DIP**: `Siv.Application` y `Siv.Persistence` dependen de interfaces definidas en
  `Siv.Domain` (`IUnitOfWork`, `IVueloRepositorio`, etc.), nunca al revés.
- **ISP**: los DTOs de escritura están separados por operación (`CrearVueloDto`,
  `ActualizarVueloDto`, `RegistrarCambioPuertaDto`, etc.) en vez de un único modelo con
  campos opcionales según el caso.
- Los controladores de `Siv.Web` **nunca llaman HTTP directamente**: siempre pasan por un
  "servicio de consumo de API" (`IVueloApiServicio`, etc.), tal como exige el lineamiento de
  la capa de presentación.

## Requisitos previos

- .NET SDK 9.0 o superior
- SQL Server (LocalDB, Express o completo) — o ajustar la cadena de conexión a otro motor
- Visual Studio 2022 (17.11+) o `dotnet` CLI

> Este proyecto fue generado sin acceso a un entorno con SDK de .NET instalado, por lo que
> **no fue compilado ni verificado automáticamente**. Es muy probable que compile sin
> problemas dado que se siguió con rigor la sintaxis de .NET 9 / EF Core 9, pero al abrirlo
> revisa la lista de verificación al final de este documento.

## Cómo ejecutar

Para la aplicaciÃ³n web solo es necesario iniciar `Siv.Api.Web` y `Siv.Web`.
`Siv.Api.Desktop` y `Siv.Desktop` corresponden al flujo de escritorio independiente.

### 1. Restaurar paquetes

Abre `Siv.sln` en Visual Studio y espera a que restaure los paquetes NuGet automáticamente,
o desde la terminal, en la carpeta raíz:

```bash
dotnet restore Siv.sln
```

### 2. Configurar la cadena de conexión

Por defecto, ambas APIs apuntan a:

```
Server=(localdb)\mssqllocaldb;Database=SivDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

Ajusta `Siv.Api.Desktop/appsettings.json` y `Siv.Api.Web/appsettings.json` si usas otro
servidor de SQL Server.

### 3. Migraciones

Las migraciones ya están incluidas (`Siv.Persistence/Migraciones/CreacionInicial`). **Ambas
APIs aplican las migraciones automáticamente al iniciar** (`Database.MigrateAsync()` en
`Program.cs`), junto con el sembrado de datos iniciales (aerolíneas, aeropuertos y estados
de vuelo).

Si prefieres generarlas de nuevo desde cero (recomendado si haces cambios al modelo), desde
la carpeta raíz:

```bash
dotnet ef migrations add CreacionInicial --project Siv.Persistence --startup-project Siv.Api.Web
dotnet ef database update --project Siv.Persistence --startup-project Siv.Api.Web
```

### 4. Ejecutar los proyectos

Debes iniciar **la API primero, y luego el cliente que la consume**:

1. `Siv.Api.Web` → arranca en `https://localhost:7200` (Swagger disponible en `/swagger`)
2. `Siv.Web` → arranca en `https://localhost:7100`, consume la API en el puerto 7200

En Visual Studio, puedes configurar **"varios proyectos de inicio"** (clic derecho en la
solución → Propiedades → Varios proyectos de inicio) y marcar `Siv.Api.Web` y `Siv.Web`
como "Iniciar", para levantarlos juntos con F5.

`Siv.Desktop` usa `Siv.Api.Desktop` (puerto 7201). Para probar cambios operativos y notificaciones, inicia primero la API y luego el cliente de escritorio.

## Estructura de carpetas relevante

```
Siv.Web/
├── Controllers/          → VuelosController, HomeController
├── Interfaces/            → Contratos de los servicios de consumo de API
├── Servicios/               → Implementaciones HttpClient de esos contratos
├── Modelos/                   → ViewModels específicos del cliente web
├── Configuracion/               → Registro de HttpClientFactory tipado
├── Views/                          → Razor views (Home, Vuelos, Shared)
└── wwwroot/css/siv.css               → Sistema de diseño (paleta, componentes)
```

## Lista de verificación al abrir en Visual Studio

- [x] Restaurar paquetes NuGet (`dotnet restore` o automático al abrir la solución)
- [x] Solución configurada para .NET 9
- [x] Cadena de conexión documentada en ambas APIs
- [x] Solución con proyectos de dominio, aplicación, persistencia, APIs, web y escritorio
- [x] Migraciones y sembrado de datos incluidos
- [x] Cliente web MVC y cliente WPF incluidos

## Próxima entrega

`Siv.Desktop` (WPF, .NET 9) consumiendo `Siv.Api.Desktop`, siguiendo el mismo patrón de
separación (ViewModels, servicios de consumo de API, sin lógica de negocio en la capa de
presentación).
