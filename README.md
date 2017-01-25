ElasticLINQ is a free C# library for searching Elasticsearch using LINQ syntax in .NET, e.g.

```csharp
var db = new ElasticContext(new ElasticConnection(new Uri("http://myserver:9200")));
var p = db.Query<People>().Where(p => p.Tags.Contains("tech") && p.State == "WA");
```

ElasticLINQ supports .NET 4.5 & PCL.

## Elasticsearch version compatibility

* 0.9 works
* 1.x works
* 2.x facets fail, other queries might work
* 5.x [under development](https://github.com/ElasticLINQ/ElasticLINQ/tree/elasticsearch5 ) in

## Builds

Binary releases are available via [NuGet](http://www.nuget.org/packages/ElasticLinq/). For information on getting started, [see the Wiki](https://github.com/CenturyLinkCloud/ElasticLINQ/wiki/Getting-Started).

[![Build Status](https://ci.appveyor.com/api/projects/status/7p4c73ocmmwjc05q?svg=true)](https://ci.appveyor.com/project/ElasticLINQ/elasticlinq/)![NuGet version](https://img.shields.io/nuget/v/elasticlinq.svg?style=flat)
