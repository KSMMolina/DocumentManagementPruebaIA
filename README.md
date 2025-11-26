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

## Cómo subir la solución
Si necesitas compartir el proyecto resultante, tienes dos caminos sugeridos:

1. **Subirlo a tu repositorio remoto (Git):**
   - Configura el origen: `git remote add origin <URL-de-tu-repo>`.
   - Sube la rama de trabajo: `git push origin work` (o la rama que uses).
   - Verifica en el repositorio remoto que los archivos estén completos (incluye `database/schema.sql` y `docs/prompts.md`).

2. **Entregarlo como archivo comprimido:**
   - Desde la raíz del proyecto, ejecuta `zip -r DocumentManagementPruebaIA.zip .`.
   - Comparte el archivo `.zip` generado, asegurándote de incluir toda la estructura (Domain, Application, Infrastructure, API, database y docs).

En ambos casos, indica también cualquier comando necesario para ejecutar el proyecto según la sección “Cómo ejecutar”.
