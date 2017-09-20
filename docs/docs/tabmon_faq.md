---
title: FAQ/Troubleshooting
layout: default
---


* TOC
{:toc}



**1.  How to I connect to the TabMon workbook?**

The TabMon workbook is included in the `Resources\\Sample Workbooks` folder in the TabMon install directory. Note that you should make sure to use the version of the Sample Workbook that matches the Tableau Server version. The SampleWorkbook is meant to fit on top of the schema set in the Counters.config file, so make sure you are using the correct Counters.config file for your version as well!

To connect the workbook to your own server, open the workbook and enter the following:

1.  Your read-only password for the data source: Server Auditing

2.  Your postgres DB name and password for you locally installed or remotely installed database (for the data source: TabMon)



**2.  What do all these counters represent?**

The counters collected by the default Counters.config file represent a common set of performance metrics that would be of interest to an average server admin. Information on the Perfmon counters can be found at Microsoft’s Perfmon counter reference at [*https://technet.microsoft.com/enus/library/cc768048.aspx*.](https://technet.microsoft.com/en-us/library/cc768048.aspx){:target="_blank"}


**3.  My “%Processor Time” is over 100%; how is this possible?**

TabMon uses the Windows Perfmon counter “Process\\% Processor Time” to calculate the amount of CPU used by individual processes. By the nature of this counter, for a single CPU (single core) the maximum value is 100%. For a multi-core system, the maximum percentage is \#cores \* 100% (for example a 4 core system will have a max of 400%).


**4.  The TabMon service won’t start.**

If TabMon is failing to start, it is likely due to a permissions issue. The most common cause of this is that the user trying to start TabMon does not have admin privileges, or else the account the TabMon service is running as does not have sufficient permissions. Remember that if you are using Remote Polling, then TabMon must be running as a user with appropriate permissions on all of the remote machines.

If you’ve double-checked all relevant permissions and TabMon still isn’t starting, a good way to get a clue is to check the most recent event from the application logs – see `C:\Program Files (x86)\TabMon\Logs\`.


**5.  I’m seeing tons of counter polling failures!**

Make sure you are using the proper `Counters.config` file for the version of Tableau Server that you are monitoring. See the [Configuring TabMon](tabmon_configure) section for more information on this. If you’re using the proper Counters.config file and still several failures, checking the logs can provide insight.

It is worth noting that systems can be very different and the machine(s) you are monitoring may simply not have certain counters.


**6.  TabMon is dropping JMX counters after I restart Tableau Server.**

This is normal. Restarting the server will cycle the open JMX ports and you will experience a temporary loss in connectivity for JMX counters. TabMon includes reconnection logic which will eventually automatically re-establish all of the connections.


**7.  Help! TabMon is generating way too much data!**

By default, TabMon samples all performance counters every minute and keeps that data around forever. Over the long term, this can amount to a lot of data!

There are a couple of ways to address this:

1.  If you are using TabMon to write to a database, you can set a data retention threshold so that old/expired data is automatically dropped. See [Set a Data Retention Threshold](tabmon_configure#set-a-data-retention-threshold) for details on how to do this.

2.  You can set the poll interval to a lower frequency so that less data is generated. You can do this by updating `PollInterval` in `TabMon.config`.

3.  You can update `Counters.config` to remove performance counters that you don’t care about, in order to reduce data volume.



**8.  What Processes/Services does TabMon monitor?**

TabMon tracks all Tableau Server related processes below (taken from [*http://onlinehelp.tableau.com/current/server/en-us/processes.htm*)](http://onlinehelp.tableau.com/current/server/en-us/processes.htm){:target="_blank"}:

| **Process**     | **File Name** | **Purpose**            | **Performance Characteristics**                                                                            |
|-----------------|---------------|------------------------|------------------------------------------------------------------------------------------------------------|
| **API Server** | wgserver.exe  | Handles REST API calls | Unless you are using REST APIs for critical business processes, this service can be down without impacting the overall health of Tableau Server.  |
| **Application Server**   | vizportal.exe                          | Handles the web application, supports browsing and searching                                          | Only consumes noticeable resources during infrequent operations, like publishing a workbook with an extract, or generating a static image for a view. Its load can be created by browser-based interaction and by tabcmd.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
| **Backgrounder**         | backgrounder.exe                       | Executes server tasks, including extract refreshes, ‘Run Now’ tasks, and tasks initiated from tabcmd  | A single-threaded process where multiple processes can be run on any or all machines in the cluster to expand capacity. The backgrounder normally doesn’t consume much process memory, but it can consume CPU, I/O, or network resources based on the nature of the workload presented to it. For example, performing large extract refreshes can use network bandwidth to retrieve data. CPU resources can be consumed by data retrieval or complex tabcmd tasks.                                                                                                                                                                                                                                                                                                                                                                                 
| **Cache Server**         | redis-server.exe                       | Query cache                                                                                           | A query cache distributed and shared across the server cluster. This in-memory cache speeds user experience across many scenarios. VizQL server, backgrounder, and data server (and API server and application server to a lesser extent) make cache requests to the cache server on behalf of users or jobs. The cache is single-threaded, so if you need better performance you should run additional instances of cache server.                                                                                                                                                                                                                                                                                                                                                                                                                 
| **Cluster Controller**   | clustercontroller.exe                  | Responsible for monitoring various components, detecting failures, and executing failover when needed | Included in the base install on every node.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
| **Coordination Service** | zookeeper.exe                          | In distributed installations, responsible for ensuring there is a quorum for making decisions during failover   | Included in the base install on every node.   |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
| **Data Engine**          | tdeserver64.exe tdeserver.exe (32-bit) | Stores data extracts and answers queries                                                              | The data engine's workload is generated by requests from the VizQL server, application server, API server, data server, and backgrounder server processes. The data engine services request from most of the other server processes as well. It is the component that loads extracts into memory and performs queries against them. Memory consumption is primarily based on the size of the data extracts being loaded. The 64-bit binary is used as the default on 64-bit operating systems, even if 32-bit Tableau Server is installed. The data engine is multi-threaded to handle multiple requests at a time. Under high load it can consume CPU, I/O, and network resources, all of which can be a performance bottleneck under load. At high load, a single instance of the data engine can consume all CPU resources to process requests. 
| **Data Server**          | dataserver.exe                         | Manages connections to Tableau Server data sources                                                    | Because it’s a proxy, it’s normally only bound by network, but it can be bound by CPU with enough simultaneous user sessions. Its load is generated by browser- and Tableau Desktop-based interaction and extract refresh jobs for Tableau Server data sources.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
| **File Store**           | filestore.exe                          | Automatically replicates extracts across data engine nodes                                            | Installed with data engine (cannot be installed separately). A file store process will always be present if there are one or more data engine processes installed.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
| **Repository**           | postgres.exe                           | Tableau Server database, stores workbook and user metadata                                            | Normally consumes few resources. It can become a bottleneck in rare cases for very large deployments (thousands of users) while performing operations such as viewing all workbooks by user or changing permissions. For more information, see [*Tableau* *Server Repository*. ](http://onlinehelp.tableau.com/current/server/en-us/server_process_repository.htm){:target="_blank"} |
| **Search & Browse**      | searchserver.exe                       | Handles fast search, filter, retrieval, and display of content metadata on the server                 | The process is memory bound first, and IO bound second. The amount of memory used scales with the amount of content (number of sites/projects/workbooks/datasources/views/users) on the server. |
| **VizQL Server**         | vizqlserver.exe                        | Loads and renders views, computes and executes queries                                                | Consumes noticeable resources during view loading and interactive use from a web browser. Can be CPU bound, I/O bound, or network bound. Process load can only be created by browser-based interaction. Can run out of process memory. |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  


**9.  How do I determine if I have system bottlenecks on my Server Instance?**

Microsoft has some general guidelines for server health available in help documents and online articles.

Refer to: [*https://technet.microsoft.com/en-us/magazine/2008.08.pulse.aspx* ](https://technet.microsoft.com/en-us/magazine/2008.08.pulse.aspx){:target="_blank"}

In general, here are some common bottlenecks/general guidelines **(these are NOT an authoritative nor is this a definitive guide to the server health; actual results will vary)**:

1.  Free Disk Space: Keep above 15% for critical systems

2.  Memory (RAM): Keep under 80% for extended periods of time

3.  Processor %: Keep under 85% for extended periods of time

4.  Network: Keep under 70% of the interface’s throughput (depending on network card)



**10.  What are cache hits/misses?**

   -   A Cache hit is a successful lookup in a cache server.

   -   A Cache miss is an unsuccessful lookup in a cache server

   -   Generally speaking, an effective cache server has more hits than misses (aka a high Cache Hit Ratio)
   

**11.  What is Request Latency and what does it mean for me?**

Request Latency is the time it takes for the host server to receive and process a request. Latency can be reduced by tweaking and upgrading computer hardware.


**12.  Can I use TabMon to monitor hosts that aren’t running Tableau?**

Absolutely! Just edit your Counters.config file to remove all of the MBean counters as well as the PerfMon counters that specifically relate to Tableau processes.

**13.  Where can I get additional support/feedback?**

As an open source product, TabMon is **Community Supported**. Please refer to the GitHub page for TabMon for more details:

[*http://github.com/tableau/TabMon* ](http://github.com/tableau/TabMon){:target="_blank"}


