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