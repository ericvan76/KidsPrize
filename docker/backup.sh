#!/bin/bash

docker run --rm --volumes-from docker_db_1 -v $(pwd)/backup:/backup ubuntu tar cvf /backup/data_$(date +%Y%m%d_%H%M%S).tar /var/lib/postgresql/data