# Entregable 3: Matriz de cumplimiento y evidencias

## 1. Estado de implementación

| Criterio | Evidencia en la solución | Estado |
|---|---|---|
| Estructura alineada con la arquitectura | Ocho proyectos separados y referencias entre capas | Cumplido |
| Registrar y consultar vuelos | `VueloServicio`, controladores Web/Desktop y clientes HTTP | Cumplido |
| Seguir un vuelo | `SeguimientoServicio` y controladores de seguimiento | Cumplido |
| Registrar cambios operativos | `CambioOperativoServicio` y `CambiosOperativosController` | Cumplido |
| Validar puertas | Entidad/repositorio de puertas y validación en cambios | Cumplido |
| Gestionar estados | `EstadoVuelo`, `CicloDeVidaVuelo` e historial | Cumplido |
| Impedir saltos de estado | Validación de secuencia en dominio | Cumplido |
| Cancelar vuelo como estado final | Validación de estado final | Cumplido |
| Generar notificaciones | `NotificacionServicio` para seguidores | Cumplido |
| Actualizar Web en tiempo real | SignalR en API Web y notificador desde API Desktop | Cumplido |
| Auditar cambios | `AuditoriaServicio` y registros desde servicios de aplicación | Cumplido |
| Auditar lecturas Web | `AuditoriaLecturaFilter` | Cumplido |
| Autorización por rol | Atributos `[Authorize]` en APIs y controladores | Cumplido |
| Integración entre Web y Desktop | Dos APIs y contratos HTTP separados | Cumplido |
| Pruebas automatizadas | Unitarias e integración en `Siv.Tests` | Cumplido después de las correcciones incluidas |

## 2. Ajustes incluidos en esta versión

Esta copia preparada para publicación contiene los siguientes ajustes respecto al ZIP original de GitHub:

- Se agregaron puertas válidas a las fábricas de pruebas de integración.
- Se habilitó el rol `Auditor` para consultar el controlador de auditorías de Desktop.
- Se fijó la versión del paquete de herramientas de Entity Framework para evitar restauraciones variables.
- Se corrigió el cierre de un elemento HTML de navegación en el layout Web.
- Se retiraron archivos huérfanos de auditoría que estaban dentro de Web sin un controlador correspondiente.

## 3. Resultado de pruebas

Ejecutar desde la raíz:

```powershell
dotnet test .\Siv.sln --configuration Release
```

Resultado esperado de la copia preparada:

```text
Passed!  Failed: 0, Passed: 19, Skipped: 0, Total: 19
```

Las pruebas verifican, entre otros aspectos, autenticación, registro de usuarios, consultas, seguimiento idempotente, seguridad de la API Web, autorización de operador y auditor, notificaciones, integración entre APIs, reservas, visibilidad por estado y auditoría del resultado de entrega.

## 4. Evidencias que deben acompañar la entrega

El código y las pruebas demuestran la implementación. Para completar la evaluación académica todavía se deben adjuntar capturas reales de la ejecución:

1. Login de usuario Web.
2. Tablero Web con vuelos.
3. Login de Desktop.
4. Registro o actualización de un vuelo.
5. Cambio de puerta u horario con causa.
6. Cambio de estado y rechazo de un salto inválido.
7. Notificación recibida en Web después de una operación Desktop.
8. Consulta de auditoría con valores anterior y nuevo.
9. Ejecución de `dotnet test` mostrando 19/19.

Guardar las imágenes en `docs/evidencias/` usando nombres descriptivos, por ejemplo `01-login-web.png` y `07-notificacion-signalr.png`, y reemplazar las marcas `[INSERTAR CAPTURA]` de la guía de evidencias.

## 5. Cierre de los ajustes técnicos

Se completaron las dos mejoras recomendadas:

- La política de visibilidad considera el nivel, el rol y el estado del vuelo. Los vuelos finalizados no se muestran en consultas públicas; los roles internos conservan acceso según su nivel.
- Cada entrega en tiempo real se audita con el resultado `Enviado` o `Fallido`, el usuario destinatario, el cambio operativo y el identificador de la notificación. La API Web devuelve `502` cuando no logra entregar el evento.

Queda como actividad de presentación adjuntar las capturas de ejecución, porque un repositorio por sí solo no demuestra la interfaz funcionando. También conviene confirmar con el profesor la puntuación del Entregable 2: el PDF menciona 10 puntos en una sección y 15 puntos en la rúbrica final.
