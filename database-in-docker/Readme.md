# Database Microsoft SQL Server in docker linux container

What will you learn?
* Run the database from the docker's container
* Enable permanent writing to the database
* Create tables when the container starts

# Run the database from the docker's container
Steps to be made:
### 1. Check if you have a docker installed:
```
docker -v
```
### 2. Download image of Microsoft Sql server:
```
docker pull microsoft/mssql-server-linux
```
### 3. Run container:
```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 --name sqlserver -d microsoft/mssql-server-linux
```

description of the parameters used:
- "**-e**" Environment variable, ACCEPT_EULA=Y means that you agree to all questions when starting the database, SA_PASSWORD=yourStrong(!)Password' passwords must be strong.
* "**-p**" Maps ports inside the container outside.
* "**-d**" Detached mode means that a Docker container runs in the background of your terminal
* "**-n**" Set name of container.

## Now you can use connection string to connect database:
```
server=localhost;user=SA;password=yourStrong(!)Password;database=master;trusted_connection=false
```

# Enable permanent writing to the database
Steps to be made:

To create a disk space where files from the database will be stored, you need to map this folder inside contenera.
Depending on the environment (windows / linux) these will be different paths. The --volume, -v parameter is used for this.

**For Linux**
```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 --name sqlserver -d -v /my/host/data/directory:/var/opt/mssql/data microsoft/mssql-server-linux
```
**For Windows**
```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 --name sqlserver -d -v /C/data:/var/opt/mssql/data microsoft/mssql-server-linux
```
Note that the path inside contenera is immutable in both cases.

# Create tables when the container starts
Steps to be made:

This is how the structured file should look like at the end of the work:

```
├── mssql-docker-start.bat
├── mssql-docker-start.sh
└── sql
    ├── docker-compose-win.yml
    ├── docker-compose.yml
    ├── docker-entrypoint-initdb.sh
    ├── query_examples.sql
    └── sql_init_scripts
        ├── execute_query.sql
```

### 1. Queries in **sql_init_scripts** folder:

You can create one file with which will be queries to the SQL database forming the structure of tables with content.

**execute_query.sql***
```
USE master
GO
DROP DATABASE mydatabase;
GO
CREATE DATABASE mydatabase;
GO
USE mydatabase;
GO
CREATE TABLE user(
  Id nvarchar(max),
  Value nvarchar(max)
);
GO
INSERT INTO user
VALUES 
    ('1', 'password'),
    ('2', 'password')
GO
```

### 2. Create **docker-compose.yml** file:

```
version: '3.3'
services:
  mssql:
    image: microsoft/mssql-server-linux:latest
    container_name: mssql-server
    ports:
      - 1433:1433
    volumes:
      - /usr/src/data/:/var/opt/mssql/data
      # we copy our scripts onto the container
      - ./:/usr/src/app 
    # bash will be executed from that path, our scripts folder
    working_dir: /usr/src/app 
    # run the entrypoint.sh that will import the data AND sqlserver
    command: sh -c ' chmod +x ./entrypoint.sh; ./entrypoint.sh & /opt/mssql/bin/sqlservr;'
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: yourStrong(!)Password
```

### 3. Create **docker-entrypoint-initdb.sh** shell file:

This is a bash file that will run after creating the database docker container and will execute all queries in files with the extension .sql thanks to the sqlcmd tool.

```
#!/bin/bash

# wait for database to start...
for i in {30..0}; do
  if /opt/mssql-tools/bin/sqlcmd -U SA -P $SA_PASSWORD -Q 'SELECT 1;' &> /dev/null; then
    echo "$0: SQL Server started"
    break
  fi
  echo "$0: SQL Server startup in progress..."
  sleep 1
done

echo "$0: Initializing database"
for f in sql_init_scripts/*; do
  case "$f" in
    *.sh)     echo "$0: running $f"; . "$f" ;;
    *.sql)    echo "$0: running $f"; /opt/mssql-tools/bin/sqlcmd -U SA -P $SA_PASSWORD -X -i  "$f"; echo ;;
    *)        echo "$0: ignoring $f" ;;
  esac
  echo
done
echo "$0: SQL Server Database ready"
```

### 4. Start the container:

Run the command from the **sql** folder
```
docker-compose up -d
```