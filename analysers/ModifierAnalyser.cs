using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ModifierAnalyser : DiagnosticAnalyzer {
    public const string DiagnosticID = "HAX003";

    static DiagnosticDescriptor Rule { get; } = new(
        DiagnosticID,
        "Private modifier is explicit",
        "The `private` modifier is redundant and can be removed.",
        "Style",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration);
    }

    static void AnalyzeNode(SyntaxNodeAnalysisContext context) {
        if (context.Node as MemberDeclarationSyntax is not { Modifiers: SyntaxTokenList modifiers }) return;

        foreach (SyntaxToken modifier in modifiers) {
            if (!modifier.IsKind(SyntaxKind.PrivateKeyword)) continue;

            Diagnostic diagnostic = Diagnostic.Create(Rule, modifier.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
