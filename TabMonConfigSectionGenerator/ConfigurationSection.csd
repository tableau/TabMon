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
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Host" />
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
      </elementProperties>
    </configurationElement>
    <configurationElement name="Host" namespace="TabMon.Config" documentation="Represents an individual host.">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false" documentation="The hostname or IP address of the host to monitor." defaultValue="&quot;localhost&quot;">
          <validator>
            <stringValidatorMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/NonEmptyString" />
          </validator>
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
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
  </configurationElements>
  <propertyValidators>
    <validators>
      <integerValidator name="PositiveInteger" minValue="1" />
      <stringValidator name="NonEmptyString" minLength="1" />
    </validators>
  </propertyValidators>
</configurationSectionModel>