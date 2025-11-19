# Asistencia-Pilares

Proyecto de ejemplo: API de asistencia.

Este README resume cómo trabajar en desarrollo y producción, migraciones, y el comportamiento de seeding.

## Estructura relevante
- `src/Core` - entidades del dominio
- `src/Infrastructure` - DbContext, migraciones y seeding
- `src/API` - proyecto web que registra servicios y expone endpoints

## Modos de ejecución

### Desarrollo (SQLite local)
Por defecto `appsettings.Development.json` apunta a `DataSource=asistencia.db`.
Esto usa SQLite para desarrollo y ejecuta `EnsureCreated()` + seeding en arranque.

Arrancar la app en desarrollo:

```zsh
cd /home/luis4armenta/proyecto_terminal/src/API
dotnet run --project AsistenciaAPI.API.csproj
```

Ver endpoint de salud/DB:

```zsh
curl http://localhost:5172/health/db
```

### Producción (SQL Server)
En producción la app usa `ConnectionStrings:DefaultConnection` de `appsettings.json` o la variable de entorno `ConnectionStrings__DefaultConnection`.
Por defecto el ejemplo apunta a SQL Server en `localhost,1433` (útil si usas Docker). En producción la app ejecuta `Database.Migrate()` en arranque.

Ejemplo: levantar SQL Server en Docker:

```zsh
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Your_password123' -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Configura la cadena de conexión en la variable de entorno y arranca en modo Production:

```zsh
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=AsistenciaDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
ASPNETCORE_ENVIRONMENT=Production dotnet run --project src/API/AsistenciaAPI.API.csproj
```

## Migraciones
Las migraciones se mantienen en `src/Infrastructure/Migrations`.
Para crear una nueva migración:

```zsh
cd src/Infrastructure
dotnet ef migrations add NombreCambio --project AsistenciaAPI.Infrastructure.csproj --startup-project ../API/AsistenciaAPI.API.csproj
```

Aplicar migraciones manualmente:

```zsh
cd src/Infrastructure
dotnet ef database update --project AsistenciaAPI.Infrastructure.csproj --startup-project ../API/AsistenciaAPI.API.csproj
```

## Seeding
Seeding idempotente implementado en `src/Infrastructure/Persistence/DbSeeder.cs` y abstraído por `ISeeder`. En dev se ejecuta automáticamente por `EnsureCreated()`. En producción se ejecuta después de `Database.Migrate()`.

Si prefieres no ejecutar seed automáticamente en producción, modifica `src/Infrastructure/Services/MigrationService.cs` para evitar llamar a `ISeeder.Seed()` cuando `IHostEnvironment.IsProduction()`.

## Tests
No hay tests incluidos aún. Recomendación: crear un proyecto de tests y usar `Microsoft.Data.Sqlite` en memoria o `InMemory` provider para testear `MigrationService` y la lógica de `ISeeder`.

## Notas
- EnsureCreated vs Database.Migrate: EnsureCreated crea el esquema directamente y no es compatible con migraciones; Database.Migrate aplica migraciones y es la forma recomendada para producción.

---

Si quieres, creo un proyecto de tests con ejemplos de unit tests para `MigrationService` y `DbSeeder`.

## Cambios en la API: `CrearEmpleadoDto`

La API ahora acepta el campo `AreaId` (opcional) y/o `NombreArea` al crear o actualizar empleados.

- Prioridad: si se proporciona `AreaId`, se prioriza y debe existir en la base de datos (se retornará error si no existe).
- Si `AreaId` no se proporciona, se utilizará `NombreArea`: la API buscará un área con ese nombre (case-insensitive) y si no existe la creará automáticamente.

Si prefieres otro comportamiento (por ejemplo: rechazar nombres nuevos en producción), indícalo y lo ajusto.

