ElasticLINQ is a free C# library for searching Elasticsearch using LINQ syntax in .NET, e.g.

```csharp
var db = new ElasticContext(new ElasticConnection(new Uri("http://myserver:9200")));
var p = db.Query<People>().Where(p => p.Tags.Contains("tech") && p.State == "WA");
```

ElasticLINQ supports .NET 4.5 & PCL with Elasticsearch 0.9.0 or greater.

Binary releases are available via [NuGet](http://www.nuget.org/packages/ElasticLinq/). For information on getting started, [see the Wiki](https://github.com/CenturyLinkCloud/ElasticLINQ/wiki/Getting-Started).

-![Build status](https://img.shields.io/teamcity/http/teamcity.tier3.com/s/elasticlinq.svg) ![NuGet version](https://img.shields.io/nuget/v/elasticlinq.svg?style=flat)
