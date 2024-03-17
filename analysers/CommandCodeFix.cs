using System.Linq;
using System.Threading;
using System.Composition;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CommandCodeFix)), Shared]
public class CommandCodeFix : CodeFixProvider {
    public override ImmutableArray<string> FixableDiagnosticIds => [CommandAnalyzer.DiagnosticID];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        Diagnostic diagnostic = context.Diagnostics.First();
        TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

        CodeAction codeAction = CodeAction.Create(
            "Implement ICommand interface",
            cancellationToken => this.ImplementICommandAsync(context.Document, diagnosticSpan, cancellationToken),
            nameof(CommandCodeFix)
        );

        context.RegisterCodeFix(codeAction, diagnostic);

        await Task.CompletedTask;
    }

    async Task<Document> ImplementICommandAsync(Document document, TextSpan diagnosticSpan, CancellationToken cancellationToken) {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First() is not ClassDeclarationSyntax classDeclaration) {
            return document;
        }

        BaseListSyntax baseList = classDeclaration?.BaseList ?? SyntaxFactory.BaseList();
        BaseListSyntax newBaseList = baseList.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("ICommand")));

        return classDeclaration?.WithBaseList(newBaseList) is not ClassDeclarationSyntax newClassDeclaration
            ? document
            : document.WithSyntaxRoot(root.ReplaceNode(classDeclaration, newClassDeclaration));
    }
}
