@echo off

IF "%1" == "" (
    echo Usage: run.bat [up^|down^|rebuild^|start^|stop]
    goto end
)

IF /I "%1" == "rebuild" (
    echo Rebuilding...
    docker-compose up --build consumer-order --scale consumer-order=3
    goto end
)

IF /I "%1" == "up" (
    echo Creating container...
    docker-compose up consumer-order --scale consumer-order=3
    goto end
)

IF /I "%1" == "down" (
    echo Disposing container...
    docker-compose down consumer-order
    goto end
)

IF /I "%1" == "start" (
    echo Starting container...
    docker-compose start consumer-order
    goto end
)

IF /I "%1" == "stop" (
    echo Stopping container...
    docker-compose stop consumer-order
    goto end
)

echo Unknown command: %1

:end