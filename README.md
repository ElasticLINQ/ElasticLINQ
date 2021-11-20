# ElasticLINQ

ElasticLINQ is a free C# library for searching Elasticsearch using LINQ syntax in .NET 4.5/PCL, e.g.

```csharp
var db = new ElasticContext(new ElasticConnection(new Uri("http://myserver:9200")));
var p = db.Query<People>().Where(p => p.Tags.Contains("tech") && p.State == "WA");
```

For information on getting started, [see the Wiki](https://github.com/CenturyLinkCloud/ElasticLINQ/wiki/Getting-Started).

## Elasticsearch version compatibility

* **0.9 to 1.x** works great
* **2.x** GroupBy not supported

Beyond 2.x is unsupported.

## Builds

Binary releases are available via [NuGet](http://www.nuget.org/packages/ElasticLinq/) or [direct downloads](https://github.com/ElasticLINQ/ElasticLINQ/releases)

[![Build Status](https://ci.appveyor.com/api/projects/status/7p4c73ocmmwjc05q/branch/master?svg=true)](https://ci.appveyor.com/project/ElasticLINQ/elasticlinq/)![NuGet version](https://img.shields.io/nuget/v/elasticlinq.svg?style=flat)
