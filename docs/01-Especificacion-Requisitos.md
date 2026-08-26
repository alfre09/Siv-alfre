# Entregable 1: Especificación de Requisitos del SIV

**Sistema:** Sistema de Información de Vuelos (SIV)  
**Versión:** 1.0  
**Estado:** Documento base para revisión académica

## 1. Propósito

El Sistema de Información de Vuelos centraliza la programación, consulta, seguimiento y actualización operativa de vuelos de un aeropuerto. La solución permite que los usuarios consulten información vigente, que los operadores registren cambios operativos y que los usuarios interesados reciban notificaciones trazables.

El sistema está compuesto por una aplicación Web para consulta pública y seguimiento, una aplicación Desktop para operación y auditoría, una API Web y una API Desktop. Ambas APIs reutilizan el dominio, los casos de uso y la persistencia para mantener reglas uniformes.

## 2. Problema identificado

La información de vuelos puede quedar distribuida entre diferentes medios y aplicaciones. Esto dificulta conocer el estado actual de un vuelo, informar cambios de horario o puerta, avisar oportunamente a los usuarios y reconstruir quién realizó cada operación.

El SIV atiende este problema mediante:

- Un registro central de vuelos.
- Estados operativos controlados.
- Historial de cambios y estados.
- Seguimiento por usuario.
- Notificaciones asociadas a cambios operativos.
- Auditoría de lecturas y modificaciones relevantes.
- Separación de permisos por rol.

## 3. Alcance

### 3.1 Incluido

- Registrar y actualizar la programación de un vuelo.
- Asociar aerolínea, aeropuerto de origen, aeropuerto de destino, horario y puerta.
- Consultar vuelos y detalles según el nivel de visibilidad del vuelo y el rol del usuario.
- Consultar historial de cambios operativos.
- Seguir y dejar de seguir vuelos.
- Registrar retrasos, adelantos, cambios de puerta, cambios de estado y cancelaciones.
- Mantener el ciclo de vida y el historial de estados del vuelo.
- Generar notificaciones para usuarios que siguen el vuelo afectado.
- Consultar auditorías desde la aplicación de escritorio.
- Aplicar autenticación y autorización por roles.
- Exponer contratos HTTP diferenciados para Web y Desktop.

### 3.2 Fuera del alcance

- Compra de pasajes con pasarelas de pago externas.
- Control físico de aeronaves, pistas o equipaje.
- Integración con sistemas externos de aerolíneas no incluidos en la solución.
- Envío de SMS o correo electrónico mediante proveedores externos.
- Gestión de turnos de personal aeroportuario.
- Optimización automática de asignación de puertas mediante inteligencia artificial.

## 4. Actores

| Actor | Responsabilidad principal |
|---|---|
| Visitante | Consultar información pública de vuelos visibles. |
| Usuario registrado | Consultar vuelos, seguir vuelos, recibir notificaciones y gestionar reservas disponibles en Web. |
| Administrador | Administrar catálogos, usuarios, vuelos, visibilidad y operaciones autorizadas. |
| Operador | Gestionar la operación diaria: horarios, puertas, estados y cancelaciones. |
| Auditor | Consultar historial de auditoría, cambios y estados sin modificar la operación. |
| Sistema SIV | Validar reglas, persistir información, generar auditorías y distribuir notificaciones. |

## 5. Reglas de negocio

### RN-01. Visibilidad

La información de un vuelo se consulta según su nivel de visibilidad, el rol del usuario y el ciclo de vida. Un administrador puede consultar toda la información; operadores y auditores consultan información no restringida incluso cuando el vuelo está en estado final; visitantes y usuarios registrados consultan información pública de vuelos que todavía están operativos.

### RN-02. Programación válida

Un vuelo debe tener número, aerolínea, origen, destino, horario programado y un estado inicial válido. No se debe activar un vuelo sin una programación válida.

### RN-03. Seguimiento

Un usuario puede seguir un vuelo válido. Registrar o eliminar un seguimiento no debe cambiar el estado operativo del vuelo. El seguimiento es idempotente para evitar duplicados del mismo usuario y vuelo.

### RN-04. Cambios operativos

Los cambios soportados incluyen retraso, adelanto, cambio de puerta, cambio de estado y cancelación. Cada cambio debe indicar una causa, conservar el valor anterior y el valor nuevo, y quedar asociado al vuelo.

### RN-05. Vuelo cancelado

Un vuelo en estado final no puede continuar su ciclo operativo. La cancelación es un estado final y debe quedar registrada en el historial.

### RN-06. Secuencia de estados

Los estados operativos deben avanzar en la secuencia definida por el dominio: `Programado -> Embarcando -> En Vuelo -> Aterrizado`. No se permite saltar estados. La cancelación puede producirse como transición final.

### RN-07. Notificaciones

Un cambio operativo puede generar notificaciones para los usuarios que siguen el vuelo. Cada notificación debe referenciar el vuelo y el cambio que la originó. La entrega en tiempo real debe auditarse individualmente como `Enviado` o `Fallido`.

### RN-08. Auditoría

Las operaciones relevantes deben registrar acción, tabla o recurso, usuario o contexto, registro afectado, descripción y, cuando aplique, valor anterior y valor nuevo. Las lecturas de la API Web se registran mediante el filtro de auditoría.

### RN-09. Autorización

Las operaciones de consulta, modificación y auditoría se restringen mediante autenticación y roles. La API Web no expone las operaciones internas de la API Desktop.

## 6. Requisitos funcionales

### Gestión y consulta de vuelos

- **RF-01:** El sistema debe permitir registrar un vuelo con programación válida.
- **RF-02:** El sistema debe permitir actualizar número, aerolínea, ruta, horario, puerta y visibilidad según permisos.
- **RF-03:** El sistema debe permitir consultar listado y detalle de vuelos.
- **RF-04:** El sistema debe filtrar la información según visibilidad y rol.
- **RF-05:** El sistema debe mostrar el estado actual, historial de estados y cambios del vuelo.

### Operación

- **RF-06:** El operador debe poder registrar retrasos y adelantos con causa.
- **RF-07:** El operador debe poder registrar cambios de puerta con causa y validar que la puerta exista y esté disponible.
- **RF-08:** El operador debe poder cambiar el estado respetando la secuencia del ciclo de vida.
- **RF-09:** El operador debe poder cancelar un vuelo con causa.
- **RF-10:** El sistema debe impedir cambios incompatibles con un estado final.

### Seguimiento y reservas

- **RF-11:** El usuario registrado debe poder seguir un vuelo.
- **RF-12:** El usuario registrado debe poder dejar de seguir un vuelo.
- **RF-13:** El sistema debe impedir seguimientos duplicados para el mismo usuario y vuelo.
- **RF-14:** El usuario registrado debe poder consultar sus notificaciones y marcar las propias como leídas.
- **RF-15:** El usuario registrado debe poder gestionar las operaciones de reserva disponibles en la aplicación Web.

### Notificaciones y auditoría

- **RF-16:** El sistema debe generar notificaciones para los seguidores afectados por un cambio.
- **RF-17:** Cada notificación debe quedar vinculada al vuelo y al cambio operativo que la originó.
- **RF-18:** El sistema debe registrar auditoría de creación, actualización, seguimiento, cambio operativo y notificación.
- **RF-19:** El auditor debe poder consultar auditorías desde la aplicación Desktop.
- **RF-20:** El sistema debe conservar el historial necesario para supervisión y análisis posterior.

## 7. Requisitos no funcionales

- **RNF-01 Seguridad:** Las APIs deben usar autenticación, autorización por roles y tokens; las contraseñas no deben almacenarse en texto plano.
- **RNF-02 Integridad:** Las operaciones que modifican vuelo, cambio operativo, auditoría y notificación deben ejecutarse de forma transaccional cuando corresponda.
- **RNF-03 Trazabilidad:** Los cambios deben conservar fecha, tipo, causa, valores anterior y nuevo cuando aplique.
- **RNF-04 Disponibilidad:** La aplicación debe poder ejecutarse localmente levantando API Web, API Desktop, Web y Desktop en el orden indicado por el README.
- **RNF-05 Rendimiento:** Las consultas principales deben ser asíncronas y devolver solo los DTO necesarios para la interfaz.
- **RNF-06 Mantenibilidad:** El dominio y la aplicación no deben depender de las interfaces de usuario; las dependencias se resuelven mediante interfaces e inyección de dependencias.
- **RNF-07 Usabilidad:** Las interfaces deben mostrar estados, errores de validación y resultados de las operaciones de forma comprensible.
- **RNF-08 Interoperabilidad:** Web y Desktop deben comunicarse mediante contratos HTTP separados y documentados en sus controladores y servicios cliente.

## 8. Criterios de aceptación

1. Un visitante puede consultar vuelos públicos.
2. Un usuario registrado puede seguir un vuelo y recibir el cambio que le corresponde.
3. Un operador puede realizar un cambio operativo válido con causa.
4. El sistema rechaza una transición de estado inválida o un cambio sobre un vuelo cancelado.
5. El cambio queda disponible en el historial y en auditoría.
6. La API Web no permite ejecutar operaciones internas ni consultar auditoría administrativa.
7. El auditor puede consultar los registros desde la API Desktop.
8. Las pruebas automatizadas de solución finalizan correctamente.
