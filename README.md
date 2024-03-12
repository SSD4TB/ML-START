Клиент-серверное взаимодействие работает, имеются фризы в клиентской программе при авторизации (работаю над решением, как и над автоматическим созданием БД).
Логирование присутствует в клиенте, но пока под него попадает далеко не всё. Вместе с фризами расширю область логирования и добавлю её на серверную часть.

OS - Windows10
.NET 8.0
UI - WPF
СУБД - MS SQL SERVER

Скрипты для БД:
```
Use master;
CREATE DATABASE [ML START];
```
```
Use [ML START];
CREATE TABLE userAuth (
                    userLogin VARCHAR(255) PRIMARY KEY NOT NULL,
                    password VARCHAR(255) NOT NULL
                    )
```
