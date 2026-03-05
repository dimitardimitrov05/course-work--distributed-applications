# Факултетен номер: 2401322023, 2401322005
# Проект: Система за управление на хотелски резервации

## Описание
Система за управление на хотелски резервации с RESTful API back-end и ASP.NET Core MVC front-end.

## Технологии
- ASP.NET Core Web API (.NET 8)
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- MS SQL Server
- JWT автентикация
- ASP.NET Core Identity
- Swagger/OpenAPI

## Инструкции за стартиране
1. Отвори solution-а във Visual Studio
2. Смени connection string-а в `HotelSystem.API/appsettings.json`
3. В Package Manager Console избери `HotelSystem.API` и пусни `Update-Database`
4. Стартирай `HotelSystem.API` проекта и провери на кой порт върви
5. В `HotelSystem.MVC/Program.cs` смени порта:
```csharp
   client.BaseAddress = new Uri("https://localhost:ТВОЯ_ПОРТ/");
```
6. Настрои Multiple Startup Projects в Properties на solution-а - и двата проекта на Start
7. Натисни F5 - работи се с Login таба
