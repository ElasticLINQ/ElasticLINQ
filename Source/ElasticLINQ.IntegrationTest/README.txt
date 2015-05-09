Note: To run these integration tests you will need an Elasticsearch server that has been loaded with
the WebUsers and JobPositions data models from http://github.com/damieng/SampleDomainData

To load them use the Populate-Elasticsearch.ps1 script from the SampleDomainData package, e.g.

./Populate-Elasticsearch.ps1 http://elasticlinq.cloudapp.net:9200 integrationtests *