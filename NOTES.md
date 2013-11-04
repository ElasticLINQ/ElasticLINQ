# ElasticLINQ release notes

Welcome to the first drop of ElasticLINQ. As this is a work in progress there are a number of things to point out and get feedback on so please review this document carefully.

## Getting started
To get started you need to create an ElasticConnection with the URL and timeout values (and optionally index name), e.g.

	var connection = new ElasticConnection(new Uri("http://192.168.2.12:9200"), TimeSpan.FromSeconds(10), "myIndex");

Then create an ElasticContext with the connection passed in the constructor and the mapping provider to use:

	var context = new ElasticContext(connection, new CouchbaseElasticMapping());

Now you are ready to execute queries:

	var results = context.Query<Customers>().Where(c => c.EndDate < DateTime.Now);

## Supported features
A number of the basic operations are supported including:

### LINQ operators

* ``Skip(n)`` maps to length
* ``Take(n)`` maps to count
* ``Select(...)`` may map to fields (see below)
* ``Where(...)`` may map to filter or query (see below)
* ``OrderBy``, ``OrderByDescending`` maps to sort (see below)
* ``ThenBy``, ``ThenByDescending`` maps to sort desc (see below)

#### Select
Select is supported and detects whole entity vs field selection with field, anonymous object and Tuple creation patterns:

Examples of supported whole-entity selects:

    Select()
    Select(r => r)
	Select(r => new { r })
	Select(r => Tuple.Create(r))
    Select(r => new { r, ElasticFields.Score })
    Select(r => Tuple.Create(r, ElasticFields.Score, r.Name)

Examples of supported name-field selects:

    Select(r => r.Name)
	Select(r => new { r.Name })
	Select(r => Tuple.Create(r.Name))
    Select(r => new { r.Name, ElasticFields.Score })
    Select(r => Tuple.Create(r.Id, ElasticFields.Score, r.Name)

#### Where
Where creates filter operations and supports the following patterns:

* `< > <= >=` maps to **range**
* `==` maps to **term** 
* `!=` maps to **term** inside a **not**
* `||` maps to **or**
* `&&` maps to **and**
* `HasValue`, ``!=null`` maps to **exists**
* `!HasValue`, ``==null`` maps to **missing**
* ``Equals`` for static and instance maps to **term**
* ``Contains`` on IEnumerable/arrays maps to **terms**

To create similar expression as queries use the .Query extension operator. It maps very similar operations but **exists** and **missing** are not available within queries on ElasticSearch.

#### OrderBy/ThenBy
Ordering is achieved by the usual LINQ methods however if you wish to sort by score you have two options:

1. Normal method with ElasticFields.Score ``OrderBy(o => ElasticFields.Score)``
2. IQueryable methods with score in the name ``OrderByScore()``

The latter is more easily discovered but the former should be kept around as it is the same pattern used in Select projections. Recommend the former for the less common orderings and latter for common.

### New operations
Where ElasticSearch exceeds the basic LINQ patterns some additional extensions are provided to expose that functionality.

####Extensions
A number of extensions on IQueryable are available via ``ElasticQueryExtensions`` to provide some of this functionality, this includes:

* ``OrderByScore``, ``OrderByScoreDescending``, ``ThenByScore``, ``ThenByScoreDescending`` to order by the _score field.
* ``Query`` to specify query criteria the same way ``Where`` maps to filter criteria.
* ``QueryString(query)`` to pass through a query_string for search.

####ElasticFields
There is a static class called ElasticFields. This currently provides just Score and Id properties but you can use these to stand-in for the _score and _id values in ElasticSearch, e.g:

``Select(c => new { c, ElasticSearch.Score })`` wraps the entity with its score.
``OrderBy(c => ElasticSearch.Score)`` orders results by score.

### Mapping
There is a mapping interface called IElasticMapping. A default CouchbaseElasticMapping is provided that maps against the current structure shown by Tier 3.

## Implementation notes

### Licensing
The code is able to be licensed under Apache 2 as it is not based on IQToolkit.

### Dependencies
JSON.Net is required for serializing the ElasticSearch queries to JSON and parsing the results coming back.

XUnit.Net is required for the unit tests.

### Unit tests
Currently around 85% test coverage.

Unit tests are very isolated and it would be a good idea to implement some integration tests to exercise the remaining parts of the pipeline. Doing so would require either an ElasticSearch instance or some kind of fake/mock to sit in at the HTTP layer.  

### Query optimizations
The query translator supports a few query optimization's to ensure that the generated ElasticLINQ query looks good and not like it was translated from another language. 

This includes currently:

* Combining multiple == for same field in same term
* Combining multiple < > <= >= for same field into single range
* Combining CLR OR and AND trees into flattened ORs and ANDs
* Cancelling out NOT around a NOT
* Converting missing to exists and exists to missing when in a NOT

## Future

### Needs improvement
1. Formatting non-string values for querying - e.g. dates
2. Error handling for unsupported LINQ syntax
3. Error handling for bad or incorrect return data

### Not yet supported
1. Primary key look-up to other DB (e.g. Couchbase)
2. Count operations
3. Facets
4. Async<T>
5. Caching
6. Any other desired ElasticSearch operations

### Not supported; not recommended

**Nested queries running multiple ElasticSearch requests**

Very complex to do and breaks the explicit boundaries recommended in LINQ providers.

**HTTP GET queries**

Very limited in scope as to what it can achieve.