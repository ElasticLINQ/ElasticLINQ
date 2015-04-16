# ElasticLINQ release notes

## Getting started
To get started you need to create an ElasticConnection with the URL and timeout values (and optionally index name), e.g.

	var connection = new ElasticConnection(new Uri("http://192.168.2.12:9200"), Index: "myIndex");

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
* ``First``, ``FirstOrDefault`` maps to length 1, filter if predicate supplied
* ``Single``, ``SingleOrDefault`` maps to length 2, filter if predicate supplied
* ``Sum``, ``Average``, ``Min``, ``Max`` map to facets
* ``GroupBy`` causes facets to switch between termless and termed facets
* ``Count``, ``LongCount`` maps to count query if top level, facet if within a GroupBy

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
Where creates **filter** operations and supports the following patterns:

* `< > <= >=` maps to **range**
* `==` maps to **term** 
* `!=` maps to **term** inside a **not**
* `||` maps to **or**
* `&&` maps to **and**
* `HasValue`, ``!=null`` maps to **exists**
* `!HasValue`, ``==null`` maps to **missing**
* ``Equals`` for static and instance maps to **term**
* ``Contains`` on IEnumerable/array maps to **terms**

To create similar expression as **queries** use the .Query extension operator. It maps very similar operations but **exists** and **missing** are not available within queries on Elasticsearch.

#### OrderBy/ThenBy
Ordering is achieved by the usual LINQ methods however if you wish to sort by score you have two options:

1. Normal method with ElasticFields.Score static property ``OrderBy(o => ElasticFields.Score)``
2. IQueryable methods with score in the name ``OrderByScore()``

The latter is more easily discovered but the former should be kept around as it is the same pattern used in Select projections. Recommend the former for the less common orderings and latter for common.

#### GroupBy/aggregate operations
There are three different ways you can perform aggregate operations like Sum, Count, Min, Max and Average:

If you want aggregates broken down by a field:
``db.Query<Robot>().GroupBy(r => r.Zone).Select(g => new { Zone = g.Key, Count = g.Count() });``

If you want one aggregate for the entire set:
``db.Query<Robot>().Count();``

We also support a less well-known operation that lets you retrieve multiple aggregates for the set in a single hit using GroupBy and a constant value:
``db.Query<Robot>().GroupBy(r => 1).Select(g => new { Count = g.Count(), Sum = g.Sum(r => r.Cost) });``

### New operations
Where Elasticsearch exceeds the basic LINQ patterns some additional extensions are provided to expose that functionality.

####Extensions
A number of extensions on IQueryable are available via ``ElasticQueryExtensions`` to provide some of this functionality, this includes:

* ``OrderByScore``, ``OrderByScoreDescending``, ``ThenByScore``, ``ThenByScoreDescending`` to order by the _score field.
* ``Query`` to specify query criteria the same way ``Where`` maps to filter criteria.
* ``QueryString(query)`` to pass through a query_string for search.
* ``MinScore(value)`` to specify a minimum score.

####ElasticFields
There is a static class called ElasticFields. This currently provides just Score and Id properties but you can use these to stand-in for the _score and _id values in Elasticsearch, e.g:

``Select(c => new { c, ElasticFields.Score })`` wraps the entity with its score.
``OrderBy(c => ElasticFields.Score)`` orders results by score.

Note: To specify a minimum score in a Where please use the MinScore extension method. ElasticFields.Score is not supported for this as only the >= operator would be valid.

### Mapping
There is a mapping interface called IElasticMapping. A default CouchbaseElasticMapping is provided that maps against the current structure shown by Tier 3.

## Implementation notes

### Dependencies
JSON.Net is required for converting the Elasticsearch queries to JSON and parsing the results coming back.

XUnit.Net is required for the unit tests.

NSubstitute is required for some unit tests.

### Unit tests
Currently around 95% test coverage and some tests use an included HTTP listener to correct sending and receiving of requests.

### Query optimizations
The query translator supports a few query optimizations to ensure that the generated ElasticLINQ query looks good and not like it was translated from another language. 

This includes currently:

* Combining multiple == for same field in same term
* Combining multiple < > <= >= for same field into single range
* Combining OR and AND expression trees into flattened ORs and ANDs
* Cancelling out NOT when inside operation can be inverted

## Future

### Needs improvement
1. Formatting non-string values for querying - e.g. dates
2. Error reporting for unsupported LINQ syntax
3. Error reporting for bad or incorrect return data

### Not yet supported
1. Async<T>
2. Caching
3. Hooks for profiling
4. Any other desired Elasticsearch operations

### Not supported; not recommended

**Nested queries running multiple Elasticsearch requests**

Very complex to do and breaks the explicit boundaries recommended in LINQ providers.
