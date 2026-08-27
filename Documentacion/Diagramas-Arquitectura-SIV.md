# Diagramas de arquitectura del SIV

Documento basado en la solución actual de Siv.sln. Los diagramas están escritos en Mermaid para poder renderizarlos en GitHub, Markdown Preview, Obsidian o convertirlos posteriormente a PNG/PDF.

## 1. Diagrama general de arquitectura

La solución separa la aplicación web para clientes de la aplicación Desktop para los puestos operativos y administrativos. Ambas APIs comparten la capa de aplicación, dominio y persistencia.

~~~mermaid
flowchart LR
    ClienteWeb[Cliente registrado<br/>Navegador]
    OperadorDesktop[Operador / Admin / Auditor<br/>Aplicación WPF]
    Web[Siv.Web<br/>ASP.NET Core MVC]
    Desktop[Siv.Desktop<br/>WPF + MVVM]
    ApiWeb[Siv.Api.Web<br/>API de clientes]
    ApiDesktop[Siv.Api.Desktop<br/>API operativa]
    App[Siv.Application<br/>Servicios + DTOs + reglas de aplicación]
    Domain[Siv.Domain<br/>Entidades + reglas + contratos]
    Persistence[Siv.Persistence<br/>EF Core + repositorios + migraciones]
    Db[(SQL Server / LocalDB<br/>SivDb)]
    SignalR[SignalR<br/>Notificaciones en tiempo real]
    Smtp[Servidor SMTP<br/>Correo opcional]

    ClienteWeb --> Web
    Web -->|HTTP + JWT| ApiWeb
    OperadorDesktop --> Desktop
    Desktop -->|HTTP + JWT| ApiDesktop
    ApiWeb --> App
    ApiDesktop --> App
    App --> Domain
    App --> Persistence
    Persistence --> Db
    ApiWeb --> SignalR
    ApiDesktop -->|Notificación operativa| ApiWeb
    App -->|Correo de cambio operativo| Smtp
~~~

## 2. Diagrama de capas

~~~mermaid
flowchart TB
    subgraph Presentacion[Presentación]
        WebUI[Siv.Web<br/>MVC, Razor, cookies]
        DesktopUI[Siv.Desktop<br/>WPF, MVVM, XAML]
    end

    subgraph Entrada[Entrada / APIs]
        ApiWebLayer[Siv.Api.Web<br/>Controllers, JWT, SignalR]
        ApiDesktopLayer[Siv.Api.Desktop<br/>Controllers, JWT]
    end

    subgraph Aplicacion[Capa de aplicación]
        Services[Servicios de aplicación]
        DTOs[DTOs y mapeadores]
        Policies[Políticas de visibilidad<br/>y validaciones]
    end

    subgraph Dominio[Dominio]
        Entities[Entidades: Vuelo, Usuario,<br/>Reserva, Seguimiento, etc.]
        Contracts[Interfaces de repositorios]
        Rules[Reglas de negocio]
    end

    subgraph Infraestructura[Persistencia]
        EF[EF Core DbContext]
        Repositories[Repositorios + Unit of Work]
        Migrations[Migraciones + semilla]
    end

    WebUI --> ApiWebLayer
    DesktopUI --> ApiDesktopLayer
    ApiWebLayer --> Services
    ApiDesktopLayer --> Services
    Services --> DTOs
    Services --> Policies
    Services --> Entities
    Services --> Contracts
    Entities --> Rules
    Contracts --> Repositories
    Repositories --> EF
    EF --> Migrations
~~~

Regla de dependencia:

Siv.Web / Siv.Desktop → Siv.Api.Web / Siv.Api.Desktop → Siv.Application → Siv.Domain

Siv.Persistence implementa los contratos del dominio y proporciona acceso a la base de datos.

## 3. Vista lógica del sistema

~~~mermaid
flowchart LR
    Auth[Autenticación<br/>Login y registro]
    Flights[Consulta de vuelos<br/>públicos y operativos]
    Operations[Operación de vuelos<br/>horario, puerta, estado]
    Reservations[Reservas<br/>crear, listar y cancelar]
    Tracking[Seguimientos<br/>seguir / dejar de seguir]
    Notifications[Notificaciones<br/>base de datos + SignalR + correo]
    Audit[Auditoría<br/>lecturas y cambios]
    Catalogs[Catálogos<br/>aerolíneas, aeropuertos,<br/>estados y puertas]
    Visibility[Visibilidad<br/>Público, Interno, Restringido]

    Auth --> Reservations
    Auth --> Tracking
    Flights --> Reservations
    Flights --> Tracking
    Operations --> Notifications
    Operations --> Audit
    Operations --> Flights
    Tracking --> Notifications
    Reservations --> Audit
    Catalogs --> Flights
    Catalogs --> Operations
    Visibility --> Flights
    Visibility --> Audit
~~~

Roles y límites de acceso:

~~~mermaid
flowchart TB
    Admin[Admin<br/>configura catálogos y supervisa]
    Operator[Operador<br/>gestiona vuelos y cambios]
    Auditor[Auditor<br/>consulta auditoría]
    Client[UsuarioRegistrado<br/>consulta, reserva y sigue vuelos]
    Anonymous[Anónimo<br/>solo consulta vuelos públicos]

    DesktopAPI[API Desktop]
    WebAPI[API Web]

    Admin --> DesktopAPI
    Operator --> DesktopAPI
    Auditor --> DesktopAPI
    Client --> WebAPI
    Anonymous --> WebAPI

    DesktopAPI --> AdminFunctions[Catálogos, vuelos,<br/>cambios y auditoría]
    WebAPI --> ClientFunctions[Vuelos disponibles,<br/>reservas y seguimientos]
~~~

La Web no debe exponer el registro o modificación de vuelos; esas operaciones pertenecen a Desktop.

## 4. Vista física

~~~mermaid
flowchart LR
    subgraph EquipoCliente[Equipo del usuario]
        Browser[Navegador web]
        WPF[Ejecutable Siv.Desktop]
    end

    subgraph EquipoServidor[Equipo de ejecución local o servidor]
        WebProcess[Siv.Web<br/>HTTP 5100 / HTTPS 7100]
        ApiWebProcess[Siv.Api.Web<br/>HTTP 5200 / HTTPS 7200]
        ApiDesktopProcess[Siv.Api.Desktop<br/>HTTP 5201 / HTTPS 7201]
    end

    subgraph Datos[Persistencia]
        SQL[(SQL Server<br/>Base SivDb)]
    end

    subgraph ServiciosExternos[Servicios opcionales]
        Mail[SMTP]
    end

    Browser --> WebProcess
    WebProcess --> ApiWebProcess
    WPF --> ApiDesktopProcess
    ApiDesktopProcess --> ApiWebProcess
    ApiWebProcess --> SQL
    ApiDesktopProcess --> SQL
    ApiWebProcess --> Mail
    ApiDesktopProcess --> Mail
~~~

Puertos configurados:

| Componente | HTTP | HTTPS |
|---|---:|---:|
| Siv.Web | 5100 | 7100 |
| Siv.Api.Web | 5200 | 7200 |
| Siv.Api.Desktop | 5201 | 7201 |

## 5. Diagrama de base de datos

~~~mermaid
erDiagram
    USUARIO {
        int UsuarioId PK
        string NombreUsuario UK
        string Correo UK
        string Rol
        string PasswordHash
        bool Activo
        datetime FechaCreacion
    }

    VUELO {
        int VueloId PK
        string NumeroVuelo UK
        int AerolineaId FK
        int AeropuertoOrigenId FK
        int AeropuertoDestinoId FK
        int EstadoVueloId FK
        string Puerta
        datetime HorarioProgramado
        string NivelVisibilidad
    }

    AEROLINEA {
        int AerolineaId PK
        string Codigo UK
        string Nombre
    }

    AEROPUERTO {
        int AeropuertoId PK
        string Codigo UK
        string Nombre
        string Ciudad
        string Pais
    }

    PUERTA {
        int PuertaId PK
        string Nombre
        int AeropuertoId FK
    }

    ESTADO_VUELO {
        int EstadoVueloId PK
        string Nombre UK
    }

    RESERVA {
        int ReservaId PK
        int VueloId FK
        string Usuario
        datetime FechaReserva
        string Estado
    }

    SEGUIMIENTO {
        int SeguimientoId PK
        int VueloId FK
        string Usuario
        datetime FechaSeguimiento
    }

    CAMBIO_OPERATIVO {
        int CambioOperativoId PK
        int VueloId FK
        string TipoCambio
        string Causa
        string ValorAnterior
        string ValorNuevo
        datetime FechaCambio
    }

    HISTORIAL_ESTADO {
        int HistorialEstadoVueloId PK
        int VueloId FK
        int EstadoAnteriorId FK
        int EstadoNuevoId FK
        datetime FechaCambio
    }

    NOTIFICACION {
        int NotificacionId PK
        int VueloId FK
        int CambioOperativoId FK
        string Usuario
        string Mensaje
        bool Leida
        datetime FechaEnvio
    }

    AUDITORIA {
        int AuditoriaId PK
        string Accion
        string Tabla
        string Detalle
        datetime Fecha
    }

    AEROLINEA ||--o{ VUELO : opera
    AEROPUERTO ||--o{ VUELO : origen
    AEROPUERTO ||--o{ VUELO : destino
    AEROPUERTO ||--o{ PUERTA : contiene
    ESTADO_VUELO ||--o{ VUELO : clasifica
    VUELO ||--o{ RESERVA : recibe
    VUELO ||--o{ SEGUIMIENTO : tiene
    VUELO ||--o{ CAMBIO_OPERATIVO : registra
    VUELO ||--o{ HISTORIAL_ESTADO : conserva
    VUELO ||--o{ NOTIFICACION : origina
    CAMBIO_OPERATIVO ||--o{ NOTIFICACION : dispara
    ESTADO_VUELO ||--o{ HISTORIAL_ESTADO : participa
~~~

Nota: en la implementación actual, Reserva.Usuario, Seguimiento.Usuario y Notificacion.Usuario conservan el nombre de usuario. El correo se almacena en Usuario.Correo para poder notificar al cliente registrado.

## 6. Arquitectura lógica de la capa de presentación

### Web: MVC

~~~mermaid
flowchart LR
    Browser[Navegador]
    Controllers[Controllers MVC<br/>Auth, Vuelos, Reservas,<br/>Notificaciones]
    Models[ViewModels<br/>validación y presentación]
    Services[Servicios de consumo HTTP]
    HttpClient[HttpClient + JWT<br/>ApiServicioBase]
    Api[API Web]
    Cookies[Cookie de sesión]

    Browser --> Controllers
    Controllers --> Models
    Controllers --> Services
    Services --> HttpClient
    HttpClient --> Api
    Controllers --> Cookies
    Cookies --> Controllers
~~~

La Web gestiona la sesión mediante cookies, conserva el JWT recibido por la API y limita el acceso a UsuarioRegistrado. Sus pantallas principales son consulta de vuelos, detalle, reservas, seguimiento y notificaciones.

### Desktop: MVVM

~~~mermaid
flowchart LR
    XAML[Views XAML]
    VM[ViewModels<br/>Vuelos, CambiosOperativos,<br/>Historial, Auditoría]
    Commands[Commands + bindings]
    DesktopServices[Servicios API Desktop]
    Token[TokenManager + TokenHandler]
    ApiDesktop[API Desktop]

    XAML <-->|Data Binding| VM
    XAML --> Commands
    Commands --> VM
    VM --> DesktopServices
    DesktopServices --> Token
    Token --> ApiDesktop
~~~

El Desktop concentra las pantallas de administración y operación. El selector de puertas se carga desde api/puertas, se filtra por aeropuerto/horario y la API vuelve a validar la selección.

## 7. Diseño de integración con las APIs

### Flujo de consulta y reserva

~~~mermaid
sequenceDiagram
    actor C as Cliente
    participant W as Siv.Web
    participant A as Siv.Api.Web
    participant D as Base de datos

    C->>W: Abre vuelos disponibles
    W->>A: GET /api/vuelos/disponibles
    A->>D: Busca vuelos públicos y activos
    D-->>A: Vuelos disponibles
    A-->>W: 200 + lista de vuelos
    W-->>C: Muestra vuelos

    C->>W: Inicia sesión
    W->>A: POST /api/auth/login
    A-->>W: JWT + rol UsuarioRegistrado
    W-->>C: Sesión mediante cookie

    C->>W: Reserva un vuelo
    W->>A: POST /api/reservas + Bearer JWT
    A->>D: Valida disponibilidad y duplicados
    D-->>A: Reserva creada
    A-->>W: 201 Created
    W-->>C: Confirmación
~~~

### Flujo operativo y notificaciones

~~~mermaid
sequenceDiagram
    actor O as Operador/Admin
    participant D as Siv.Desktop
    participant AD as Siv.Api.Desktop
    participant App as Servicios de aplicación
    participant DB as Base de datos
    participant AW as Siv.Api.Web
    participant RT as SignalR
    participant SMTP as SMTP
    actor C as Cliente seguido

    O->>D: Selecciona vuelo y cambio operativo
    D->>AD: POST /api/cambiosoperativos/puerta<br/>o horario/estado/cancelacion
    AD->>App: Ejecuta regla de negocio
    App->>DB: Guarda cambio e historial
    App->>DB: Crea notificaciones para seguidores
    App->>AW: Envía evento al servicio Web
    AW->>RT: Publica notificación en tiempo real
    App->>SMTP: Envía correo si Email:Enabled=true
    SMTP-->>App: Resultado SMTP
    DB-->>AD: Operación confirmada
    AD-->>D: 200 + cambio registrado
    RT-->>C: Notificación inmediata
    SMTP-->>C: Correo de actualización
~~~

El error de SMTP se registra y no deshace el cambio operativo ni la notificación persistida.

### Contrato resumido de endpoints

| API | Endpoint | Uso | Acceso |
|---|---|---|---|
| Web | POST /api/auth/login | Login de cliente | Anónimo |
| Web | POST /api/auth/registro | Registro con correo | Anónimo |
| Web | GET /api/vuelos/disponibles | Vuelos reservables | Público |
| Web | POST /api/reservas | Crear reserva | Usuario registrado |
| Web | GET /api/reservas/mis-reservas | Consultar reservas propias | Usuario registrado |
| Web | DELETE /api/reservas/{id} | Cancelar reserva propia | Usuario registrado |
| Web | POST /api/seguimientos | Seguir vuelo | Usuario registrado |
| Desktop | POST /api/vuelos | Crear vuelo | Admin |
| Desktop | PUT /api/vuelos/{id} | Actualizar vuelo | Admin / Operador |
| Desktop | POST /api/cambiosoperativos/puerta | Cambiar puerta | Admin / Operador |
| Desktop | POST /api/cambiosoperativos/horario | Retraso o adelanto | Admin / Operador |
| Desktop | POST /api/cambiosoperativos/estado | Cambiar estado | Admin / Operador |
| Desktop | GET /api/auditorias | Consultar auditoría | Admin / Auditor |
| Desktop | GET /api/puertas | Cargar puertas disponibles | Admin / Operador |

## 8. Recomendación para entregar los diagramas

1. Abrir este archivo en GitHub o en un editor con soporte Mermaid.
2. Exportar cada bloque como SVG o PNG.
3. Si el profesor solicita UML tradicional, importar las relaciones principales en draw.io.
4. Mantener la separación: puestos administrativos y operativos en Desktop; clientes y reservas en Web.
