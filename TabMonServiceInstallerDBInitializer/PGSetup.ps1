# This script initializes Postgres, creates the Postgres service, creates the tabmon database, creates the tabmon user, and edits the config values to allow for remote access.
param(
	[string]$LogDir = ".",
	[string]$InstallDir = "C:\Postgres",
	[string]$SuperUser = "tabmonsuperuser",
	[string]$Port = "5432",
	[string]$DBName = "TabMon",
	[string]$ServiceName = "TabMon-Postgres",
	[string]$Username = "tabmonuser",
	[string]$Password = "password"
)
# Creates the log file for this script and changes the directory to the Postgres bin folder.
New-Item $LogDir\DBCreation.log -type file -Force;
cd $InstallDir\bin *>> $LogDir\DBCreation.log;

# Adds a rule in the firewall to ensure that the Postgres port is open.
netsh advfirewall firewall add rule name="Open Port $Port" dir=in action=allow protocol=TCP localport=$Port *>> $LogDir\DBCreation.log;

# Initializes Postgres, creates the Postgres service, and starts the service.
.\initdb --username=$SuperUser --auth=trust --pgdata="$InstallDir\data" --encoding=utf8 *>> $LogDir\DBCreation.log;
.\pg_ctl register -N $ServiceName -U "NT AUTHORITY\SYSTEM" -D "$InstallDir\data" -W *>> $LogDir\DBCreation.log;
start-service $ServiceName *>> $LogDir\DBCreation.log;

# Replaces lines in the postgresql.conf file to allow postgres to listen on all ports. Also sets the port that postgres will run on.
(gc $InstallDir\data\postgresql.conf).replace("#listen_addresses = 'localhost'		# what IP address(es) to listen on;", "listen_addresses = '*'		# what IP address(es) to listen on;") | sc $InstallDir\data\postgresql.conf;
(gc $InstallDir\data\postgresql.conf).replace("#port = 5432				# (change requires restart)", "port = $Port				# (change requires restart)") | sc $InstallDir\data\postgresql.conf;

# Restarts the service to set the postgresql.conf values.
restart-service $ServiceName *>> $LogDir\DBCreation.log;
Start-Sleep -s 5; # Wait time is included to allow slower computers to fully restart the Postgres Process. Without the wait a slow computer may try creating the DB before the process completely is started.

# Creates the database that Tabmon will use. Also creates the (non super) user that will access the database.
.\createdb --port=$Port --username=$SuperUser --owner=$SuperUser --no-password $DBName *>> $LogDir\DBCreation.log;
.\psql --dbname=$DBname --username=$SuperUser -w --port=$Port --command="CREATE USER $Username WITH PASSWORD '$Password';" *>> $LogDir\DBCreation.log;

# Replaces lines in the pg_hba.conf file to allow remote access for the tabmon user, force password authentication for remote users, and enable trusted authentication for all local users.
(gc $InstallDir\data\pg_hba.conf).replace("host    all             all             127.0.0.1/32            trust", "host    all             $SuperUser      127.0.0.1/32            trust`nhost    all             $Username         all                   password") | sc $InstallDir\data\pg_hba.conf *>> $LogDir\DBCreation.log;
(gc $InstallDir\data\pg_hba.conf).replace("host    all             all             ::1/128                 trust", "host    all             $SuperUser      ::1/128                 trust`nhost    all             $Username         ::1/128                 trust") | sc $InstallDir\data\pg_hba.conf *>> $LogDir\DBCreation.log;

# Restarts Postgres to ensure all config changes are made.
restart-service $ServiceName *>> $LogDir\DBCreation.log;