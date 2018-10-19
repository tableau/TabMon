Usage Instructions!
Note: Anything between '[]' should be replaced with the description.

STEP 1: Log into tsm on the cluster that you wish to add to the TabMon config.
            Command: tsm login -u [USERNAME]

STEP 2: Pipe tsm topology list-ports to a file.
            Command: tsm topology list-ports > [OUTPUT_FILE]
            Example: tsm topology list-ports > "C:\topology_out.txt"

STEP 3: Run TabMonConfigBuilder with topology output and output file.
            Command: tabmonconfigbuilder [TARGET] [OUTPUT]
            Example: tabmonconfigbuilder "C:\topology_out.txt" "C:\output.txt"

STEP 4: Open "output.txt" and verify that the host and process count are
        correct. If there are any discrepancies, you may need to manually
        add or remove hosts and processes.

STEP 5: For each host, replace the worker in "computerName" and
        "address" entry to reflect the correct host information.

STEP 6: Copy the information below the line into the "Cluster section"
        of the TabMon.Config.

If there are any questions or issues, feel free to reach out via github.
    https://github.com/tableau/TabMon