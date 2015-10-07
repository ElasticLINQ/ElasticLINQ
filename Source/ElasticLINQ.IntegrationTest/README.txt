Note: To run these integration tests you will need an Elasticsearch server that has been loaded with
the WebUsers and JobPositions data models from http://github.com/damieng/SampleDomainData

To load them use the Populate-Elasticsearch.ps1 script from the SampleDomainData package, e.g.

./Populate-Elasticsearch.ps1 http://integration.elasticlinq.net:9200 integrationtest *

This will load all documents from the sample data into the new index 'integrationtest'.