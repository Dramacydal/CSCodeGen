﻿using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public class FileGenerator : IGenerator
{
    public ClassGenerator ClassGenerator { get; set; }
    
    public IEnumerable<CodeLine> Generate(GenerationContext context)
    {
        LineGenerator g = new(context);

        g.Create($"namespace {context.Namespace};");
        g.CreateEmpty();

        g.Lines.AddRange(ClassGenerator.Generate(context));

        var aliases = context.BuildAliases();

        if (aliases.Count > 0)
            g.Lines.Insert(0, context.CreateLine());

        foreach (var t in aliases)
            g.Lines.Insert(0, context.CreateLine($"using {t.Key} = {t.Value};"));
        
        var usings = context.BuildUsings();

        if (usings.Count > 0)
            g.Lines.Insert(0, context.CreateLine());

        foreach (var t in usings)
            g.Lines.Insert(0, context.CreateLine($"using {t};"));

        return g.Lines;
    }
}
