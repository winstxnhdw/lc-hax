using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
class CommandAnalyzer : DiagnosticAnalyzer {
    internal const string DiagnosticID = "HAX001";

    static DiagnosticDescriptor Rule { get; } = new(
        DiagnosticID,
        "Implement ICommand Interface",
        "Command '{0}' must implement ICommand",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [CommandAnalyzer.Rule];

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyseSyntaxNode, SyntaxKind.ClassDeclaration);
    }

    static void AnalyseSyntaxNode(SyntaxNodeAnalysisContext context) {
        if (context.Node is not ClassDeclarationSyntax classDeclaration) return;
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken) is not INamedTypeSymbol namedTypeSymbol) return;
        if (!namedTypeSymbol.Name.EndsWith("Command") || ImplementsICommand(namedTypeSymbol)) return;

        context.ReportDiagnostic(
            Diagnostic.Create(CommandAnalyzer.Rule, classDeclaration.Identifier.GetLocation(), namedTypeSymbol.Name)
        );
    }

    static bool ImplementsICommand(INamedTypeSymbol classSymbol) => classSymbol.AllInterfaces.Any(i => i.Name is "ICommand");
}
