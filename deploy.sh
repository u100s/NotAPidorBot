#!/bin/bash

# Переменные
PROJECT_PATH="/path/to/your/project"
REMOTE_USER="your_remote_user"
REMOTE_HOST="your_remote_host"
REMOTE_PORT="your_port"
REMOTE_DIRECTORY="/path/to/remote/directory"
SERVICE_NAME="YourServiceName"

# Сборка проекта
dotnet build $PROJECT_PATH -c Release

# Проверка успешности сборки
if [ $? -ne 0 ]; then
  echo "Сборка не удалась"
  exit 1
fi

# Копирование файлов на удалённый сервер
scp -P $REMOTE_PORT -r $PROJECT_PATH/bin/Release/net8.0/* $REMOTE_USER@$REMOTE_HOST:$REMOTE_DIRECTORY

# Проверка успешности копирования
if [ $? -ne 0 ]; then
  echo "Копирование файлов не удалось"
  exit 1
fi

# Перезапуск сервиса на удалённом сервере
# ssh $REMOTE_USER@$REMOTE_HOST "sudo systemctl restart $SERVICE_NAME"

# Проверка успешности перезапуска
# if [ $? -ne 0 ]; then
#   echo "Перезапуск сервиса не удался"
#   exit 1
# fi

echo "Сборка и публикация успешно завершены"
