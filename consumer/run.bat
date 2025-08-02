@echo off

IF "%1" == "" (
    echo Usage: run.bat [up^|down^|rebuild^|start^|stop]
    goto end
)

IF /I "%1" == "rebuild" (
    echo Rebuilding...
    docker-compose up --build consumer
    goto end
)

IF /I "%1" == "up" (
    echo Creating container...
    docker-compose up consumer
    goto end
)

IF /I "%1" == "down" (
    echo Disposing container...
    docker-compose down consumer
    goto end
)

IF /I "%1" == "start" (
    echo Starting container...
    docker-compose start consumer
    goto end
)

IF /I "%1" == "stop" (
    echo Stopping container...
    docker-compose stop consumer
    goto end
)

echo Unknown command: %1

:end