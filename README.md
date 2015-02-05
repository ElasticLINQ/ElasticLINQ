# ElasticLINQ

ElasticLINQ is a free C# library for searching Elasticsearch using LINQ syntax in .NET.

## Minimal example

```csharp
var db = new ElasticContext(new ElasticConnection(new Uri("http://myserver:9200")));
var people = db.Query<People>().Where(p => p.Interests.Contains("databases") && p.State == "WA");
```

## Installation
Binary releases are available via [NuGet](http://www.nuget.org/packages/ElasticLinq/).


## Getting started
For information on getting started, [see the Wiki](https://github.com/CenturyLinkCloud/ElasticLINQ/wiki/Getting-Started).
