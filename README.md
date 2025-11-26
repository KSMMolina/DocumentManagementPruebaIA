# DocumentManagementPruebaIA

Estructura inicial para el módulo de Gestión Documental siguiendo principios de DDD y Arquitectura Limpia en .NET 8. Incluye el modelo de dominio, casos de uso principales, adaptadores en memoria y script SQL para PostgreSQL.

## Estructura del proyecto
- `Domain/`: Entidades, Value Objects, enums y eventos del dominio.
- `Application/`: Casos de uso y contratos (puertos) para persistencia.
- `Infrastructure/`: Adaptadores (in-memory), configuración de EF Core y extensiones para DI.
- `API/`: Minimal API con endpoint de ejemplo y configuración de Swagger.
- `database/schema.sql`: Script SQL normalizado para la base de datos `DocumentManagementIaTest`.
- `docs/prompts.md`: Registro de prompts utilizados.

## Cómo ejecutar (local)
1. Instala .NET 8 SDK y PostgreSQL.
2. Crea la base de datos `DocumentManagementIaTest` y ejecuta el script `database/schema.sql`.
3. Desde la raíz, restaura dependencias y publica la solución:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project API/DocumentManagement.API.csproj
   ```
4. Explora los endpoints en Swagger en `http://localhost:5000/swagger` (o el puerto configurado).

## Reglas de negocio cubiertas
- Máximo 3 niveles de carpetas y 2 subcarpetas por nivel.
- Máximo 5 archivos por carpeta y tamaño límite de 50 MB por archivo.
- Permisos inmutables para administrador; solo acceso de lectura/descarga.
- Auditoría de operaciones clave (creación, permisos, carga/actualización de archivos).
