<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="d0ed9acb-0435-4532-afdd-b5115bc4d562" namespace="TabMon" xmlSchemaNamespace="TabMon" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="TabMonConfig" namespace="TabMon.Config" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="TabMonConfig">
      <elementProperties>
        <elementProperty name="Database" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="Database" isReadOnly="false" documentation="Contains configuration information for the results database.  Only required if OutputMode is &quot;DB&quot;.">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Database" />
          </type>
        </elementProperty>
        <elementProperty name="Clusters" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="Clusters" isReadOnly="false" documentation="Contains definitions of which machines to monitor and their logical clustering.">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Clusters" />
          </type>
        </elementProperty>
        <elementProperty name="PollInterval" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="PollInterval" isReadOnly="false" documentation="Contains information about the polling frequency.">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PollInterval" />
          </type>
        </elementProperty>
        <elementProperty name="OutputMode" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="OutputMode" isReadOnly="false" documentation="Contains information about the result output mode.">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/OutputMode" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="Cluster" namespace="TabMon.Config" documentation="Represents a logical grouping of hosts." xmlItemName="Host" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false" documentation="The name of the cluster.  Used to logically group multiple hosts." defaultValue="&quot;Primary&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Host" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="Database" namespace="TabMon.Config" documentation="Contains configuration information for the results database.  Only required if OutputMode is &quot;DB&quot;.">
      <attributeProperties>
        <attributeProperty name="Type" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="type" isReadOnly="false" documentation="The type of database that this is." defaultValue="&quot;Postgres&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Name" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="name" isReadOnly="false" documentation="The name of the instantiated database." defaultValue="&quot;TabMon&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Server" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="Server" isReadOnly="false" documentation="Contains information about the database server location.">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Server" />
          </type>
        </elementProperty>
        <elementProperty name="User" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="User" isReadOnly="false" documentation="Contains information about the database user.">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/User" />
          </type>
        </elementProperty>
        <elementProperty name="Table" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="Table" isReadOnly="false" documentation="Contains information about the results table.">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Table" />
          </type>
        </elementProperty>
        <elementProperty name="Indexes" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="Indexes" isReadOnly="false" documentation="Contains columns to be indexed upon table creation.">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Indexes" />
          </type>
        </elementProperty>
        <elementProperty name="PurgeOldData" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="PurgeOldData" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PurgeOldData" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="Server" namespace="TabMon.Config" documentation="Contains information about the database server location.">
      <attributeProperties>
        <attributeProperty name="Host" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="host" isReadOnly="false" documentation="The hostname or IP address of the database server." defaultValue="&quot;localhost&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Port" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="port" isReadOnly="false" documentation="The port number of the database server." defaultValue="5432">
          <validator>
            <integerValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PositiveInteger" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="User" namespace="TabMon.Config" documentation="Contains information about the database user.">
      <attributeProperties>
        <attributeProperty name="Login" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="login" isReadOnly="false" documentation="The login for the database user." defaultValue="&quot;tabmon&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Password" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="password" isReadOnly="false" documentation="The password of the database user." defaultValue="&quot;password&quot;">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="Table" namespace="TabMon.Config" documentation="Contains information about the results table.">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="name" isReadOnly="false" documentation="The name of the database table to write results to." defaultValue="&quot;countersamples&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="PollInterval" namespace="TabMon.Config" documentation="Contains information about the polling frequency.">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false" documentation="The interval between polling cycles, in seconds." defaultValue="30">
          <validator>
            <integerValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PositiveInteger" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="OutputMode" namespace="TabMon.Config" documentation="Contains information about the result output mode.">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false" documentation="The output mode.  &quot;CSV&quot; for local flat file output, or &quot;DB&quot; for database output." defaultValue="&quot;db&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="Clusters" namespace="TabMon.Config" documentation="Contains definitions of which machines to monitor and their logical clustering.  Each cluster may contain multiple hosts." xmlItemName="Cluster" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Cluster" />
      </itemType>
    </configurationElementCollection>
    <configurationElementCollection name="Indexes" namespace="TabMon.Config" documentation="Contains columns to be indexed upon table creation." xmlItemName="Index" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="Generate" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="generate" isReadOnly="false" documentation="Whether TabMon will generate and validate indexes upon startup." defaultValue="true">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Index" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="Index" namespace="TabMon.Config" documentation="Represents an individual index.">
      <attributeProperties>
        <attributeProperty name="Column" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="column" isReadOnly="false" documentation="The column name to be indexed." defaultValue="&quot;&quot;">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Clustered" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="clustered" isReadOnly="false" documentation="Whether the index is clustered or not." defaultValue="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="PurgeOldData">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="enabled" isReadOnly="false" documentation="Whether TabMon will purge data after a certain time." defaultValue="true">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="ThresholdDays" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="thresholdDays" isReadOnly="false" documentation="The age in days that a row has to exceed  before it is pruged." defaultValue="30">
          <validator>
            <integerValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PositiveInteger" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="Process" namespace="TabMon.Config" documentation="Represents a group of processes on a host." xmlItemName="Port" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="ProcessName" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="processName" isReadOnly="false" documentation="Name of the process." defaultValue="&quot;PROCESSNAME&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Port" />
      </itemType>
    </configurationElementCollection>
    <configurationElementCollection name="Host" namespace="TabMon.Config" documentation="Represents an individual host." xmlItemName="Process" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="ComputerName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="computerName" isReadOnly="false" documentation="The computer name of the host to monitor." defaultValue="&quot;YOURCOMPUTERNAME&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Address" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="address" isReadOnly="false" documentation="The hostname or IP address of the host to monitor." defaultValue="&quot;localhost&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="SpecifyPorts" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="specifyPorts" isReadOnly="false" documentation="Whether to manually specify the process ports in the config." defaultValue="&quot;false&quot;">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Process" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="Port" namespace="TabMon.Config" documentation="Represents a port.">
      <attributeProperties>
        <attributeProperty name="PortNumber" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="portNumber" isReadOnly="false" documentation="The number of the port." defaultValue="9999">
          <validator>
            <integerValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PositiveInteger" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="ProcessNumber" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="processNumber" isReadOnly="false" documentation="Process instance number." defaultValue="0">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators>
      <integerValidator name="PositiveInteger" minValue="1" />
      <stringValidator name="NonEmptyString" minLength="1" />
    </validators>
  </propertyValidators>
</configurationSectionModel>