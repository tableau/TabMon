---
title: Prerequisites for Installing TabMon
layout: default
---

In this section:

* TOC
{:toc}


To install TabMon on your system please ensure that you have the following:

### System Requirements
-   OS: Windows

-   Environment:
    -   Tableau Server (9.0 or above) [*http://www.tableau.com/products/server*](http://www.tableau.com/products/server){:target="_blank"} 
    -   Microsoft .NET framework 4.5 and above (installer will install this for you) [*http://www.microsoft.com/en-us/download/details.aspx?id=30653*](http://www.microsoft.com/en-us/download/details.aspx?id=30653){:target="_blank"} 
    -   Visual C++ Redistributable Packages for VS 2013 (installer will install this for you) [*http://www.microsoft.com/en-us/download/details.aspx?id=40784*](http://www.microsoft.com/en-us/download/details.aspx?id=40784){:target="_blank"}
    -   Windows Management Framework 3.0 [*http://www.microsoft.com/en-us/download/details.aspx?id=34595*](http://www.microsoft.com/en-us/download/details.aspx?id=34595){:target="_blank"}

### Configure Tableau Server

**Open Tableau Server JMX ports:**

1. Open command line as administrator

2.  Navigate to Tableau Server Bin Folder 
 
3. Run the following tabadmin commands:

```
Tableau Server 2018.1 and Older
    tabadmin set service.jmx_enabled true

    tabadmin stop

    tabadmin configure

    tabadmin start
```

```
Tableau Server 2018.2+
    tsm configuration set -k service.jmx_enabled -v true
    
    tsm pending-changes apply
```

**Configure the Tableau Server “readonly” user:**

This step is only required if you want to use the “What’s Going On?” dashboard in the provided sample workbook. TabMon does not actually utilize this user account in any way.

1.  Open command line as administrator

2.  Navigate to Tableau Server Bin Folder

3.  Run the following tabadmin commands

```
Tableau Server 2018.1 and Older
    tabadmin dbpass --username readonly [Password here]

    tabadmin restart
```

```
Tableau Server 2018.2+
    tsm data-access repository-access enable --repository-username readonly --repository-password <PASSWORD>
```
