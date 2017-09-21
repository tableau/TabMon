---
title: Upgrading from a Previous Version of TabMon
layout: default
---

The TabMon installer will handle most of the upgrade work for you, but during the upgrade your TabMon.config and Counters.config files will be overwritten. Please follow the following steps for a smooth upgrade experience:

-   Back up your TabMon.config and Counters.config files.

-   Run the TabMon installer. Make sure to deselect **Install and initialize Postgres for Me**.

-   Open up the `TabMon.config` file from the newly installed version and update the database and host options to match your backup copy. Note that the schema of this config can change between versions, so you shouldn’t just overwrite the full contents of TabMon.config with your backup, as there may be new options that are required.

-   Ensure you are using the correct `Counters.config` and Sample Workbook files for your Tableau Server version. See [Configuring TabMon](tabmon_configure).

-   Fire up TabMon!
