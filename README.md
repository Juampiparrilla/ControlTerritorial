# Control Territorial

Este proyecto es una aplicación diseñada para organizar y gestionar estructuras territoriales en campañas políticas o movimientos sociales. Permite tener un control detallado sobre la jerarquía de participantes, la logística del día de la elección y la ubicación de los votantes en escuelas y mesas específicas.

## 🎯 Objetivo del Sistema

El sistema busca proveer una plataforma centralizada para:
- **Tener visibilidad de la estructura territorial:** Visualizar la jerarquía de roles desde Administradores hasta Votantes.
- **Gestión de Liderazgos:** Saber exactamente qué personas dependen de cada líder (Ej. Referentes → Punteros → Votantes).
- **Control de Votación:** Asociar personas con escuelas y mesas específicas de votación.
- **Logística Operativa:** Organizar y asignar vehículos para el traslado de votantes.

## 🚀 Arquitectura y Tecnologías

La aplicación está construida sobre una arquitectura sólida y moderna para asegurar mantenibilidad y escalabilidad.

- **Backend:** C# / .NET 8 / ASP.NET Core Web API
- **Arquitectura:** Arquitectura Limpia basada en N-Capas (Domain, Application, Infrastructure, API).
- **ORM:** Entity Framework Core
- **Base de Datos:** PostgreSQL
- **Patrones de Diseño:** Patrón Repositorio, Inyección de Dependencias, DTOs (Data Transfer Objects).

## 🏢 Modelo y Casos de Uso

### Gestión de Roles y Estructura Jerárquica
El sistema maneja distintos niveles con diferentes alcances y permisos de visibilidad:
- **Administrador:** Gestión total del sistema y visión global.
- **Grupo:** Gestiona y ve la información de su agrupación (Referentes y Punteros).
- **Referente:** Gestiona su información y tiene visibilidad de sus Punteros.
- **Puntero:** Gestiona su listas y visualiza a los Votantes que dependen de él.
- **Votante:** Nivel base. Solo posee relación hacia arriba y con centros de votación.
- **Chofer:** Encargado de la logística vehicular.

### Entidades Principales
- `Persona`: Núcleo del sistema. Implementa relaciones jerárquicas directas (Líder / Subordinados).
- `Escuela` y `Mesa`: Estructura para la asignación de lugares físicos de votación.
- `Vehiculo`: Registro del parque automotor destinado a logística y transporte en terreno.

## 🛠️ Estructura del Proyecto

- `ControlTerritorial.API`: Capa de presentación (Controladores y configuración de arranque).
- `ControlTerritorial.Application`: Casos de uso de la aplicación, Interfaces, DTOs y validaciones lógicas (Capa de Servicios).
- `ControlTerritorial.Domain`: Corazón del negocio. Contiene las Entidades (Entities) y Enum/Reglas centrales. No tiene dependencias de infraestructura.
- `ControlTerritorial.Infrastructure`: Implementaciones técnicas. Contexto de Base de datos (DbContext), Migraciones y clases concretas de Repositorios.

## 🚧 Próximos Pasos (Roadmap)
- [] Implementación de UI/Frontend.
- [] Desarrollo del módulo de control y seguimiento en tiempo real ("Día D").
- [] Implementación de Seguridad/Autenticación con JWT.
- [] Integración de georreferenciación.

---
*Desarrollado y diseñado para garantizar una administración territorial clara y estructurada.*
