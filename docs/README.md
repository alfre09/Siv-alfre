# Documentación del Sistema de Información de Vuelos

Esta carpeta contiene los documentos de los tres entregables solicitados para el Sistema de Información de Vuelos (SIV).

## Documentos

1. [Especificación de requisitos](01-Especificacion-Requisitos.md)
2. [Arquitectura y diseño UML](02-Arquitectura-y-UML.md)
3. [Matriz de cumplimiento y evidencias](03-Matriz-Cumplimiento-y-Evidencias.md)
4. [Guía de evidencias](evidencias/README.md)

## Diagramas fuente

Los diagramas UML editables están en [`diagramas/`](diagramas/):

- Casos de uso.
- Componentes e integración.
- Clases principales del dominio y aplicación.
- Secuencias de los flujos clave.

Los archivos `.puml` pueden abrirse con PlantUML, IntelliJ IDEA, Visual Studio Code o cualquier visor compatible.

## Relación con el código

La documentación describe la solución que se encuentra en los proyectos:

```text
Siv.Web       -> Siv.Api.Web       -> Siv.Application -> Siv.Persistence
Siv.Desktop   -> Siv.Api.Desktop   -> Siv.Application -> Siv.Persistence
Siv.Api.Web   <-> Siv.Api.Desktop  (notificaciones y coordinación)
```

Los servicios que existen en `Siv.Web` y `Siv.Desktop` son adaptadores HTTP de cliente. La lógica de negocio se concentra en `Siv.Application` y las reglas invariantes principales en `Siv.Domain`.
