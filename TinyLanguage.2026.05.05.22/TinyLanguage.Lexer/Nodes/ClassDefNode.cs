using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a class definition (Implementation Note 16):
    //   ["static"] "class" <id> ["extends" <id>] ["implements" <id_list>]
    //   "{" <member_list> "}"
    // An empty class body is valid (member_list may be empty).
    public class ClassDefNode : AstNode
    {
        // The class name.
        public string Id { get; }

        // The name of the superclass, or null when "extends" is absent.
        public string ExtendsId { get; }

        // The list of implemented interface names (empty when "implements" is absent).
        public IReadOnlyList<string> ImplementsIds { get; }

        // The members: MethodDefNode, FieldDeclareNode, ConstDeclareNode,
        // ConstructorDefNode, EnumDefNode.
        public IReadOnlyList<AstNode> Members { get; }

        // True when the "static" keyword preceded the "class" keyword.
        public bool IsStatic { get; }

        public ClassDefNode(
            int line,
            string id,
            string extendsId,
            IReadOnlyList<string> implementsIds,
            IReadOnlyList<AstNode> members,
            bool isStatic)
            : base(line)
        {
            Id = id;
            ExtendsId = extendsId;
            ImplementsIds = implementsIds;
            Members = members;
            IsStatic = isStatic;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitClassDefNode(this);
        }
    }
}
