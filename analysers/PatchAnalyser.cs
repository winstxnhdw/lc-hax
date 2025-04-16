using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
class PatchAnalyser : DiagnosticAnalyzer {
    internal const string DiagnosticID = "HAX002";

    static DiagnosticDescriptor Rule { get; } = new(
        DiagnosticID,
        "Patch instance requires double underscore prefix",
        "Instance parameter of Patch method '{0}' must be named '__instance'",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [PatchAnalyser.Rule];

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyseSyntaxNode, SyntaxKind.MethodDeclaration);
    }

    static void AnalyseSyntaxNode(SyntaxNodeAnalysisContext context) {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration) return;
        if (context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken) is not IMethodSymbol methodSymbol) return;
        if (!methodSymbol.ContainingType.Name.EndsWith("Patch")) return;
        if (!methodSymbol.Parameters.Any(parameter => parameter.Name is "instance")) return;

        context.ReportDiagnostic(
            Diagnostic.Create(PatchAnalyser.Rule, methodDeclaration.Identifier.GetLocation(), methodSymbol.Name)
        );
    }
}
