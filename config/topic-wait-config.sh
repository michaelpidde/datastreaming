#!/bin/bash

BROKER=kafka:9092
MAX_TRIES=30
RETENTION_MS=604800000


# Wait for Kafka service to be accessible -
# there is a delay between Docker saying it's Running and it really being available
for i in {1..MAX_TRIES}
do
  echo "Checking if Kafka is ready (try $i)..."
  kafka-topics.sh --bootstrap-server "$BROKER" --list >/dev/null 2>&1
  # $? stores the exit status of the last command
  if [ $? -eq 0 ]; then
    echo "Kafka is ready..."
    break
  fi
  sleep 2
done


create_and_config_topic() {
    # Will only create if it does not exist
    kafka-topics.sh --bootstrap-server "$BROKER" \
    --create --if-not-exists --topic $1 \
    --partitions $2 \
    --replication-factor 1 \
    --config retention.ms=$RETENTION_MS

    kafka-configs.sh --bootstrap-server "$BROKER" \
    --entity-type topics --entity-name $1 \
    --alter --add-config retention.ms=$RETENTION_MS
}

create_and_config_topic "sqlserver.application.dbo.customer" 1
create_and_config_topic "sqlserver.application.dbo.product" 1
create_and_config_topic "sqlserver.application.dbo.order" 1
