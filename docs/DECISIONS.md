# Decisions

- **Database:** Для MessageBoard выделена отдельная Azure SQL Database, чтобы изолировать данные проекта. Для работы с БД используем EF Core.

- **Limits:** Ограничения длины полей находятся в CommentLimits в Domain и используются в EF configuration. Это позволяет хранить значения в одном месте.

- **Secrets:** Секреты и строка подключения хранятся в User Secrets и читаются через IConfiguration. В Git добавляем только шаблон без секретных значений.
