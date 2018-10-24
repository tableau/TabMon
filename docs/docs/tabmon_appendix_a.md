---
title: Appendix A - Additional Installation Instructions
layout: default
---

In this section:

* TOC
{:toc}



### Manually Installing and Configuring Postgres

Follow the steps below to manually install PostgreSQL on your system. If you already have a PostgreSQL server and just need to initialize the TabMon results database, skip to step 3.

1.  Download Postgres:

   [http://www.enterprisedb.com/products-services-training/pgdownload\#windows](http://www.enterprisedb.com/products-services-training/pgdownload#windows){:target="_blank"}

2.  Run the installer and use the following options in the setup wizard:

    1.  Superuser password: your choice

    2.  Port: 5432

    3.  Locale: English, United States

    4.  If prompted to launch StackBuilder, choose no.

3.  Open PGadminIII

    1.  Connect to your Postgres server using the postgres login account.

    2.  Right click on Login Roles and choose New Login Role

    3.  Enter the following parameters:

        1.  Role name: tabmon

        2.  Definition: Enter a password

        3.  Role privileges: Select “Can create databases”

    4.  Right click on Databases and choose New Database

    5.  Enter the following parameters:

        1.  Name = TabMon

        2.  Owner = tabmon

        3.  Encoding = UTF8 
        
        4.  Tablespace = pg\_default

        5.  Collation = English\_United States.1252

        6.  Character type = English\_United States.1252

        7.  Connection Limit = -1

        8.  Under privileges:

            1.  Give all privileges to tabmon

            2.  Revoke all privileges from public

4.  Update TabMon.config

    1.  Open up `C:\Program Files (x86)\TabMon\Config\TabMon.config` and change the configured “tabmon” database user password to whatever you set in step 3.

**Note that by default, Postgres does not allow any remote connections**. If you want to connect to Postgres from a remote machine, you will need to add a pg\_hba.conf entry to enable it. See instructions for this at: [*https://www.postgresql.org/docs/current/static/auth-pg-hba-conf.html*.](https://www.postgresql.org/docs/current/static/auth-pg-hba-conf.html){:target="_blank"}


### Uninstalling TabMon and Postgres

TabMon can be removed by simply uninstalling it through the Windows “Add/Remove Programs” interface. Application log files and CSV results are left behind and must be manually deleted.

**However, in order to preserve your data, this uninstall does not automatically remove Postgres.**

To fully remove Postgres:

1.  Open up a console window as administrator.

2.  Enter the following commands to remove the service:

    `sc stop TabMon-Postgres`

    `sc delete TabMon-Postgres`

3.  Delete the folder where you installed Postgres (default is `C:\Postgres`).


### Using TabMonConfigBuilder.exe To Help Build TabMon.config

With the release of TabMon v1.3, TabMon.Config now allows users to specify JMX ports. This means that the configuration can be more complicated to create. TabMonConfigBuilder.exe is a small utility that parses the results from `tsm topology list-ports` and creates the framework for a cluster in the TabMon.Config.

To run TabMonConfigBuilder.exe follow the steps below:

1.  Download TabMonConfigBuilder.zip at this link:
      [*https://github.com/tableau/TabMon/releases/download/v1.3/TabMonConfigBuilder.zip*.](https://github.com/tableau/TabMon/releases/download/v1.3/TabMonConfigBuilder.zip){:target="_blank"}
      
2.  Unzip TabMonConfigBuilder.zip.

3.  Open a command prompt in administrator mode and navigate to the directory that TabMonConfigBuilder was unzipped into.

NOTE: Anything between `[]` should be replaced with the description.

4.  Log into tsm on the cluster that you wish to add to the TabMon config.
      Command: tsm login -u `[USERNAME]`
      
5.  Pipe tsm topology list-ports to a file.
      Command: tsm topology list-ports > `[OUTPUT_FILE]`
      Example: tsm topology list-ports > "C:\topology_out.txt"
      
File topology_out.txt output:
![]({{ site.baseurl }}/assets/TabMonConfigSpecifyPorts.PNG)
      
      
6. Run TabMonConfigBuilder with topology output and output file.
      Command: tabmonconfigbuilder `[TARGET]` `[OUTPUT]`
      Example: tabmonconfigbuilder "C:\topology_out.txt" "C:\output.txt"
      
7.  Open "output.txt" and verify that the host and process count are correct. If there are any discrepancies, you may need to manually add or remove hosts and processes.

8.  For each host, replace the worker in `computerName` and `address` entry to reflect the correct host information.

9.  Copy the information below the line into the `Cluster` section of the TabMon.Config.


TabMonConfigBuilder.exe output should look something like this:
![]({{ site.baseurl }}/assets/TabMonConfigBuilderOut.PNG)

